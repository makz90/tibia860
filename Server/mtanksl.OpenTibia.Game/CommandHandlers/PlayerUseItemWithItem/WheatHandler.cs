﻿using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class WheatHandler : CommandHandler<PlayerUseItemWithItemCommand>
    {
        private HashSet<ushort> wheats = new HashSet<ushort>() { 2694 };

        private HashSet<ushort> millstones = new HashSet<ushort>() { 1381, 1382, 1383, 1384 };

        private ushort flour = 2692;

        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerUseItemWithItemCommand command)
        {
            if (wheats.Contains(command.Item.Metadata.OpenTibiaId) && millstones.Contains(command.ToItem.Metadata.OpenTibiaId) )
            {
                return context.AddCommand(new ItemDecrementCommand(command.Item, 1) ).Then(ctx =>
                {
                    return ctx.AddCommand(new TileCreateItemCommand(command.Player.Tile, flour, 1) );
                } );
            }

            return next(context);
        }
    }
}