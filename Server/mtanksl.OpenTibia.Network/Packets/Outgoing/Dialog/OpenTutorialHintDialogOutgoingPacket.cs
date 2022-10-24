﻿using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class OpenTutorialHintDialogOutgoingPacket : IOutgoingPacket
    {
        public OpenTutorialHintDialogOutgoingPacket(byte tutorialId)
        {
            this.TutorialId = tutorialId;
        }

        public byte TutorialId { get; set; }

        public void Write(ByteArrayStreamWriter writer)
        {
            writer.Write( (byte)0xDC );

            writer.Write(TutorialId);
        }
    }
}