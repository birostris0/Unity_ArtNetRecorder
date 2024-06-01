using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace wip.ArtNetRecorder.Timeline
{
    public class DmxRecordDataAsset : ScriptableObject
    {
        [SerializeField] private double duration;
        [SerializeField] private List<DmxRecordPacket> data;
        [SerializeField] private int maxUniverse;

        public double Duration => duration;
        public List<DmxRecordPacket> Data => data;
        public int MaxUniverse => maxUniverse;

        public static void SetData(ref DmxRecordData record, ref DmxRecordDataAsset asset)
        {
            asset.duration = record.Duration / 1000;
            asset.data = record.Data?.ToList() ?? new();
            asset.maxUniverse = record.Data?.Max(packet => packet.numUniverses) ?? 0;
        }
    }
}
