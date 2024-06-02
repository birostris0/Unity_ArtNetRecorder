using System;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxTrackLayerMixerBehaviour : PlayableBehaviour
    {
        public DmxTrackAsset Track { get; internal set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Track)
                return;

            Span<float> layerMixedUniverses = stackalloc float[Track.ArtNetSize];
            layerMixedUniverses.Clear();

            for (int trackIndex = 0; trackIndex < playable.GetInputCount(); trackIndex++)
            {
                var behaviour = ((ScriptPlayable<DmxTrackMixerBehaviour>)playable.GetInput(trackIndex)).GetBehaviour();
                var clipCount = behaviour.clipCount;
                if (clipCount == 0)
                    continue;

                if (behaviour.trackUniverses is not UniverseData[] trackUniverses || trackUniverses.Length != Track.MaxUniverse * clipCount)
                    continue;

                if (behaviour.clipWeights is not float[] clipWeights || clipWeights.Length != clipCount)
                    continue;

                for (int universeIndex = 0; universeIndex < Track.MaxUniverse; universeIndex++)
                {
                    var universeOffset = universeIndex * Track.MaxDmxSize;
                    var layerMixedDmx = layerMixedUniverses[universeOffset..(universeOffset + Track.MaxDmxSize)];

                    for (int channel = 0; channel < Track.MaxDmxSize; channel++)
                    {
                        float mixedChannelData = 0;
                        for (int clipIndex = 0; clipIndex < clipCount; clipIndex++)
                        {
                            if (trackUniverses[(universeIndex * clipCount) + clipIndex] is not UniverseData universeData)
                                continue;

                            mixedChannelData += universeData.data[channel] * clipWeights[clipIndex];
                        }

                        layerMixedDmx[channel] = Math.Clamp(mixedChannelData + layerMixedDmx[channel], 0, 255);
                    }
                }
            }
        }
    }
}
