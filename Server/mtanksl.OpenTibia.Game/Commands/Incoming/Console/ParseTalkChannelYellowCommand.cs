﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ParseTalkChannelYellowCommand : Command
    {
        public ParseTalkChannelYellowCommand(Player player, ushort channelId, string message)
        {
            Player = player;

            ChannelId = channelId;

            Message = message;
        }

        public Player Player { get; set; }

        public ushort ChannelId { get; set; }

        public string Message { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                Channel channel = context.Server.Channels.GetChannel(ChannelId);

                if (channel != null)
                {
                    if (channel.ContainsPlayer(Player) )
                    {                                                           
                        foreach (var observer in channel.GetPlayers() )
                        {
                            context.AddPacket(observer.Client.Connection, new ShowTextOutgoingPacket(0, Player.Name, Player.Level, TalkType.ChannelYellow, channel.Id, Message) );
                        }

                        resolve(context);
                    }
                }                
            } );
        }
    }
}