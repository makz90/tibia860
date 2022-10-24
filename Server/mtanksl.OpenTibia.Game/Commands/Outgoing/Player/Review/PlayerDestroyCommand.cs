﻿using OpenTibia.Common.Objects;
using System.Linq;

namespace OpenTibia.Game.Commands
{
    public class PlayerDestroyCommand : Command
    {
        public PlayerDestroyCommand(Player player)
        {
            Player = player;
        }

        public Player Player { get; set; }

        public override Promise Execute(Context context)
        {
            Tile tile = Player.Tile;

            return context.AddCommand(new TileRemoveCreatureCommand(tile, Player) ).Then( (ctx, index) =>
            {
                #region Save player to database

                var databasePlayer = context.DatabaseContext.PlayerRepository.GetPlayer(Player.DatabasePlayerId);

                databasePlayer.Direction = (int)Player.Direction;

                databasePlayer.OutfitItemId = Player.Outfit.ItemId;

                databasePlayer.OutfitId = Player.Outfit.Id;

                databasePlayer.OutfitHead = Player.Outfit.Head;

                databasePlayer.OutfitBody = Player.Outfit.Body;

                databasePlayer.OutfitLegs = Player.Outfit.Legs;

                databasePlayer.OutfitFeet = Player.Outfit.Feet;

                databasePlayer.OutfitAddon = (int)Player.Outfit.Addon;

                databasePlayer.CoordinateX = tile.Position.X;

                databasePlayer.CoordinateY = tile.Position.Y;

                databasePlayer.CoordinateZ = tile.Position.Z;

                context.DatabaseContext.PlayerRepository.UpdatePlayer(databasePlayer);

                #endregion

                #region Save player items to database

                foreach (var databasePlayerItem in databasePlayer.PlayerItems.ToList() )
                {
                    context.DatabaseContext.PlayerRepository.RemovePlayerItem(databasePlayerItem);
                }

                int sequenceId = 101;

                foreach (var pair in Player.Inventory.GetIndexedContents() )
                {
                    AddPlayerItems(context, ref sequenceId, pair.Key, (Item)pair.Value);
                }

                #endregion

                #region Save player depot items to database

                foreach (var databasePlayerDepotItem in databasePlayer.PlayerDepotItems.ToList() )
                {
                    context.DatabaseContext.PlayerRepository.RemovePlayerDepotItem(databasePlayerDepotItem);
                }

                sequenceId = 101;

                foreach (var pair in ctx.Server.Lockers.GetIndexedLockers(Player.DatabasePlayerId) )
                {
                    AddPlayerDepotItems(context, ref sequenceId, pair.Key, pair.Value);
                }

                context.DatabaseContext.Commit();

                #endregion

                #region Destroy

                foreach (var item in Player.Inventory.GetItems() )
                {
                    ctx.AddCommand(new ItemDestroyCommand(item) );
                }

                foreach (var pair in ctx.Server.Lockers.GetIndexedLockers(Player.DatabasePlayerId).ToList() )
                {
                    ctx.Server.Lockers.RemoveLocker(Player.DatabasePlayerId, pair.Key);

                    ctx.AddCommand(new ItemDestroyCommand(pair.Value) );
                }

                foreach (var pair in Player.Client.ContainerCollection.GetIndexedContainers() )
                {
                    Player.Client.ContainerCollection.CloseContainer(pair.Key);
                }

                foreach (var pair in Player.Client.WindowCollection.GetIndexedWindows() )
                {
                    Player.Client.WindowCollection.CloseWindow(pair.Key);
                }

                /*
                foreach (var channel in ctx.Server.Channels.GetChannels().ToList() )
                {
                    if (channel.ContainsPlayer(Player) )
                    {
                        channel.RemovePlayer(Player);
                    }

                    if (channel is PrivateChannel privateChannel)
                    {
                        if (privateChannel.Owner == Player)
                        {
                            privateChannel.Owner = null;
                        }

                        if (privateChannel.ContainsInvitation(Player) )
                        {
                            privateChannel.RemoveInvitation(Player);
                        }
                    }
                }
                */

                /*
                foreach (var ruleViolation in ctx.Server.RuleViolations.GetRuleViolations().ToList() )
                {
                    if (ruleViolation.Reporter == Player)
                    {
                        ruleViolation.Reporter = null;
                    }

                    if (ruleViolation.Assignee == Player)
                    {
                        ruleViolation.Assignee = null;
                    }
                }
                */

                ctx.Server.PlayerFactory.Destroy(Player);

                #endregion
            } );
        }

        private void AddPlayerItems(Context context, ref int sequence, int index, Item item)
        {
            var playerItem = new Data.Models.PlayerItem()
            {
                PlayerId = Player.DatabasePlayerId,

                SequenceId = sequence++,

                ParentId = index,

                OpenTibiaId = item.Metadata.OpenTibiaId,

                Count = item is StackableItem stackableItem ? stackableItem.Count :

                        item is FluidItem fluidItem ? (int)fluidItem.FluidType :

                        item is SplashItem splashItem ? (int)splashItem.FluidType : 1,
            };

            context.DatabaseContext.PlayerRepository.AddPlayerItem(playerItem);

            if (item is Container container)
            {
                foreach (var i in container.GetItems().Reverse() )
                {
                    AddPlayerItems(context, ref sequence, playerItem.SequenceId, i);
                }
            }
        }

        private void AddPlayerDepotItems(Context context, ref int sequence, int index, Item item)
        {
            var playerItem = new Data.Models.PlayerDepotItem()
            {
                PlayerId = Player.DatabasePlayerId,

                SequenceId = sequence++,

                ParentId = index,

                OpenTibiaId = item.Metadata.OpenTibiaId,

                Count = item is StackableItem stackableItem ? stackableItem.Count :

                        item is FluidItem fluidItem ? (int)fluidItem.FluidType :

                        item is SplashItem splashItem ? (int)splashItem.FluidType : 1,
            };

            context.DatabaseContext.PlayerRepository.AddPlayerDepotItem(playerItem);

            if (item is Container container)
            {
                foreach (var i in container.GetItems().Reverse() )
                {
                    AddPlayerDepotItems(context, ref sequence, playerItem.SequenceId, i);
                }
            }
        }
    }
}