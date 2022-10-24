﻿using OpenTibia.Common.Structures;
using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class SetOutfitOutgoingPacket : IOutgoingPacket
    {
        public SetOutfitOutgoingPacket(uint creatureId, Outfit outfit)
        {
            this.CreatureId = creatureId;

            this.Outfit = outfit;
        }

        public uint CreatureId { get; set; }

        public Outfit Outfit { get; set; }
        
        public void Write(ByteArrayStreamWriter writer)
        {
            writer.Write( (byte)0x8E );

            writer.Write(CreatureId);

            writer.Write(Outfit);
        }
    }
}