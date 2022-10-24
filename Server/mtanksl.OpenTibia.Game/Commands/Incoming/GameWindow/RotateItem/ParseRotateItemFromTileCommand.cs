﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;

namespace OpenTibia.Game.Commands
{
    public class ParseRotateItemFromTileCommand : ParseRotateItemCommand
    {
        public ParseRotateItemFromTileCommand(Player player, Position fromPosition, byte fromIndex, ushort itemId) : base(player)
        {
            FromPosition = fromPosition;

            FromIndex = fromIndex;

            ItemId = itemId;
        }

        public Position FromPosition { get; set; }

        public byte FromIndex { get; set; }

        public ushort ItemId { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                Tile fromTile = context.Server.Map.GetTile(FromPosition);

                if (fromTile != null)
                {
                    if (Player.Tile.Position.CanSee(fromTile.Position) )
                    {
                        Item fromItem = fromTile.GetContent(FromIndex) as Item;

                        if (fromItem != null && fromItem.Metadata.TibiaId == ItemId)
                        {
                            if ( IsRotatable(context, fromItem) )
                            {
                                context.AddCommand(new PlayerRotateItemCommand(Player, fromItem) ).Then(ctx =>
                                {
                                    resolve(ctx);
                                } );
                            }
                        }
                    }
                }
            } );            
        }
    }
}