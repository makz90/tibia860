﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Commands
{
    public class ParsePassLeadershipToCommand : Command
    {
        public ParsePassLeadershipToCommand(Player player, uint creatureId)
        {
            Player = player;

            CreatureId = creatureId;
        }

        public Player Player { get; set; }

        public uint CreatureId { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {


                resolve(context);
            } );
        }
    }
}