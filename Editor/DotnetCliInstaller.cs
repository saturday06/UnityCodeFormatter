#nullable enable

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityCodeFormatter.Editor
{
    public class DotnetCliInstaller
    {
        /// <summary>
        /// 共有HttpClient。エディタのアセンブリリロード時にDisposeする。
        /// https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient#create-an-httpclient
        /// </summary>
        private static HttpClient? s_httpClient = new();

        /// <summary>
        /// dotnet cliのインストール先のパス。
        /// </summary>
        private static string InstallBasePath =>
            Path.Combine(Application.dataPath, "..", "Library", "jp.leafytree.unitycodeformatter");

        /// <summary>
        /// dotnet cliのパス
        /// </summary>
        public static string DotnetCliPath =>
            Path.Combine(
                InstallBasePath,
                "dotnet" + (Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : "")
            );

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                // HttpClientはIDisposableだが使いまわすのが推奨されているのでstaticになっている。
                // そのためアセンブリリロード直前にDisposeする。
                s_httpClient?.Dispose();
                s_httpClient = null;
            };
        }

        public static async ValueTask<bool> Install()
        {
            HttpClient? httpClient = s_httpClient;
            if (httpClient == null)
            {
                // s_httpClientが解放済みの状態。
                // AssemblyReloadEvents.beforeAssemblyReloadが呼ばれた後に
                // このメソッドを実行するとここに到達する。
                Debug.LogError("No http client");
                return false;
            }

            string installerUrl =
                Application.platform == RuntimePlatform.WindowsEditor
                    ? "https://dot.net/v1/dotnet-install.ps1"
                    : "https://dot.net/v1/dotnet-install.sh";
            string installerPath = Path.Combine(InstallBasePath, Path.GetFileName(installerUrl));
            System.Diagnostics.ProcessStartInfo processStartInfo;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell", // TODO: 絶対パスのほうが良いか？
                    CreateNoWindow = true,
                    ArgumentList =
                    {
                        "-ExecutionPolicy",
                        "RemoteSigned",
                        "-File",
                        installerPath,
                        "-InstallDir",
                        InstallBasePath,
                        "-Channel",
                        "8.0",
                    },
                };
            }
            else
            {
                processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "/usr/bin/env", // TODO: 本当にこれが良いか? /bin/bashのが良い?
                    CreateNoWindow = true,
                    // ArgumentList = { "bash", installerPath },
                };
            }

            int progressId = Progress.Start("Install dotnet cli");
            try
            {
                // インストーラースクリプトをダウンロード
                Progress.Report(progressId, 0, "Downloading installer script");
                HttpResponseMessage response = await httpClient.GetAsync(installerUrl);
                byte[] installerScript = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(installerPath, installerScript);
                Debug.LogFormat("{0}", installerScript.Length);

                // インストーラースクリプトを実行
                Progress.Report(progressId, 0.1f, "Executing installer script");
                using var process = new System.Diagnostics.Process
                {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };

                var processTaskCompletionSource = new TaskCompletionSource<bool>();
                process.Exited += (sender, args) => processTaskCompletionSource.SetResult(true);

                if (!process.Start())
                {
                    Debug.LogError("Failed to start powershell");
                    return false;
                }

                await processTaskCompletionSource.Task;
                return true;
            }
            finally
            {
                Progress.Remove(progressId);
            }
        }
    }
}
