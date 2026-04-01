using WzComparerR2;
using WzComparerR2.Config;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using System;

namespace HeadBucket
{
    public class HeadBucketModel
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
            string secretId = Environment.GetEnvironmentVariable(WcR2config.SecretID);
            string secretKey = Environment.GetEnvironmentVariable(WcR2config.SecretKey);
            long durationSecond = 600; //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            this.cosXml = new CosXmlServer(config, qCloudCredentialProvider);
        }

        HeadBucketModel()
        {
            InitCosXml();
        }

        public void HeadBucket()
        {
            try
            {
                var WcR2config = WcR2Config.Default;
                // 存储桶名称，此处填入格式必须为 BucketName-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer
                string bucket = WcR2config.Bucket;
                HeadBucketRequest request = new HeadBucketRequest(bucket);
                //执行请求
                HeadBucketResult result = cosXml.HeadBucket(request);
                //请求成功
                //Console.WriteLine(result.GetResultInfo());
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

        public static void HeadBucketModelMain()
        {
            HeadBucketModel m = new HeadBucketModel();
            m.HeadBucket();
        }
    }
}