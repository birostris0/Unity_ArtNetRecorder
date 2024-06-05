using System;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxTrackMixerBehaviour : PlayableBehaviour
    {
        protected int clipCount;
        public int ClipCount => clipCount;

        protected float[] mixedUniverses;
        public ReadOnlySpan<float> MixedUniverses => mixedUniverses;

        public DmxTrackAsset Track { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Span<float> mixedUniverses = stackalloc float[Track.ArtNetSize];
            mixedUniverses.Clear();
            ProcessFrame(playable, Track, ref clipCount, ref mixedUniverses);
            this.mixedUniverses = mixedUniverses.ToArray();
        }

        protected static void ProcessFrame(in Playable playable, in DmxTrackAsset Track, ref int clipCount, ref Span<float> mixedUniverses)
        {
            if (!Track)
                return;

            clipCount = playable.GetInputCount();
            if (clipCount == 0)
                return;

            for (int clipIndex = 0; clipIndex < clipCount; clipIndex++)
            {
                var clipWeight = playable.GetInputWeight(clipIndex);
                if (playable.GetPlayState() is not PlayState.Playing || clipWeight == 0)
                    continue;

                var behaviour = ((ScriptPlayable<DmxPlayableBehaviour>)playable.GetInput(clipIndex)).GetBehaviour();
                var packetData = behaviour.Packet.Data;
                if (packetData.IsEmpty)
                    continue;

                for (int i = 0; i < packetData.Length; i++)
                {
                    mixedUniverses[i] += packetData[i] * clipWeight;
                }
            }
        }
    }
}
