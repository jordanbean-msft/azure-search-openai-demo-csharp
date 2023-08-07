// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrepareDocs;

public class FileMetadata
{
    public string FilePath { get; set; }
    public List<string> GroupIds { get; set; }
}

public class FilesMetadata
{
    public List<FileMetadata> Files { get; set; }
}
