﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using System.Linq;

namespace OpenTibia.Game.Strategies
{
    public class FollowWalkStrategy : IWalkStrategy
    {
        public Tile GetNext(Context context, Tile spawn, Creature creature, Creature target)
        {
            MoveDirection[] moveDirections = context.Server.Pathfinding.GetMoveDirections(creature.Tile.Position, target.Tile.Position);

            if (moveDirections.Length != 0)
            {
                return context.Server.Map.GetTile(creature.Tile.Position.Offset(moveDirections[0] ) );                
            }

            return null;
        }
    }
}