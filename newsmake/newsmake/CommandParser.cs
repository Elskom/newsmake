// Copyright (c) 2018-2019, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class CommandParser
    {
        private readonly string[] args;

        public CommandParser(string[] args)
        {
            this.args = args;
            this.Groups = new List<Group>();
        }

        public int Length
            => this.args.Length;

        public bool HasDocs { get; set; } = false;

        public string DocsUrl { get; set; } = string.Empty;

        private List<Group> Groups { get; set; }

        /*
        [Obsolete("Do not use this version of operator +; use the Dictionary<string, Command> one as it allows you to save code.", true)]
        public static CommandParser operator +(CommandParser parser, Group group)
        {
            // if "Global" command group does not exist.
            if (parser.GetGroupIndex("Global") == -1)
            {
                parser.AddGroup(new Group("Global"));
            }

            parser.AddGroup(group);
            return parser;
        }

        [Obsolete("Do not use this version of operator +; use the Dictionary<string, Command> one as it allows you to save code.", true)]
        public static CommandParser operator +(CommandParser parser, Group[] groups)
        {
            // if "Global" command group does not exist.
            if (parser.GetGroupIndex("Global") == -1)
            {
                parser.AddGroup(new Group("Global"));
            }

            parser.AddGroups(groups);
            return parser;
        }
        */

        public static CommandParser operator +(CommandParser parser, Dictionary<string, Command> cmds)
        {
            // if "Global" command group does not exist.
            if (parser.GetGroupIndex("Global") == -1)
            {
                parser.AddGroup(new Group("Global"));
            }

            foreach (var command in cmds)
            {
                if (parser.GetGroup(command.Key) == null)
                {
                    parser.AddGroup(new Group(command.Key));
                }

                var group = parser.GetGroup(command.Key);
                group += command.Value;
                parser.Replace(new Dictionary<string, Group> { { group.GroupName, group }, });
            }

            return parser;
        }

        public void ProcessCommands()
        {
            if (this.Length == 0)
            {
                this.ShowHelp();
            }
            else
            {
                var cmdgroup = string.Empty;
                var foundcmd = false;
                var foundgrp = false;
                var currentArg = string.Empty;
                foreach (var arg in this.args)
                {
                    currentArg = arg;
                    foreach (var group in this.Groups)
                    {
                        // if (cmd.Group == null && cmd.Group.GroupName.Equals(arg))
                        // {
                        //     foundgrp = true;
                        //     cmdgroup = cmd.Group.GroupName;
                        // }
                        /*else */
                        if (/*cmd.Group != null && */group.CommandEquals(arg) && (group.GroupName.Equals(cmdgroup) || group.GroupName.Equals("Global")))
                        {
                            foundcmd = true;

                            // now we got to filter the commands, stripping the ones already processed for passing to the invoked command.
                            var tmp = this.args.ToList();
                            var index = tmp.IndexOf(arg);
                            tmp.RemoveRange(0, index + 1);
                            group.FindCommand(arg).InvokeCommand(tmp.ToArray());
                            tmp.Clear();
                        }
                    }
                }

                if (!foundcmd && !foundgrp)
                {
                    Console.Error.WriteLine($"Error: invalid command-line option '{currentArg}'.");
                }
            }
        }

        private void ShowHelp()
        {
            // thanks dotnet-cli for the idea of the help contents.
            Console.WriteLine($"Version: {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"usage: {Assembly.GetEntryAssembly().GetName().Name} <command>");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            foreach (var group in this.Groups)
            {
                foreach (var cmd in group.Commands)
                {
                    Console.WriteLine($" {(group.GroupName.Equals("Global") ? string.Empty : group.GroupName + " ")}{cmd.CommandSwitch}\t{cmd.CommandDescription}");
                    Console.WriteLine();
                }
            }

            if (this.HasDocs)
            {
                Console.WriteLine($"For more information, visit {this.DocsUrl}");
            }
        }

        private Group GetGroup(string groupName)
        {
            foreach (var group in this.Groups)
            {
                if (group.GroupName.Equals(groupName))
                {
                    return group;
                }
            }

            return null;
        }

        private int GetGroupIndex(string groupName)
        {
            foreach (var group in this.Groups)
            {
                if (group.GroupName.Equals(groupName))
                {
                    return this.Groups.IndexOf(group);
                }
            }

            return -1; // to be consistant with IndexOf()!!!
        }

        private void AddGroup(Group group)
            => this.Groups.Add(group);

        /*
        [Obsolete("Not needed anymore.", true)]
        private void AddGroups(Group[] groups)
            => this.Groups.AddRange(groups);
        */

        private void Replace(Dictionary<string, Group> groups)
        {
            foreach (var item in groups)
            {
                this.Groups[this.GetGroupIndex(item.Key)] = item.Value;
            }
        }
    }
}
