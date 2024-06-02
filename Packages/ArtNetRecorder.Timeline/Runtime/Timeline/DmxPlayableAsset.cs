using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    [System.ComponentModel.DisplayName("DMX Clip")]
    public class DmxPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private DmxRecordDataAsset dmx;

        public DmxRecordDataAsset Dmx => dmx;

        public override double duration => dmx != null
            ? dmx.Duration
            : 120;

        public ClipCaps clipCaps => ClipCaps.All;

        public TimelineClip Clip { get; internal set; }
        public DmxTrackAsset Track { get; internal set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DmxPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Clip = this;

            return playable;
        }
    }
}
