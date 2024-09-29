#nullable enable

using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityCodeFormatter.Editor
{
    public class DotnetCliInstaller
    {
        // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient#create-an-httpclient
        private static HttpClient? s_httpClient = new();

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            // HttpClientはIDisposableだが使いまわすのが推奨されているのでstaticになっている。
            // そのためアセンブリリロード直前にDisposeする。
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                s_httpClient?.Dispose();
                s_httpClient = null;
            };
        }

        public static async Task Install()
        {
            HttpClient? httpClient = s_httpClient;
            if (httpClient == null)
            {
                Debug.LogError("No http client");
                return;
            }
            HttpResponseMessage response = await httpClient.GetAsync(
                "https://dot.net/v1/dotnet-install.ps1"
            );
            byte[] script = await response.Content.ReadAsByteArrayAsync();
            Debug.LogFormat("{0}", script.Length);
        }
    }
}
