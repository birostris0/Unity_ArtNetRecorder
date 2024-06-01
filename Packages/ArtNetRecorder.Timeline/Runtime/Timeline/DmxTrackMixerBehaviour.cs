using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxTrackMixerBehaviour : PlayableBehaviour
    {
        internal float[][] mixedUniverses;

        public DmxTrackAsset Track { get; internal set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Track)
                return;

            mixedUniverses = new float[Track.maxUniverse][];

            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                var weight = playable.GetInputWeight(i);
                if (playable.GetPlayState() is not PlayState.Playing || weight == 0)
                    continue;

                var behaviour = ((ScriptPlayable<DmxPlayableBehaviour>)playable.GetInput(i)).GetBehaviour();
                if (behaviour.packet?.data is not List<UniverseData> data || data.Count == 0)
                    continue;

                foreach (var universeData in data)
                {
                    var dmx = universeData.data;
                    var mixedDmx = mixedUniverses[universeData.universe] switch
                    {
                        float[] mixed => mixed,
                        _ => new float[Track.maxDmxSize]
                    };

                    for (int channel = 0; channel < dmx.Length; channel++)
                    {
                        mixedDmx[channel] += Math.Clamp(dmx[channel] * weight, 0, 255);
                    }

                    mixedUniverses[universeData.universe] = mixedDmx;
                }
            }
        }
    }
}
