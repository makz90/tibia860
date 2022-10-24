﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class GreatManaPotionHandler : CommandHandler<PlayerUseItemWithCreatureCommand>
    {
        private HashSet<ushort> manaPotions = new HashSet<ushort>() { 7590 };

        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerUseItemWithCreatureCommand command)
        {
            if (manaPotions.Contains(command.Item.Metadata.OpenTibiaId) && command.ToCreature is Player player)
            {
                context.AddCommand(new ItemDecrementCommand(command.Item, 1) );

                context.AddCommand(new CombatChangeManaCommand(null, player, null, Server.Random.Next(200, 300) ) );

                context.AddCommand(new ShowMagicEffectCommand(player.Tile.Position, MagicEffectType.BlueShimmer) );

                context.AddCommand(new ShowTextCommand(player, TalkType.MonsterSay, "Aaaah...") );

                return Promise.FromResult(context);
            }

            return next(context);
        }
    }
}