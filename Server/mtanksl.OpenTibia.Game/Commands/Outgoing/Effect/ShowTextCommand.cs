﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ShowTextCommand : Command
    {
        public ShowTextCommand(Creature creature, TalkType talkType, string message)
        {
            Creature = creature;

            TalkType = talkType;

            Message = message;
        }

        public Creature Creature { get; set; }

        public TalkType TalkType { get; set; }

        public string Message { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {
                foreach (var observer in context.Server.GameObjects.GetPlayers() )
                {
                    if (observer.Tile.Position.CanSee(Creature.Tile.Position) )
                    {
                        context.AddPacket(observer.Client.Connection, new ShowTextOutgoingPacket(0, Creature.Name, 0, TalkType, Creature.Tile.Position, Message) );
                    }
                }

                resolve(context);
            } );
        }
    }
}