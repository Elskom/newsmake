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
            MiniDump.DumpFailed += MiniDump_DumpFailed;
            MiniDump.DumpGenerated += MiniDump_DumpGenerated;
            _ = Assembly.GetEntryAssembly().EntryPoint.GetCustomAttributes(false);
            var commandParsor = new CommandParsor(args);
            commandParsor.AddCommand(new Command("--version", "Global", "Shows the version of this command-line program.", (string[] commands) => Commands.VersionCommand()));
            commandParsor.AddCommand(new Command("build", "Global", "builds a changelog or news file from any *.master file in the current or sub directory.", (string[] commands) => Commands.BuildCommand()));
            commandParsor.AddCommand(new Command("release", "new", "Creats a new pending release folder and imports it in the *.master file in the current or sub directory.", (string[] release) => Commands.NewReleaseCommand(release)));
            commandParsor.AddCommand(new Command("entry", "new", "Creates a new entry file for the current pending release.", (string[] commands) => Commands.NewEntryCommand()));
            commandParsor.AddCommand(new Command("release", "finalize", "Finalizes a pending release to a section file.", (string[] commands) => Commands.FinalizeReleaseCommand()));
            if (commandParsor.Length == 0)
            {
                commandParsor.ShowHelp();
            }
            else
            {
                commandParsor.ProcessCommands();
            }

            return 0;
        }

        private static void MiniDump_DumpGenerated(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"{e.Caption}: {e.Text}");
            Environment.Exit(1);
        }

        private static void MiniDump_DumpFailed(object sender, MessageEventArgs e)
            => Console.WriteLine($"{e.Caption}: {e.Text}");
    }
}
