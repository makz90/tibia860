﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class PlayerUseItemWithItemCommand : Command
    {
        public PlayerUseItemWithItemCommand(Player player, Item item, Item toItem)
        {
            Player = player;

            Item = item;

            ToItem = toItem;
        }

        public Player Player { get; set; }

        public Item Item { get; set; }

        public Item ToItem { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                context.AddPacket(Player.Client.Connection, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouCanNotUseThisObject) );
            } );
        }
    }
}