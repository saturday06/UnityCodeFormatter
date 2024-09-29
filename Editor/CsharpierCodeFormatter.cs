using UnityEngine;
using UnityEditor.Experimental;

namespace UnityCodeFormatter
{
    public class CsharpierCodeFormatter : AssetsModifiedProcessor
    {
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            throw new System.NotImplementedException();
        }
   }
}
