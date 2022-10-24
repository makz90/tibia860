﻿using OpenTibia.Common.Structures;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ShowProjectileCommand : Command
    {
        public ShowProjectileCommand(Position fromPosition, Position toPosition, ProjectileType projectileType)
        {
            FromPosition = fromPosition;

            ToPosition = toPosition;

            ProjectileType = projectileType;
        }

        public Position FromPosition { get; set; }

        public Position ToPosition { get; set; }

        public ProjectileType ProjectileType { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                foreach (var observer in context.Server.GameObjects.GetPlayers() )
                {
                    if (observer.Tile.Position.CanSee(FromPosition) || observer.Tile.Position.CanSee(ToPosition) )
                    {
                        context.AddPacket(observer.Client.Connection, new ShowProjectileOutgoingPacket(FromPosition, ToPosition, ProjectileType) );
                    }
                }

                resolve(context);
            } );
        }
    }
}