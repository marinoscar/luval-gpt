using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.BlobStorage.S3
{
    public class S3FileManager : IBlobFileManager
    {

        protected virtual IAmazonS3 Client { get; private set; }
        public string BucketName { get; private set; }

        public S3FileManager(string key, string secret, string bucketName)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(secret)) throw new ArgumentNullException(nameof(secret));
            if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));

            var credentials = new BasicAWSCredentials(key, secret);
            Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
            BucketName = bucketName;
        }

        public async Task<BlobResult> UploadAsync(Blob blob)
        {
            if(blob == null) throw new ArgumentNullException(nameof(blob));
            if (string.IsNullOrWhiteSpace(blob.Name)) throw new ArgumentNullException(nameof(blob.Name));

            var p = blob.Properties.ToDictionary(p => p.Key, p => (object)p.Value);
            await Client.UploadObjectFromStreamAsync(BucketName, blob.Name, blob.Content, p);
            return new BlobResult(blob)
            {
                ObjectUrl = GetS3Url(blob.Name)
            };
        }

        public BlobResult Upload(Blob blob)
        {
            if (blob == null) throw new ArgumentNullException(nameof(blob));
            if (string.IsNullOrWhiteSpace(blob.Name)) throw new ArgumentNullException(nameof(blob.Name));

            var transfer = new TransferUtility(Client);
            var request = new TransferUtilityUploadRequest()
            {
                BucketName = BucketName,
                InputStream = blob.Content,
                Key = blob.Name
            };
            foreach (var kv in blob.Properties)
                request.Metadata.Add(kv.Key, kv.Value);

            
            transfer.Upload(request);

            return new BlobResult(blob)
            {
                ObjectUrl = GetS3Url(blob.Name)
            };
        }

        private string GetS3Url(string name)
        {
            return $"https://{BucketName.ToLowerInvariant().Trim()}.s3.amazonaws.com/{name}";
        }
    }
}
