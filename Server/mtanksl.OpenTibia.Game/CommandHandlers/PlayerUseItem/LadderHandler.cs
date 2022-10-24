﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class LadderHandler : CommandHandler<PlayerUseItemCommand>
    {
        private HashSet<ushort> ladders = new HashSet<ushort>() { 1386, 3678, 5543 };

        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerUseItemCommand command)
        {
            if (ladders.Contains(command.Item.Metadata.OpenTibiaId) )
            {
                return context.AddCommand(new CreatureUpdateParentCommand(command.Player, context.Server.Map.GetTile( ( (Tile)command.Item.Parent ).Position.Offset(0, 1, -1) ), Direction.South) );
            }

            return next(context);
        }
    }
}