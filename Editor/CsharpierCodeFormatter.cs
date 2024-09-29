#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;

namespace UnityCodeFormatter.Editor
{
    public class CsharpierCodeFormatter : AssetsModifiedProcessor
    {
        [MenuItem("Tools/UnityCodeFormatter/Format")]
        public static void Format() { }

        // TODO: Libraryフォルダ内にフォルダ掘って自動インストールする
        public static readonly string DotnetFileName = "C:\\Program Files\\dotnet\\dotnet.exe";

        protected override void OnAssetsModified(
            string[] changedAssets,
            string[] addedAssets,
            string[] deletedAssets,
            AssetMoveInfo[] movedAssets
        )
        {
            // TODO: どれを無視するかてきなヤツをどうしようか
            string[] remainingAssetPaths = changedAssets
                .Concat(addedAssets)
                .Where(assetPath => assetPath.EndsWith(".cs"))
                .Distinct()
                .ToArray();
            while (remainingAssetPaths.Length > 0)
            {
                // いっぺんにすべてのソースをフォーマットしようとするとエラーになるので20こずつに分ける
                const int assetPathCountAtOneTime = 20;
                string[] formattingAssetPaths = remainingAssetPaths
                    .Take(assetPathCountAtOneTime)
                    .ToArray();
                remainingAssetPaths = remainingAssetPaths.Skip(assetPathCountAtOneTime).ToArray();
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = DotnetFileName,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ArgumentList = { "dotnet", "csharpier" },
                };
                foreach (string formattingAssetPath in formattingAssetPaths)
                {
                    processStartInfo.ArgumentList.Add(formattingAssetPath);
                }

                try
                {
                    using var process = Process.Start(processStartInfo);
                    process.WaitForExit();
                }
                catch (Win32Exception)
                {
                    // TODO: 触る環境が増えたら考える
                    throw;
                }

                foreach (string formattingAssetPath in formattingAssetPaths)
                {
                    ReportAssetChanged(formattingAssetPath);
                }
            }
        }
    }
}
