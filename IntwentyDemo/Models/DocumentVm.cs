using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntwentyDemo.Models
{
    public class DocumentVm
    {
        public string Id { get; set; }

        public bool IsCloudStorageDocument { get; set; }

        public string StoragePath { get; set; }

        public string FileName { get; set; }

    }
}
