﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.Commands
{
    public class CreatureUpdateHealthCommand : Command
    {
        public CreatureUpdateHealthCommand(Creature creature, ushort health, ushort maxHealth)
        {
            Creature = creature;

            Health = health;

            MaxHealth = maxHealth;
        }

        public Creature Creature { get; set; }

        public ushort Health { get; set; }

        public ushort MaxHealth { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                if (Creature.Health != Health || Creature.MaxHealth != MaxHealth)
                {
                    Creature.Health = Health;

                    Creature.MaxHealth = MaxHealth;

                    Tile fromTile = Creature.Tile;

                    foreach (var observer in context.Server.GameObjects.GetPlayers() )
                    {
                        if (observer == Creature)
                        {
                            context.AddPacket(observer.Client.Connection, new SendStatusOutgoingPacket(observer.Health, observer.MaxHealth, observer.Capacity, observer.Experience, observer.Level, observer.LevelPercent, observer.Mana, observer.MaxMana, observer.Skills.MagicLevel, observer.Skills.MagicLevelPercent, observer.Soul, observer.Stamina) );
                        }
                    
                        if (observer.Tile.Position.CanSee(fromTile.Position) )
                        {
                            context.AddPacket(observer.Client.Connection, new SetHealthOutgoingPacket(Creature.Id, (byte)Math.Ceiling(100.0 * Creature.Health / Creature.MaxHealth) ) );
                        }
                    }
                }

                resolve(context);
            } );
        }
    }
}