using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace JPNSample.API
{
    public class PollerOptions
    {
        public string PollerTimerExpression { get; set; }
        public string HackerNewsBaseUrl { get; set; }
        public string RedisConnectionString { get; set; }
        public string ArticlesApiWebhookUrl { get; set; }
    }
}
