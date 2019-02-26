// Copyright (c) 2018-2019, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System;

    internal class Command
    {
        private readonly Action<string[]> commandCode;

        internal Command(string commandSwitch, string commandGroup, string commandDescr, Action<string[]> commandCode)
        {
            this.CommandSwitch = commandSwitch;
            this.CommandGroup = commandGroup;
            this.CommandDescription = commandDescr;
            this.commandCode = commandCode;
        }

        internal string CommandGroup { get; private set; }

        internal string CommandDescription { get; private set; }

        internal string CommandSwitch { get; private set; }

        internal bool Equals(string value)
            => this.CommandSwitch.Equals(value);

        internal void InvokeCommand(string[] commands)
            => this.commandCode.Invoke(commands);
    }
}
