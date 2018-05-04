using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageBlob.Models;
using ImageResizer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AzureStorageBlob.Helpers
{
    public class StorageHelper
    {
        private readonly StorageConfig _storageConfig = null;

        public StorageHelper(StorageConfig storageConfig)
        {
            _storageConfig = storageConfig;
        }

        public async Task<JsonImage> Upload(Stream stream, string nameFile)
        {
            var url = await UploadFileToStorage(stream, nameFile, _storageConfig);

            return new JsonImage()
            {
                Url = url,
            };
        }

        private static async Task<string> UploadFileToStorage(Stream fileStream, string fileName,
            StorageConfig storageConfig)
        {
            var storageCredentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(storageConfig.ImageContainer);
            var blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(fileStream);

            return blockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString();
        }

        public bool IsImage(string nameFile)
        {
            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
            return formats.Any(item => nameFile.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
