﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTibia.Game.CommandHandlers
{
    public class ThrowAwayContainerCloseHandler : CommandHandler<PlayerMoveItemCommand>
    {
        public override Promise Handle(Context context, Func<Context, Promise> next, PlayerMoveItemCommand command)
        {
            if (command.Item is Container container)
            {
                HashSet<Player> isNextFrom = new HashSet<Player>();

                switch (container.Parent)
                {
                    case Tile fromTile:

                        foreach (var observer in context.Server.GameObjects.GetPlayers() )
                        {
                            if (observer.Tile.Position.IsNextTo(fromTile.Position) )
                            {
                                isNextFrom.Add(observer);
                            }                            
                        }

                        break;

                    case Inventory fromInventory:

                        isNextFrom.Add(fromInventory.Player);

                        break;

                    case Container fromContainer:

                        switch (fromContainer.Root() )
                        {
                            case null:

                                isNextFrom.Add(command.Player);

                                break;

                            case Tile fromTile:

                                foreach (var observer in context.Server.GameObjects.GetPlayers() )
                                {
                                    if (observer.Tile.Position.IsNextTo(fromTile.Position) )
                                    {
                                        isNextFrom.Add(observer);
                                    }
                                }

                                break;

                            case Inventory fromInventory:

                                isNextFrom.Add(fromInventory.Player);

                                break;
                        }

                        break;
                }

                return next(context).Then(ctx =>
                {
                    HashSet<Player> isNextTo = new HashSet<Player>();

                    switch (command.Item.Parent)
                    {
                        case Tile toTile:

                            foreach (var observer in context.Server.GameObjects.GetPlayers() )
                            {
                                if (observer.Tile.Position.IsNextTo(toTile.Position) )
                                {
                                    isNextTo.Add(observer);
                                }
                            }

                            break;

                        case Inventory toInventory:

                            isNextTo.Add(toInventory.Player);

                            break;

                        case Container toContainer:

                            switch (toContainer.Root() )
                            {
                                case null:

                                    isNextTo.Add(command.Player);

                                    break;

                                case Tile toTile:

                                    foreach (var observer in context.Server.GameObjects.GetPlayers() )
                                    {
                                        if (observer.Tile.Position.IsNextTo(toTile.Position) )
                                        {
                                            isNextTo.Add(observer);
                                        }
                                    }

                                    break;

                                case Inventory toInventory:

                                    isNextTo.Add(toInventory.Player);

                                    break;
                            }

                            break;
                    }

                    foreach (var observer in isNextFrom.Except(isNextTo) )
                    {
                        foreach (var pair in observer.Client.ContainerCollection.GetIndexedContainers() )
                        {
                            if (pair.Value.IsContentOf(container) )
                            {
                                observer.Client.ContainerCollection.CloseContainer(pair.Key);

                                context.AddPacket(observer.Client.Connection, new CloseContainerOutgoingPacket(pair.Key) );
                            }
                        }
                    }

                    foreach (var observer in isNextFrom.Intersect(isNextTo) )
                    {
                        foreach (var pair in observer.Client.ContainerCollection.GetIndexedContainers() )
                        {
                            if (pair.Value == container)
                            {
                                var items = new List<Item>();

                                foreach (var item in container.GetItems() )
                                {
                                    items.Add(item);
                                }

                                context.AddPacket(observer.Client.Connection, new OpenContainerOutgoingPacket(pair.Key, container.Metadata.TibiaId, container.Metadata.Name, container.Metadata.Capacity, container.Parent is Container, items) );
                            }                           
                        }
                    }
                } );
            }

            return next(context);
        }
    }
}