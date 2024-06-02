using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxTrackMixerBehaviour : PlayableBehaviour
    {
        internal int clipCount;
        internal UniverseData[] trackUniverses;
        internal float[] clipWeights;

        public DmxTrackAsset Track { get; internal set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Track)
                return;

            clipCount = playable.GetInputCount();
            if (clipCount == 0)
                return;

            trackUniverses = new UniverseData[Track.MaxUniverse * clipCount];
            clipWeights = new float[clipCount];

            for (int clipIndex = 0; clipIndex < clipCount; clipIndex++)
            {
                var clipWeight = playable.GetInputWeight(clipIndex);
                if (playable.GetPlayState() is not PlayState.Playing || clipWeight == 0)
                    continue;

                var behaviour = ((ScriptPlayable<DmxPlayableBehaviour>)playable.GetInput(clipIndex)).GetBehaviour();
                if (behaviour.packet?.data is not List<UniverseData> data || data.Count == 0)
                    continue;

                foreach (var universeData in data)
                {
                    if (universeData.data is not byte[] dmx || dmx.Length == 0)
                        continue;

                    trackUniverses[(universeData.universe * clipCount) + clipIndex] = universeData;
                }

                clipWeights[clipIndex] = clipWeight;
            }
        }
    }
}
