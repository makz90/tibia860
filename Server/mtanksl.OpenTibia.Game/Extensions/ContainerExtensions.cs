﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;

namespace OpenTibia.Game.Extensions
{
    public static class ContainerExtensions
    {
        public static PromiseResult<byte> AddItem(this Container container, Item item)
        {
            Context context = Context.Current;

            return context.AddCommand(new ContainerAddItemCommand(container, item) );
        }

        public static PromiseResult<Item> CreateItem(this Container container, ushort openTibiaId, byte count)
        {
            Context context = Context.Current;

            return context.AddCommand(new ContainerCreateItemCommand(container, openTibiaId, count) );
        }

        public static PromiseResult<byte> RemoveItem(this Container container, Item item)
        {
            Context context = Context.Current;

            return context.AddCommand(new ContainerRemoveItemCommand(container, item) );
        }

        public static PromiseResult<byte> ReplaceItem(this Container container, Item fromItem, Item toItem)
        {
            Context context = Context.Current;

            return context.AddCommand(new ContainerReplaceItemCommand(container, fromItem, toItem) );
        }
    }
}