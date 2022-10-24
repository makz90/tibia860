﻿using OpenTibia.Game.Commands;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public abstract class CommandHandler : ICommandHandler
    {
        public abstract Promise Handle(Context context, Func<Context, Promise> next, Command command);
    }

    public abstract class CommandHandler<T> : CommandHandler where T : Command
    {
        public override Promise Handle(Context context, Func<Context, Promise> next, Command command)
        {
            return Handle(context, next, (T)command);
        }

        public abstract Promise Handle(Context context, Func<Context, Promise> next, T command);
    }

    public class InlineCommandHandler<T> : CommandHandler<T> where T : Command
    {
        private Func<Context, Func<Context, Promise>, T, Promise> handle;

        public InlineCommandHandler(Func<Context, Func<Context, Promise>, T, Promise> handle)
        {
            this.handle = handle;
        }

        public override Promise Handle(Context context, Func<Context, Promise> next, T command)
        {
            return handle(context, next, command);
        }
    }
}