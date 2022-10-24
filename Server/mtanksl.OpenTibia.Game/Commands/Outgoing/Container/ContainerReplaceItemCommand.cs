﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ContainerReplaceItemCommand : CommandResult<byte>
    {
        public ContainerReplaceItemCommand(Container container, Item fromItem, Item toItem)
        {
            Container = container;

            FromItem = fromItem;

            ToItem = toItem;
        }

        public Container Container { get; set; }

        public Item FromItem { get; set; }

        public Item ToItem { get; set; }
        
        public override PromiseResult<byte> Execute(Context context)
        {
            return PromiseResult<byte>.Run(resolve =>
            {
                byte index = Container.GetIndex(FromItem);

                Container.ReplaceContent(index, ToItem);

                foreach (var observer in Container.GetPlayers() )
                {
                    foreach (var pair in observer.Client.ContainerCollection.GetIndexedContainers() )
                    {
                        if (pair.Value == Container)
                        {
                            context.AddPacket(observer.Client.Connection, new ContainerUpdateOutgoingPacket(pair.Key, index, ToItem) );
                        }
                    }
                }

                resolve(context, index);
            } );
        }
    }
}