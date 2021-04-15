using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{
    public class Document
    {
        public string Id { get; set; }

        public bool IsCloudStorageDocument { get; set; }

        public string StoragePath { get; set; }

        public string FileName { get; set; }

    }
}
