
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Intwenty;
using Intwenty.Interface;

namespace IntwentyDemo.Services
{

    public interface IDocumentStorageService
    {
        Task<bool> DeleteDocument(string filename);
        Task<string> StoreDocument(string filename, Stream document);
        Task SetDocumentURIs(List<DocumentVm> doclist);
    }


    public class DocumentStorageService : IDocumentStorageService
    {
        private IIntwentyDbLoggerService DbLogger { get; }
        private IOptions<IntwentySettings> Settings { get; }
        private IIntwentyDataService DataService { get; }

        public DocumentStorageService(IOptions<IntwentySettings> setings, IIntwentyDataService ds, IIntwentyDbLoggerService dblogger)
        {
            this.Settings = setings;
            this.DataService = ds;
            this.DbLogger = dblogger;
        }

        public async Task<bool> DeleteDocument(string blobname)
        {
            try
            {
                BlobServiceClient account = new BlobServiceClient(Settings.Value.StorageConnectionString);
                BlobContainerClient containerclient = account.GetBlobContainerClient(Settings.Value.StorageContainer);
                await containerclient.DeleteBlobAsync(blobname);
            }
            catch (Exception ex)
            {
                await DbLogger.LogErrorAsync(String.Format("Error in DocumentStorageService.DeleteDocument {0}", ex.Message));
            }
            return true;
        }

        public async Task<string> StoreDocument(string filename, Stream document)
        {
          
            try
            {
                BlobServiceClient account = new BlobServiceClient(Settings.Value.StorageConnectionString);
                BlobContainerClient containerclient = account.GetBlobContainerClient(Settings.Value.StorageContainer);
                var res = await containerclient.UploadBlobAsync(filename, document);

                document.Dispose();

                return res.Value.VersionId;

            }
            catch(Exception ex) 
            {
                await DbLogger.LogErrorAsync(String.Format("Error in DocumentStorageService.StoreDocument {0}", ex.Message));
            }

            return string.Empty;
        }

        /// <summary>
        /// Fetch URI's for shared access
        /// </summary>
        public async Task SetDocumentURIs(List<DocumentVm> doclist)
        {
            try
            {
                BlobServiceClient account = new BlobServiceClient(Settings.Value.StorageConnectionString);
                BlobContainerClient containerclient = account.GetBlobContainerClient(Settings.Value.StorageContainer);

                foreach (var d in doclist.FindAll(p => p.IsCloudStorageDocument))
                {
                   
                }

            }
            catch (Exception ex)
            {
                await DbLogger.LogErrorAsync(String.Format("Error in DocumentStorageService.SetDocumentURIs {0}", ex.Message));
            }
        }

    }
}
