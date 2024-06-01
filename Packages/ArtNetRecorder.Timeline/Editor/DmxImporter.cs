using System;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace wip.ArtNetRecorder.Timeline
{
    [ScriptedImporter(1, "dmx")]
    public class DmxImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            try
            {
                var record = DmxRecordData.ReadFromFilePath(ctx.assetPath) ?? throw new Exception();
                var asset = ScriptableObject.CreateInstance<DmxRecordDataAsset>();
                DmxRecordDataAsset.SetData(ref record, ref asset);

                ctx.AddObjectToAsset("Main", asset);
                ctx.SetMainObject(asset);
            }
            catch (Exception e)
            {
                ctx.LogImportError(e.Message);
            }
        }
    }
}
