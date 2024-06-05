using System;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxPlayableBehaviour : PlayableBehaviour
    {
        protected RuntimeDmxRecordPacket packet;
        public ref readonly RuntimeDmxRecordPacket Packet => ref packet;

        public DmxPlayableAsset Clip { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ProcessFrame(playable, Clip, ref packet);
        }

        protected static void ProcessFrame(in Playable playable, in DmxPlayableAsset Clip, ref RuntimeDmxRecordPacket packet)
        {
            var time = playable.GetTime();

            packet = default;
            if (time < 0 || !Clip)
                return;

            var packets = Clip.Packets;
            for (int sequence = 0; sequence < packets.Length; sequence++)
            {
                packet = packets[sequence];
                if (packet.time > time)
                    break;
            }
        }
    }
}
