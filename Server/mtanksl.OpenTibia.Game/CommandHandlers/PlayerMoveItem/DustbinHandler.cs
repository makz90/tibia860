﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class DustbinHandler : CommandHandler<PlayerMoveItemCommand>
    {
        private HashSet<ushort> dustbins = new HashSet<ushort>() { 1777 };

        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerMoveItemCommand command)
        {
            if (command.ToContainer is Tile toTile && toTile.TopItem != null && dustbins.Contains(toTile.TopItem.Metadata.OpenTibiaId) )
            {
                return context.AddCommand(new ItemDecrementCommand(command.Item, command.Count) );
            }

            return next(context);
        }
    }
}