﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;

namespace OpenTibia.Game.Extensions
{
    public static class ItemExtensions
    {
        public static Promise DecayDesctroy(this Item item, int executeInMilliseconds)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemDecayDestroyCommand(item, executeInMilliseconds) );
        }

        public static PromiseResult<Item> DecayTransform(this Item item, int executeInMilliseconds, ushort openTibiaId, byte count)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemDecayTransformCommand(item, executeInMilliseconds, openTibiaId, count) );
        }

        public static Promise Decrement(this Item item, byte count)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemDecrementCommand(item, count) );
        }

        public static Promise Destroy(this Item item)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemDestroyCommand(item) );
        }

        public static PromiseResult<Item> Transform(this Item item, ushort openTibiaId, byte count)
        {
            Context context = Context.Current;
            
            return context.AddCommand(new ItemTransformCommand(item, openTibiaId, count) );
        }

        public static Promise UpdateParentToTile(this Item item, Tile toTile)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemUpdateParentToTileCommand(item, toTile) );
        }

        public static Promise UpdateParentToInventory(this Item item, Inventory toInventory, byte slot)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemUpdateParentToInventoryCommand(item, toInventory, slot) );
        }

        public static Promise UpdateParentToContainer(this Item item, Container toContiner)
        {
            Context context = Context.Current;

            return context.AddCommand(new ItemUpdateParentToContainerCommand(item, toContiner) );
        }
    }
}