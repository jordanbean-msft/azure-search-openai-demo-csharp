﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Components.Authorization;

namespace ClientApp.Pages;

public sealed partial class Ask
{
    private string _userQuestion = "";
    private string _lastReferenceQuestion = "";
    private bool _isReceivingResponse = false;
    private ApproachResponse? _approachResponse = null;

    [Inject] public required ISessionStorageService SessionStorage { get; set; }
    [Inject] public required ApiClient ApiClient { get; set; }

    [CascadingParameter(Name = nameof(Settings))]
    public required RequestSettingsOverrides Settings { get; set; }
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

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            _approachResponse = new ApproachResponse(
                $"HTTP 404: You must be logged in to ask a question.",
                null,
                Array.Empty<string>(),
                "You must be logged in to ask a question.");
            return;
        }

        _isReceivingResponse = true;
        _lastReferenceQuestion = _userQuestion;

        try
        {
            var request = new AskRequest(
                Question: _userQuestion,
                Approach: Settings.Approach,
                Overrides: Settings.Overrides);

            var result = await ApiClient.AskQuestionAsync(request);
            _approachResponse = result.Response;
        }
        finally
        {
            _isReceivingResponse = false;
        }
    }

    private void OnClearChat()
    {
        _userQuestion = _lastReferenceQuestion = "";
        _approachResponse = null;
    }
}
