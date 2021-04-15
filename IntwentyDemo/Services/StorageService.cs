using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Intwenty;
using Intwenty.Model;
using Intwenty.Interface;
using Intwenty.Services;
using IntwentyDemo.Models;

namespace IntwentyDemo.Services
{

    public interface IStorageService
    {
        Task<bool> DeleteDocument(string filename);

        Task<string> StoreDocument(string filename, Stream document);

        void SetSharedAccessURI(List<DocumentVm> documentlist);
    }


    public class StorageService : AzureBlobStorageService, IStorageService
    {

        public StorageService(IOptions<IntwentySettings> setings, IIntwentyDbLoggerService dblogger) : base(setings, dblogger)
        {

        }

        public void SetSharedAccessURI(List<DocumentVm> documentlist)
        {
            base.SetSharedAccessURI(documentlist);
        }
    }
}
