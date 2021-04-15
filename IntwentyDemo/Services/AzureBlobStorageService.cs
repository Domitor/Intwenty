using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using Intwenty.Model;
using Intwenty.Model.Dto;
using System.Threading.Tasks;
using Intwenty.Interface;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using IntwentyDemo.Models;

namespace Intwenty.Services
{

    public interface IStorageService
    {
        Task<bool> DeleteDocument(string filename);

        Task<string> StoreDocument(string filename, Stream document);

        void SetSharedAccessURI(List<DocumentVm> documentlist, string resourcetype = "b", int expireminutes = 60);
    }

    public class AzureBlobStorageService
    {

        protected IntwentySettings Settings { get;  }
        protected IIntwentyDbLoggerService DbLogger { get; }

        public AzureBlobStorageService(IOptions<IntwentySettings> settings, IIntwentyDbLoggerService dblogger)
        {
            Settings = settings.Value;
            DbLogger = dblogger;
        }

        public async Task<string> StoreDocument(string filename, Stream document)
        {

            try
            {

                BlobServiceClient blobServiceClient = new BlobServiceClient(Settings.StorageConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Settings.StorageContainerName);

                BlobClient blobClient = containerClient.GetBlobClient(filename);
                await blobClient.UploadAsync(document, true);

                document.Dispose();

                return blobClient.Uri.ToString();


            }
            catch (Exception ex)
            {
                await DbLogger.LogErrorAsync("Error in AzureBlobStorageService.StoreDocument(): " + ex.Message);
            }

            return string.Empty;
        }

        public async Task<bool> DeleteDocument(string filename)
        {
            try
            {
                
                BlobServiceClient blobServiceClient = new BlobServiceClient(Settings.StorageConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Settings.StorageContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(filename);
                await blobClient.DeleteAsync();

            }
            catch (Exception ex)
            {
                await DbLogger.LogErrorAsync("Error in AzureBlobStorageService.DeleteDocument(): " + ex.Message);
            }
            return true;
        }

       
        /// <summary>
        /// Assigns shared access URIs with read permissions to the documents in the list, resourcetype is set default to b (blobs)
        /// </summary>
        public void SetSharedAccessURI(List<DocumentVm> documentlist, string resourcetype="b", int expireminutes=60)
        {
            try
            {
            
                StorageSharedKeyCredential credential = new StorageSharedKeyCredential(Settings.StorageName, Settings.StorageSharedKey);

                //build the blob container url
                string blobcontainer_url = string.Format("https://{0}.blob.core.windows.net/{1}", Settings.StorageName, Settings.StorageContainerName);

                //directly build BlobContainerClient, then pass it to GetServiceSasUriForContainer() method
                BlobContainerClient containerClient = new BlobContainerClient(new Uri(blobcontainer_url), credential);


                foreach (var d in documentlist.FindAll(p => p.IsCloudStorageDocument))
                {
                    var blobclient = containerClient.GetBlobClient(d.FileName);
                    BlobSasBuilder sasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = blobclient.BlobContainerName,
                        BlobName = blobclient.Name,
                        Resource = resourcetype
                    };

                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expireminutes);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    Uri sasUri = blobclient.GenerateSasUri(sasBuilder);

                    d.StoragePath = sasUri.ToString();
                }
                
            }
            catch (Exception ex)
            {
                DbLogger.LogErrorAsync("Error in AzureBlobStorageService.SetDocumentURIs(): " + ex.Message);
            }
        }

    }
}
