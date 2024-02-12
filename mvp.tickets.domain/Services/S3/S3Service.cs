using Minio;
using Minio.DataModel.Args;
using mvp.tickets.domain.Models;
using System.IO;

namespace mvp.tickets.domain.Services
{
    public class S3Service: IS3Service
    {
        private readonly ISettings _settings;
        private readonly IMinioClient _s3Client;
        public S3Service(ISettings settings, IMinioClient s3Client)
        {
            _settings = settings;
            _s3Client = s3Client;
            //_s3Client = new AmazonS3Client(settings.S3.AccessKey, settings.S3.SecretKey, new AmazonS3Config
            //{
            //    pref
            //    ServiceURL = settings.S3.Url,
            //    ForcePathStyle = true,
            //    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(settings.S3.Region)
            //});
        }

        public async Task<bool> PutObjectAsync(string fileName, Stream stream)
        {
            try
            {
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_settings.S3.Bucket)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType("application/octet-stream")
                    .WithObject(fileName);

                await _s3Client.PutObjectAsync(putObjectArgs);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<byte[]> GetObjectStreamAsync(string fileName)
        {
            try
            {
                byte[] bytes = null;
                var req = new GetObjectArgs()
                    .WithBucket(_settings.S3.Bucket)
                    .WithObject(fileName)
                    .WithCallbackStream(st =>
                    {
                        var ms = new MemoryStream();
                        st.CopyTo(ms);
                        bytes = ms.ToArray();
                    });

                await _s3Client.GetObjectAsync(req);
                return bytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task DeleteObjectAsync(string fileName)
        {
            try
            {
                var req = new RemoveObjectArgs()
                    .WithBucket(_settings.S3.Bucket)
                    .WithObject(fileName);

                await _s3Client.RemoveObjectAsync(req);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CopyObjectAsync(string sourceFileName, string destinationFileName)
        {
            try
            {
                var cpSrcArgs = new CopySourceObjectArgs()
                    .WithBucket(_settings.S3.Bucket)
                    .WithObject(sourceFileName);
                var args = new CopyObjectArgs()
                    .WithBucket(_settings.S3.Bucket)
                    .WithObject(destinationFileName)
                    .WithCopyObjectSource(cpSrcArgs);
                await _s3Client.CopyObjectAsync(args);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
