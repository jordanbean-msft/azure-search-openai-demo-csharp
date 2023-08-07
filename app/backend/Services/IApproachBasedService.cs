// Copyright (c) Microsoft. All rights reserved.

using System.Security.Claims;

namespace MinimalApi.Services;

public interface IApproachBasedService
{
    Approach Approach { get; }

    Task<ApproachResponse> ReplyAsync(
        string question,
        ClaimsPrincipal user,
        RequestOverrides? overrides = null,
        CancellationToken cancellationToken = default);
}
