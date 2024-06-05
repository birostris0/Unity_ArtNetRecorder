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
    public abstract class DmxTrackAsset : TrackAsset, ILayerable
    {
        public DmxTrackAsset RootTrack => isSubTrack
            ? (parent as DmxTrackAsset).RootTrack
            : this;

        protected int maxDmxSize;
        public int MaxDmxSize => RootTrack.maxDmxSize;

        protected int maxUniverse;
        public int MaxUniverse => RootTrack.maxUniverse;

        public int ArtNetSize => MaxUniverse * MaxDmxSize;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var playable = ScriptPlayable<DmxTrackMixerBehaviour>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Track = this;

            return playable;
        }

        public Playable CreateLayerMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            SetCapacity(RootTrack, this);

            var playable = ScriptPlayable<DmxTrackLayerMixerBehaviour>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Track = this;

            return playable;
        }

        protected static void SetCapacity(DmxTrackAsset RootTrack, in DmxTrackAsset self)
        {
            var allClips = RootTrack
                .GetChildTracks()
                .Append(RootTrack)
                .SelectMany(track => track.GetClips());

            RootTrack.maxDmxSize = 0;
            RootTrack.maxUniverse = 0;
            foreach (var clip in allClips)
            {
                if (clip?.asset is not DmxPlayableAsset clipAsset)
                    continue;

                clipAsset.Clip = clip;
                clipAsset.Track = self;

                if (clipAsset.Dmx)
                {
                    RootTrack.maxDmxSize = Math.Max(RootTrack.maxDmxSize, RuntimeDmxRecordPacket.dmxSize);
                    RootTrack.maxUniverse = Math.Max(RootTrack.maxUniverse, clipAsset.Dmx.MaxUniverse + 1);
                }
            }
        }
    }
}
