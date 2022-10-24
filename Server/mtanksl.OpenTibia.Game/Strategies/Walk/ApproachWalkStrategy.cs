﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using System.Collections.Generic;
using System.Linq;

namespace OpenTibia.Game.Strategies
{
    public class ApproachWalkStrategy : IWalkStrategy
    {
        public Tile GetNext(Context context, Tile spawn, Creature creature, Creature target)
        {
            int deltaY = creature.Tile.Position.Y - target.Tile.Position.Y;

            int deltaX = creature.Tile.Position.X - target.Tile.Position.X;

            HashSet<Direction> directions = new HashSet<Direction>();

            if (deltaY < 0)
            {
                directions.Add(Direction.South);
            }
            else if (deltaY > 0)
            {
                directions.Add(Direction.North);
            }

            if (deltaX < 0)
            {
                directions.Add(Direction.East);
            }
            else if (deltaX > 0)
            {
                directions.Add(Direction.West);
            }

            if (directions.Count > 0)
            {
                foreach (var direction in directions.ToArray().Shuffle() )
                {
                    Tile toTile = context.Server.Map.GetTile(creature.Tile.Position.Offset(direction) );

                    if (toTile == null || toTile.GetItems().Any(i => i.Metadata.Flags.Is(ItemMetadataFlags.NotWalkable) || i.Metadata.Flags.Is(ItemMetadataFlags.BlockPathFinding) ) || toTile.GetCreatures().Any(c => c.Block) )
                    {

                    }
                    else
                    {
                        return toTile;
                    }
                }
            }

            foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West }.Shuffle() )
            {
                Tile toTile = context.Server.Map.GetTile(creature.Tile.Position.Offset(direction) );

                if (toTile == null || toTile.GetItems().Any(i => i.Metadata.Flags.Is(ItemMetadataFlags.NotWalkable) || i.Metadata.Flags.Is(ItemMetadataFlags.BlockPathFinding) ) || toTile.GetCreatures().Any(c => c.Block) )
                {

                }
                else
                {
                    return toTile;
                }
            }

            return null;
        }
    }
}