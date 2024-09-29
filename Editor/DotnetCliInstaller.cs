#nullable enable

using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCodeFormatter.Editor
{
    public class DotnetCliInstaller
    {
        private static readonly HttpClient s_httpClient = new();

        public static async Task Install()
        {
            HttpResponseMessage response = await s_httpClient.GetAsync(
                "https://dot.net/v1/dotnet-install.ps1"
            );
            string script = await response.Content.ReadAsStringAsync();
            Debug.LogFormat("{0}", script.Length);
        }
    }
}
