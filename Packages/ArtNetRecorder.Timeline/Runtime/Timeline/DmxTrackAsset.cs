using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    [System.ComponentModel.DisplayName("DMX Track")]
    [TrackClipType(typeof(DmxPlayableAsset))]
    public class DmxTrackAsset : TrackAsset, ILayerable
    {
        internal int maxUniverse;
        internal int maxDmxSize;

        public DmxTrackAsset RootTrack => isSubTrack
            ? (parent as DmxTrackAsset).RootTrack
            : this;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var playable = ScriptPlayable<DmxTrackMixerBehaviour>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Track = this;

            return playable;
        }

        public Playable CreateLayerMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var allClips = RootTrack
                .GetChildTracks()
                .Append(RootTrack)
                .SelectMany(track => track.GetClips());

            foreach (var clip in allClips)
            {
                if (clip?.asset is not DmxPlayableAsset clipAsset)
                    continue;

                clipAsset.Clip = clip;
                clipAsset.Track = this;

                if (clipAsset.Dmx)
                {
                    maxUniverse = Math.Max(maxUniverse, clipAsset.Dmx.MaxUniverse + 1);
                    maxDmxSize = Math.Max(maxDmxSize, clipAsset.Dmx.Data?.Max(packet => packet?.data?.Max(universe => universe?.data?.Length ?? 0) ?? 0) ?? 0);
                }
            }

            var playable = ScriptPlayable<DmxTrackLayerMixerBehaviour>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Track = this;

            return playable;
        }
    }
}
