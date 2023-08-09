// Copyright (c) Microsoft. All rights reserved.

namespace Shared.Models;

public record ApproachResponse(
    string Answer,
    string? Thoughts,
    string[] DataPoints,
    string CitationBaseUrl,
    string[] GroupIds,
    string? Error = null);
