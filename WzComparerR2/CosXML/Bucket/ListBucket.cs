using WzComparerR2;
using WzComparerR2.Config;
using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model;
using COSXML.Model.Bucket;
using COSXML.Model.Service;
using COSXML.Model.Tag;
using System;
using System.Collections.Generic;

namespace ListBucket
{
    public class ListBucketModel
    {
        public CosXml cosXml;

        // 初始化COS服务实例
        private void InitCosXml()
        {
            var WcR2config = WcR2Config.Default;
            string region = Environment.GetEnvironmentVariable(WcR2config.Region);
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region)
                .Build();
            string secretId = Environment.GetEnvironmentVariable(WcR2config.SecretID);
            string secretKey = Environment.GetEnvironmentVariable(WcR2config.SecretKey);
            long durationSecond = 600; //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            this.cosXml = new CosXmlServer(config, qCloudCredentialProvider);
        }

        ListBucketModel()
        {
            InitCosXml();
        }

        public void GetService()
        {
            try
            {
                GetServiceRequest request = new GetServiceRequest();
                //执行请求
                GetServiceResult result = cosXml.GetService(request);
                //得到所有的 buckets
                List<ListAllMyBuckets.Bucket> allBuckets = result.listAllMyBuckets.buckets;
                foreach (var bucket in allBuckets)
                {
                    Console.WriteLine(bucket.name);
                }
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

        public static void ListBucketModelMain()
        {
            ListBucketModel m = new ListBucketModel();
            m.GetService();
        }
    }
}