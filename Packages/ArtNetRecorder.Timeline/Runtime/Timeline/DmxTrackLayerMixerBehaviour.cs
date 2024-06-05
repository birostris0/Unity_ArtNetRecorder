using System;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxTrackLayerMixerBehaviour : PlayableBehaviour
    {
        public DmxTrackAsset Track { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Span<float> layerMixedUniverses = stackalloc float[Track.ArtNetSize];
            layerMixedUniverses.Clear();
            ProcessFrame(playable, Track, ref layerMixedUniverses);
        }

        protected static void ProcessFrame(in Playable playable, in DmxTrackAsset Track, ref Span<float> layerMixedUniverses)
        {
            if (!Track)
                return;

            for (int trackIndex = 0; trackIndex < playable.GetInputCount(); trackIndex++)
            {
                var behaviour = ((ScriptPlayable<DmxTrackMixerBehaviour>)playable.GetInput(trackIndex)).GetBehaviour();
                if (behaviour.ClipCount == 0)
                    continue;

                var mixedUniverses = behaviour.MixedUniverses;
                if (mixedUniverses.Length != layerMixedUniverses.Length)
                    continue;

                for (int i = 0; i < layerMixedUniverses.Length; i++)
                {
                    layerMixedUniverses[i] = Math.Clamp(mixedUniverses[i] + layerMixedUniverses[i], 0, 255);
                }
            }
        }
    }
}
