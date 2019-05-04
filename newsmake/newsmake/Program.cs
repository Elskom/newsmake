// Copyright (c) 2018-2019, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System;
    using System.Messaging;
    using System.Reflection;
    using Elskom.Generic.Libs;

    internal static class Program
    {
        [MiniDump(Text = "Please send a copy of {0} to https://github.com/Elskom/newsmake/issues by making an issue and attaching the log(s) and mini-dump(s).", DumpType = MinidumpTypes.ValidTypeFlags)]
        internal static int Main(string[] args)
        {
            // MiniDump.DumpFailed += MiniDump_DumpFailed;
            MiniDump.DumpMessage += MiniDump_DumpMessage;
            _ = Assembly.GetEntryAssembly().EntryPoint.GetCustomAttributes(false);
            var commandParser = new CommandParser(args);
            commandParser += new Command[]
            {
                new Command("--version", "Global", "Shows the version of this command-line program.", (string[] commands) => Commands.VersionCommand()),
                new Command("build", "Global", "builds a changelog or news file from any *.master file in the current or sub directory.", (string[] commands) => Commands.BuildCommand()),
                new Command("release", "new", "Creates a new pending release folder and imports it in the *.master file in the current or sub directory.", (string[] release) => Commands.NewReleaseCommand(release)),
                new Command("entry", "new", "Creates a new entry file for the current pending release.", (string[] commands) => Commands.NewEntryCommand()),
                new Command("release", "finalize", "Finalizes a pending release to a section file.", (string[] commands) => Commands.FinalizeReleaseCommand()),
            };
            if (commandParser.Length == 0)
            {
                commandParser.ShowHelp();
            }
            else
            {
                commandParser.ProcessCommands();
            }

            return 0;
        }

        private static void MiniDump_DumpMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"{e.Caption}: {e.Text}");
            if (!e.Text.StartsWith("Mini-dumping failed with Code: "))
            {
                Environment.Exit(1);
            }
        }
    }
}
