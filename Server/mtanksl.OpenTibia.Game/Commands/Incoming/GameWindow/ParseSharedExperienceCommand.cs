﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Commands
{
    public class ParseSharedExperienceCommand : Command
    {
        public ParseSharedExperienceCommand(Player player, bool enabled)
        {
            Player = player;

            Enabled = enabled;
        }

        public Player Player { get; set; }

        public bool Enabled { get; set; }

        public override Promise Execute(Context context)
        {
            return Promise.Run(resolve =>
            {


                resolve(context);
            } );
        }
    }
}