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

            Span<float> layerMixedUniverses = stackalloc float[Track.maxUniverse * Track.maxDmxSize];
            layerMixedUniverses.Clear();

            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                var behaviour = ((ScriptPlayable<DmxTrackMixerBehaviour>)playable.GetInput(i)).GetBehaviour();
                if (behaviour.mixedUniverses is not float[][] mixedUniverses || mixedUniverses.Length == 0)
                    continue;

                for (int universe = 0; universe < mixedUniverses.Length; universe++)
                {
                    var mixedDmx = mixedUniverses[universe];
                    if (mixedDmx is null || mixedDmx.Length == 0)
                        continue;

                    var offset = universe * Track.maxDmxSize;
                    var layerMixedDmx = layerMixedUniverses[offset..(offset + Track.maxDmxSize)];

                    for (int channel = 0; channel < mixedDmx.Length; channel++)
                    {
                        layerMixedDmx[channel] = Math.Clamp(mixedDmx[channel] + layerMixedDmx[channel], 0, 255);
                    }
                }
            }
        }
    }
}
