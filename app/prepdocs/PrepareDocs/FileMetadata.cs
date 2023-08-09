// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrepareDocs;

internal record class FileMetadata(
    string FilePath,
    List<string> GroupIds);

internal record class FilesMetadata(
    List<FileMetadata> Files);
