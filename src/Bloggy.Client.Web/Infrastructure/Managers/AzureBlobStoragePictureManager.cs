using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Bloggy.Client.Web.Infrastructure.Managers
{
    // refer to: https://gist.github.com/tugberkugurlu/3621671
    public class AzureBlobStoragePictureManager : IPictureManager
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobStoragePictureManager(CloudBlobClient blobClient)
        {
            if (blobClient == null) throw new ArgumentNullException("blobClient");
            _blobClient = blobClient;
        }

        public async Task<string> UploadAsync(Stream inputStream, string containerReferenceName, string pictureReferenceName, string contentType)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerReferenceName);
            if (await container.CreateIfNotExistsAsync().ConfigureAwait(false))
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }

            CloudBlockBlob blob = container.GetBlockBlobReference(pictureReferenceName);
            blob.Properties.ContentType = contentType;
            await blob.UploadFromStreamAsync(inputStream).ConfigureAwait(false);

            return blob.Uri.AbsoluteUri;
        }
    }
}