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

    internal class CommandParsor
    {
        private readonly string[] args;
        private readonly List<Command> cmds;

        internal CommandParsor(string[] args)
        {
            this.args = args;
            this.cmds = new List<Command>();
        }

        internal int Length
            => this.args.Length;

        internal bool HasDocs { get; set; } = false;

        internal string DocsUrl { get; set; } = string.Empty;

        internal void AddCommand(Command cmd)
            => this.cmds.Add(cmd);

        internal void AddCommands(Command[] cmds)
            => this.cmds.AddRange(cmds);

        internal void ProcessCommands()
        {
            var cmdgroup = string.Empty;
            var foundcmd = false;
            var foundgrp = false;
            var currentArg = string.Empty;
            foreach (var arg in this.args)
            {
                currentArg = arg;
                foreach (var cmd in this.cmds)
                {
                    if (cmd.CommandGroup.Equals(arg))
                    {
                        foundgrp = true;
                        cmdgroup = cmd.CommandGroup;
                    }

                    if (cmd.Equals(arg) && (cmd.CommandGroup.Equals(cmdgroup) || cmd.CommandGroup.Equals("Global")))
                    {
                        foundcmd = true;

                        // now we got to filter the commands, stripping the ones already processed for passing to the invoked command.
                        var tmp = this.args.ToList();
                        var index = tmp.IndexOf(arg);
                        tmp.RemoveRange(0, index + 1);
                        cmd.InvokeCommand(tmp.ToArray());
                        tmp.Clear();
                    }
                }
            }

            if (!foundcmd && !foundgrp)
            {
                Console.Error.WriteLine($"Error: invalid command-line option '{currentArg}'.");
            }
        }

        internal void ShowHelp()
        {
            // thanks dotnet-cli for the idea of the help contents.
            Console.WriteLine($"Version: {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"usage: {Assembly.GetEntryAssembly().GetName().Name} <command>");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            foreach (var cmd in this.cmds)
            {
                Console.WriteLine($" {(cmd.CommandGroup.Equals("Global") ? string.Empty : cmd.CommandGroup)} {cmd.CommandSwitch}\t{cmd.CommandDescription}");
                Console.WriteLine();
            }

            if (this.HasDocs)
            {
                Console.WriteLine($"For more information, visit {this.DocsUrl}");
            }
        }
    }
}
