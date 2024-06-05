using System;
using System.Linq;

namespace wip.ArtNetRecorder.Timeline
{
    [Serializable]
    public readonly struct RuntimeDmxRecordPacket
    {
        public const int dmxSize = 512;

        public readonly double time;
        public readonly int maxUniverse;

        private readonly byte[] data;
        public ReadOnlySpan<byte> Data => data;

        public RuntimeDmxRecordPacket(DmxRecordPacket packet)
        {
            var maxDmxSize = packet?.data?.Max(universeData => universeData?.data?.Length ?? 0) ?? 0;
            if (maxDmxSize != dmxSize)
                throw new NotImplementedException();

            time = packet.time / 1000;
            maxUniverse = packet.numUniverses;
            data = new byte[(maxUniverse + 1) * dmxSize];

            foreach (var universeData in packet.data)
            {
                if (universeData.data?.Length == 0)
                    continue;

                for (int channel = 0; channel < universeData.data.Length; channel++)
                {
                    data[(universeData.universe * dmxSize) + channel] = universeData.data[channel];
                }
            }
        }

        public static implicit operator RuntimeDmxRecordPacket(DmxRecordPacket packet) => new(packet);
    }
}
