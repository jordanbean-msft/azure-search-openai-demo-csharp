// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Components.Authorization;

namespace ClientApp.Pages;

public sealed partial class Chat
{
    private string _userQuestion = "";
    private UserQuestion _currentQuestion;
    private string _lastReferenceQuestion = "";
    private bool _isReceivingResponse = false;

    private readonly Dictionary<UserQuestion, ApproachResponse?> _questionAndAnswerMap = new();

    [Inject] public required ISessionStorageService SessionStorage { get; set; }

    [Inject] public required ApiClient ApiClient { get; set; }

    [CascadingParameter(Name = nameof(Settings))]
    public required RequestSettingsOverrides Settings { get; set; }

    [CascadingParameter(Name = nameof(IsReversed))]
    public required bool IsReversed { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> _authenticationStateTask { get; set; }

    private Task OnAskQuestionAsync(string question)
    {
        _userQuestion = question;
        return OnAskClickedAsync();
    }

    private async Task OnAskClickedAsync()
    {
        if (string.IsNullOrWhiteSpace(_userQuestion))
        {
            return;
        }

        var user = (await _authenticationStateTask).User;

        if(user.Identity == null || !user.Identity.IsAuthenticated)
        {
            _questionAndAnswerMap[_currentQuestion] = new ApproachResponse(
                $"HTTP 404: You must be logged in to ask a question.",
                null,
                Array.Empty<string>(),
                "You must be logged in to ask a question.");
            return;
        }

        _isReceivingResponse = true;
        _lastReferenceQuestion = _userQuestion;
        _currentQuestion = new(_userQuestion, DateTime.Now);
        _questionAndAnswerMap[_currentQuestion] = null;

        try
        {
            var history = _questionAndAnswerMap
                .Where(x => x.Value is not null)
                .Select(x => new ChatTurn(x.Key.Question, x.Value!.Answer))
                .ToList();

            history.Add(new ChatTurn(_userQuestion));

            var request = new ChatRequest(history.ToArray(), Settings.Approach, Settings.Overrides);


            var result = await ApiClient.ChatConversationAsync(request);

            _questionAndAnswerMap[_currentQuestion] = result.Response;
            if (result.IsSuccessful)
            {
                _userQuestion = "";
                _currentQuestion = default;
            }
        }
        finally
        {
            _isReceivingResponse = false;
        }
    }

    private void OnClearChat()
    {
        _userQuestion = _lastReferenceQuestion = "";
        _currentQuestion = default;
        _questionAndAnswerMap.Clear();
    }
}
