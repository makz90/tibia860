﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Commands
{
    public class ItemUpdateParentToContainerCommand : Command
    {
        public ItemUpdateParentToContainerCommand(Item item, Container toContainer)
        {
            Item = item;

            ToContainer = toContainer;
        }

        public Item Item { get; set; }
        
        public Container ToContainer { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                switch (Item.Parent)
                {
                    case Tile fromTile:

                        context.AddCommand(new TileRemoveItemCommand(fromTile, Item) ).Then( (ctx, index) =>
                        {
                            return ctx.AddCommand(new ContainerAddItemCommand(ToContainer, Item) );

                        } ).Then( (ctx, index) =>
                        {
                            resolve(ctx);
                        } );

                        break;

                    case Inventory fromInventory:

                        context.AddCommand(new InventoryRemoveItemCommand(fromInventory, Item) ).Then( (ctx, index) =>
                        {
                            return ctx.AddCommand(new ContainerAddItemCommand(ToContainer, Item) );

                        } ).Then( (ctx, index) =>
                        {
                            resolve(ctx);
                        } );

                        break;

                    case Container fromContainer:

                        context.AddCommand(new ContainerRemoveItemCommand(fromContainer, Item) ).Then( (ctx, index) =>
                        {
                            return ctx.AddCommand(new ContainerAddItemCommand(ToContainer, Item) );

                        } ).Then( (ctx, index) =>
                        {
                            resolve(ctx);
                        } );

                        break;
                }
            } );
        }
    }
}