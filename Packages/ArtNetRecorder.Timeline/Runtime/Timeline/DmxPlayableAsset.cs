using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    [System.ComponentModel.DisplayName("DMX Clip")]
    public class DmxPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] protected DmxRecordDataAsset dmx;
        public ref readonly DmxRecordDataAsset Dmx => ref dmx;

        protected RuntimeDmxRecordPacket[] packets;
        public ReadOnlySpan<RuntimeDmxRecordPacket> Packets => packets;

        public override double duration => dmx != null
            ? dmx.Duration
            : 120;

        public ClipCaps clipCaps => ClipCaps.All;

        public TimelineClip Clip { get; set; }
        public DmxTrackAsset Track { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            packets = dmx
                .Data
                .Select(packet => new RuntimeDmxRecordPacket(packet))
                .ToArray();

            var playable = ScriptPlayable<DmxPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Clip = this;

            return playable;
        }
    }
}
