using System;

using Flurl;

namespace JPNSample.API.Core.Integration.Extensions.Flurl
{
    public static class FlurlFluentHttpExtensions
    {
        public static Url ForceHttps(this string url)
        {
            // Using fail-first patterns, check valid was supplied prior to any futher processing 
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            var flurlUrl = new Url(new Uri(url));
            if (!flurlUrl.ToString(true).Contains("https"))
                url = url.Replace("http", "https");

            flurlUrl.ThrowIfNotValidUrl();

            return url;
        }

        internal static Url ThrowIfNotValidUrl(this Url flurlUrl)
        {
            // Using fail-first patterns, check valid was supplied prior to any futher processing
            if (!flurlUrl?.IsValid() ?? false)
                throw new FormatException($"Malformed uri detected: {flurlUrl.ToString(true)}");

            return flurlUrl;
        }

        public static Url ForceHttps(this Url url) => url.ToString().ForceHttps();
    }


}
