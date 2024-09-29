#nullable enable

using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.Experimental;

namespace UnityCodeFormatter.Editor
{
    public class CsharpierCodeFormatter : AssetsModifiedProcessor
    {
        [MenuItem("Tools/UnityCodeFormatter/Format")]
        public static void Format() {
        }
            
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
#if UNITY_EDITOR_OSX
            var dotnetPath = Path.Combine(
                EditorApplication.applicationContentsPath,
                "NetCoreRuntime",
                "dotnet"
            );
            
            Debug.LogFormat("dotnet: {0} {1}", dotnetPath, File.Exists(dotnetPath));
#endif
        }
   }
}
