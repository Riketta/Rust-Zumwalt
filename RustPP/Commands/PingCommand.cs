﻿namespace RustPP.Commands
{
    using Fougerite;
    using System;

    public class PingCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "Ping: " + Arguments.argUser.networkPlayer.lastPing);
        }
    }
}