﻿using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class CloseTradeOutgoingPacket : IOutgoingPacket
    {
        public void Write(ByteArrayStreamWriter writer)
        {
            writer.Write( (byte)0x7F );
        }
    }
}