using WzComparerR2;
using WzComparerR2.Config;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using System;
namespace DeleteBucket
{
    public class DeleteBucketModel
    {
        public CosXml cosXml;

        // 初始化COS服务实例
        private void InitCosXml()
        {
            var WcR2config = WcR2Config.Default;
            string region = Environment.GetEnvironmentVariable(WcR2config.Region);
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region) // 设置默认的地域, COS 地域的简称请参照 https://cloud.tencent.com/document/product/436/6224
                .Build();
            string secretId = Environment.GetEnvironmentVariable(WcR2config.SecretID); // 云 API 密钥 SecretId, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
            string secretKey = Environment.GetEnvironmentVariable(WcR2config.SecretKey); // 云 API 密钥 SecretKey, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
            long durationSecond = 600; //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            this.cosXml = new CosXmlServer(config, qCloudCredentialProvider);
        }

        DeleteBucketModel()
        {
            InitCosXml();
        }

        public void DeleteBucket()
        {
            try
            {
                var WcR2config = WcR2Config.Default;
                // 存储桶名称，此处填入格式必须为 BucketName-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer
                string bucket = WcR2config.Bucket;
                DeleteBucketRequest request = new DeleteBucketRequest(bucket);
                //执行请求
                DeleteBucketResult result = cosXml.DeleteBucket(request);
                //请求成功
                Console.WriteLine(result.GetResultInfo());
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Console.WriteLine("CosClientException: " + clientEx);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Console.WriteLine("CosServerException: " + serverEx.GetInfo());
            }
        }

        public static void DeleteObjectModelMain()
        {
            DeleteBucketModel m = new DeleteBucketModel();
            m.DeleteBucket();
        }
    }
}