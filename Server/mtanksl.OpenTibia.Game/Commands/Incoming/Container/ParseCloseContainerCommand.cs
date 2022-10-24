﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ParseCloseContainerCommand : Command
    {
        public ParseCloseContainerCommand(Player player, byte containerId)
        {
            Player = player;

            ContainerId = containerId;
        }

        public Player Player { get; set; }

        public byte ContainerId { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                Container container = Player.Client.ContainerCollection.GetContainer(ContainerId);

                if (container != null)
                {
                    Player.Client.ContainerCollection.CloseContainer(ContainerId);

                    context.AddPacket(Player.Client.Connection, new CloseContainerOutgoingPacket(ContainerId) );

                    resolve(context);
                }
            } );
        }
    }
}