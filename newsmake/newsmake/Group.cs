// Copyright (c) 2018-2019, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System.Collections.Generic;

    internal class Group
    {
        // this #.ctor is for constructing a group command.
        internal Group(string groupName)
            : this()
            => this.GroupName = groupName;

        private Group()
        {
            if (this.Commands == null)
            {
                this.Commands = new List<Command>();
            }
        }

        internal string GroupName { get; private set; }

        internal List<Command> Commands { get; private set; }

        public static Group operator +(Group group, Command cmd)
        {
            group.AddCommand(cmd);
            return group;
        }

        /*
        [Obsolete("Not used.", true)]
        public static Group operator +(Group group, Command[] cmds)
        {
            group.AddCommands(cmds);
            return group;
        }
        */

        internal Command FindCommand(string commandName)
        {
            foreach (var command in this.Commands)
            {
                if (command.Equals(commandName))
                {
                    return command;
                }
            }

            return null;
        }

        internal bool CommandEquals(string commandName)
            => this.FindCommand(commandName) != null;

        private void AddCommand(Command cmd)
        {
            // important; set the group property value to this object.
            cmd.Group = this;
            this.Commands.Add(cmd);
        }

        /*
        [Obsolete("Not used.", true)]
        private void AddCommands(Command[] cmds)
        {
            // important; set the group property values to this object.
            foreach (var command in cmds)
            {
                command.Group = this;
            }

            this.Commands.AddRange(cmds);
        }
        */
    }
}
