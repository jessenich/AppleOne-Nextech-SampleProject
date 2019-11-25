using System;
using System.IO;
using System.Runtime.InteropServices;
using JPNSample.API.Core.Cache;
using JPNSample.API.Core.Integration.HackerNews;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

[assembly: FunctionsStartup(typeof(JPNSample.API.Startup))]

namespace JPNSample.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLogging(cfg => cfg.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddHttpContextAccessor()
                .AddOptions<PollerOptions>()
                .Configure<IConfiguration>((settings, cfg) => cfg.Bind(settings));

            builder.Services
                .AddSingleton(x => {
                    var options = x.GetService<IOptions<PollerOptions>>();
                    return ConnectionMultiplexer.Connect(options.Value.RedisConnectionString);
                })
                .AddSingleton<ICacheProvider, RedisCacheProvider>(x => {
                    var multiplexer = x.GetService<ConnectionMultiplexer>();
                    var logger = x.GetService<ILogger<RedisCacheProvider>>();
                    return new RedisCacheProvider(multiplexer, logger);
                })
                .AddSingleton<IHackerNewsRestClient, HackerNewsRestClient>(x => {
                    var options = x.GetService<IOptions<PollerOptions>>();
                    var cacheProvider = x.GetService<ICacheProvider>();
                    var logger = x.GetService<ILogger<HackerNewsRestClient>>();
                    return new HackerNewsRestClient(options.Value.HackerNewsBaseUrl, cacheProvider, logger);
                });
        }
    }
}
