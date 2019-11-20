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

        internal Command(string commandSwitch, string commandDescr, Action<string[]> commandCode)
        {
            this.CommandSwitch = commandSwitch;
            this.CommandDescription = commandDescr;
            this.commandCode = commandCode;
        }

        private Command()
        {
        }

        internal Group Group { get; set; }

        internal string CommandDescription { get; private set; }

        internal string CommandSwitch { get; private set; }

        internal bool Equals(string value)
            => this.CommandSwitch != null ? this.CommandSwitch.Equals(value) : false;

        internal void InvokeCommand(string[] commands)
        {
            if (this.commandCode != null)
            {
                this.commandCode.Invoke(commands);
            }
            else
            {
                throw new InvalidOperationException("Calling InvokeCommand on a object with no code to invoke.");
            }
        }
    }
}
