using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public class DmxPlayableBehaviour : PlayableBehaviour
    {
        internal DmxRecordPacket packet;

        public DmxPlayableAsset Clip { get; internal set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var time = playable.GetTime() * 1000;

            int sequence = 0;
            packet = null;
            if (time < 0 || !Clip || !Clip.Dmx || Clip.Dmx.Data is not List<DmxRecordPacket> data || data.Count == 0)
                return;

            while (sequence < data.Count)
            {
                packet = data[sequence];
                if (packet is null)
                    break;

                if (packet.time > time)
                    break;

                sequence++;
            }
        }
    }
}
