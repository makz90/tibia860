﻿using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public interface IOutgoingPacket
    {
        void Write(ByteArrayStreamWriter writer);
    }
}