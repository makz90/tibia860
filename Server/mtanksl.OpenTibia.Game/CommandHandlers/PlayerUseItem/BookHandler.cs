﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class BookHandler : CommandHandler<PlayerUseItemCommand>
    {
        private HashSet<ushort> books = new HashSet<ushort>() { 1955 };

        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerUseItemCommand command)
        {
            if (books.Contains(command.Item.Metadata.OpenTibiaId) )
            {
                var readableItem = (ReadableItem)command.Item;

                context.AddPacket(command.Player.Client.Connection, new OpenTextDialogOutgoingPacket(0, command.Item.Metadata.TibiaId, 255, readableItem.Text, readableItem.Author, readableItem.Date) );

                return Promise.FromResult(context);
            }

            return next(context);
        }
    }
}