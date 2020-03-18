// Copyright (c) 2018-2020, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class Commands
    {
        internal static void VersionCommand()
            => Console.WriteLine($"Version: {Assembly.GetEntryAssembly().GetName().Version}");

        internal static void BuildCommand()
        {
            var ext = ".master";
            var project_name = string.Empty;
            var outputfile_name = string.Empty;
            var tabs = true;
            var devmode = false;
            var delete_files = true;
            var first_import = true;
            var found_master_file = false;
            var output_format_md = false;
            var section_data = new List<string>();
            foreach (var p in Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
            {
                if (p.EndsWith(ext))
                {
                    found_master_file = true;
                    Console.WriteLine($"Processing {p}...");

                    // set the current directory to p.
                    Directory.SetCurrentDirectory(new FileInfo(p).Directory.FullName);
                    var master_file = File.ReadAllLines(p);
                    for (var i = 0; i < master_file.Length; i++)
                    {
                        // a hack to make this all work as intended.
                        var line = master_file[i];
                        if (!line.Contains("# "))
                        {
                            do
                            {
                                // get environment variable name.
                                // find the "$(" and make a substring then find the first ")"
                                var env_open = line.IndexOf("$(");
                                var env_close = line.IndexOf(")");
                                if (env_open != env_close)
                                {
                                    env_open += 2;
                                    var envvar = line.Substring(env_open, env_close - env_open);
                                    var envvalue = Environment.GetEnvironmentVariable(envvar);

                                    // a hack to resolve the current working directory manually...
                                    if (envvar.Equals("CD") || envvar.Equals("cd"))
                                    {
                                        envvalue = Directory.GetCurrentDirectory().ToString();
                                        envvalue += "/";
                                        envvalue = envvalue.Replace("\\", "/");
                                        env_open -= 2;
                                        line = line.Replace(line.Substring(env_open, (env_close + 1) - env_open), envvalue);
                                    }
                                    else if (envvalue != null)
                                    {
                                        env_open -= 2;
                                        line = line.Replace(line.Substring(env_open, (env_close + 1) - env_open), envvalue);
                                    }
                                    else
                                    {
                                        Console.Error.WriteLine("Fatal: Environment variable does not exist.");
                                        Environment.Exit(1);
                                    }
                                }
                            }
                            while (line.Contains("$("));

                            if (line.Contains("projname = \""))
                            {
                                project_name = line;
                                project_name = project_name.Replace(project_name.Substring(0, 12), string.Empty);
                                project_name = project_name.Replace(project_name.Substring(project_name.Length - 1, 1), string.Empty);
                            }
                            else if (!line.Contains("projname = \"") && project_name.Equals(string.Empty))
                            {
                                Console.Error.WriteLine("Fatal: Project name not set.");
                                Environment.Exit(1);
                            }
                            else if (line.Contains("devmode = "))
                            {
                                devmode = line.Equals("devmode = true");
                            }
                            else if (line.Contains("genfilename = \""))
                            {
                                outputfile_name = line;
                                outputfile_name = outputfile_name.Replace(outputfile_name.Substring(0, 15), string.Empty);
                                outputfile_name = outputfile_name.Replace(outputfile_name.Substring(outputfile_name.Length - 1, 1), string.Empty);
                            }
                            else if (!line.Contains("genfilename = \"") && outputfile_name.Equals(string.Empty))
                            {
                                Console.Error.WriteLine("Fatal: generated output file name not set.");
                                Environment.Exit(1);
                            }
                            else if (line.Contains("tabs = "))
                            {
                                tabs = line.Equals("tabs = true");
                            }
                            else if (line.Contains("deletechunkentryfiles = "))
                            {
                                delete_files = !devmode ? line.Equals("deletechunkentryfiles = true") : false;
                            }
                            else if (line.Contains("outputasmd = "))
                            {
                                output_format_md = line.Equals("outputasmd = true");
                            }
                            else if (line.Contains("import \""))
                            {
                                var imported_folder = line;
                                imported_folder = imported_folder.Replace(imported_folder.Substring(0, 8), string.Empty);
                                imported_folder = imported_folder.Replace(imported_folder.Substring(imported_folder.Length - 1, 1), string.Empty);
                                var section_string = string.Empty;
                                if (first_import)
                                {
                                    section_string = output_format_md ? "Whats new in v" : "                          Whats new in v";
                                    section_string += imported_folder;
                                    section_string += "\n==============================================================================\n";
                                    first_import = false;
                                }
                                else
                                {
                                    if (!output_format_md)
                                    {
                                        // if the section divider is not here the resulting markdown
                                        // would look like trash.
                                        section_string =
                                          "\n==============================================================================\n";
                                        section_string += "                             ";
                                    }

                                    section_string += project_name;
                                    section_string += " v";
                                    section_string += imported_folder;
                                    section_string += "\n==============================================================================\n";
                                }

                                var section_text = string.Empty;
                                if (Directory.Exists(Directory.GetCurrentDirectory() + "/" + imported_folder))
                                {
                                    foreach (var imported_path in
                                      Directory.GetFiles(Directory.GetCurrentDirectory() + "/" + imported_folder, "*", SearchOption.AllDirectories))
                                    {
                                        var temp = !output_format_md ? tabs ? "\t+ " : "    + " : "+ ";
                                        var entry_lines = File.ReadAllLines(imported_path);
                                        foreach (var entry_line in entry_lines)
                                        {
                                            temp += entry_line;
                                        }

                                        Formatline(ref temp, tabs, output_format_md);
                                        temp += "\n";
                                        section_text += temp;
                                    }
                                }

                                if (delete_files && !section_text.Equals(string.Empty))
                                {
                                    // save section text and then delete the folder.
                                    using (var section_file = File.OpenWrite(Directory.GetCurrentDirectory() + "/" + imported_folder + ".section"))
                                    {
                                        section_file.Write(Encoding.UTF8.GetBytes(section_text), 0, Encoding.UTF8.GetByteCount(section_text));
                                    }

                                    foreach (var imported_path in Directory.GetFiles(Directory.GetCurrentDirectory() + "/" + imported_folder, "*", SearchOption.AllDirectories))
                                    {
                                        File.Delete(imported_path);
                                    }

                                    Directory.Delete(Directory.GetCurrentDirectory() + "/" + imported_folder);
                                }

                                if (section_text.Equals(string.Empty))
                                {
                                    // load saved *.section file.
                                    if (File.Exists(Directory.GetCurrentDirectory() + "/" + imported_folder + ".section"))
                                    {
                                        section_text = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + imported_folder + ".section");
                                    }
                                }

                                section_string += section_text;
                                section_data.Add(section_string);
                            }
                        }
                    }

                    if (!outputfile_name.Equals(string.Empty))
                    {
                        var finfo = new FileInfo(outputfile_name);
                        if (!finfo.Directory.Exists)
                        {
                            finfo.Directory.Create();
                        }

                        using (var output_file = finfo.OpenWrite())
                        {
                            foreach (var section in section_data)
                            {
                                output_file.Write(Encoding.UTF8.GetBytes(section), 0, Encoding.UTF8.GetByteCount(section));
                            }
                        }

                        Console.WriteLine($"Successfully Generated '{outputfile_name}'.");

                        // clear leaking any specific information to other *.master file configurations that should not be present.
                        project_name = string.Empty;
                        outputfile_name = string.Empty;
                        tabs = true;
                        devmode = false;
                        delete_files = true;
                        first_import = true;
                        output_format_md = false;
                        section_data.Clear();
                    }
                }
            }

            if (!found_master_file)
            {
                Console.Error.WriteLine("Fatal: no *.master file found.");
                Environment.Exit(1);
            }
        }

        internal static void NewReleaseCommand(string[] args)
        {
            if (args.Length == 1)
            {
                var ext = ".master";
                var path = string.Empty;
                string[] lines = null;
                var masterfile = string.Empty;
                foreach (var p in Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
                {
                    if (p.EndsWith(ext))
                    {
                        var fi = new FileInfo(p);
                        path = fi.Directory.FullName;
                        lines = File.ReadAllLines(p);
                        masterfile = p;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(path))
                {
                    Console.Error.WriteLine("Fatal: no *.master file found.");
                    Environment.Exit(1);
                }

                foreach (var p in Directory.GetDirectories(path))
                {
                    if (!Directory.Exists($"{path}{Path.DirectorySeparatorChar}{args[0]}"))
                    {
                        Console.Error.WriteLine($"Fatal: The *.master file already has a pending release.");
                        Environment.Exit(1);
                    }
                }

                if (!Directory.Exists($"{path}{Path.DirectorySeparatorChar}{args[0]}"))
                {
                    _ = Directory.CreateDirectory($"{path}{Path.DirectorySeparatorChar}{args[0]}");

                    // TODO: Insert the new release first in the *.master file's imports.
                    var linecnt = 0;
                    foreach (var line in lines)
                    {
                        linecnt++;
                        if (line.Contains("import \"") && !line.Contains($"import \"{args[0]}\""))
                        {
                            break;
                        }
                        else if (line.Contains($"import \"{args[0]}\""))
                        {
                            Console.Error.WriteLine($"Fatal: The import of the release '{args[0]}' already exists in the *.master file.");
                            Environment.Exit(1);
                        }
                    }

                    var linelst = lines.ToList();
                    linelst.Insert(linecnt - 1, $"import \"{args[0]}\"");
                    lines = linelst.ToArray();
                    File.WriteAllLines(masterfile, lines);
                }
                else
                {
                    Console.Error.WriteLine($"Fatal: The release '{args[0]}' already exists.");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.Error.WriteLine("Fatal: no release name specified.");
                Environment.Exit(1);
            }
        }

        internal static void NewEntryCommand()
        {
            var ext = ".master";
            var path = string.Empty;
            foreach (var p in Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
            {
                if (p.EndsWith(ext))
                {
                    var fi = new FileInfo(p);
                    path = fi.Directory.FullName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                Console.Error.WriteLine("Fatal: no *.master file found.");
                Environment.Exit(1);
            }

            foreach (var p in Directory.GetDirectories(path))
            {
                var currentFile = "item000";
                var files = Directory.GetFiles(p);
                foreach (var file in files)
                {
                    currentFile = file;
                }

                var fileCountStr = string.Empty;
                if (files.Length < 10)
                {
                    fileCountStr = $"00{files.Length}";
                }
                else if (files.Length > 9)
                {
                    fileCountStr = $"0{files.Length}";
                }
                else if (files.Length > 99)
                {
                    fileCountStr = $"{files.Length}";
                }

                var newfileCount = files.Length + 1;
                var newfileCountStr = string.Empty;
                if (newfileCount < 10)
                {
                    newfileCountStr = $"00{newfileCount}";
                }
                else if (newfileCount > 9)
                {
                    newfileCountStr = $"0{newfileCount}";
                }
                else if (newfileCount > 99)
                {
                    newfileCountStr = $"{newfileCount}";
                }

                var newFile = $"{currentFile.Replace($"item{fileCountStr}", $"item{newfileCountStr}")}";
                using (File.Create(newFile))
                {
                }
            }
        }

        internal static void FinalizeReleaseCommand()
        {
            var ext = ".master";
            var tabs = true;
            var output_format_md = false;
            var found = false;
            foreach (var p in Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
            {
                if (p.EndsWith(ext))
                {
                    found = true;
                    var fi = new FileInfo(p);
                    var master_file = File.ReadAllLines(p);
                    for (var i = 0; i < master_file.Length; i++)
                    {
                        // a hack to make this all work as intended.
                        var line = master_file[i];
                        if (!line.Contains("# "))
                        {
                            if (line.Contains("tabs = "))
                            {
                                tabs = line.Equals("tabs = true");
                            }
                            else if (line.Contains("outputasmd = "))
                            {
                                output_format_md = line.Equals("outputasmd = true");
                            }
                            else if (line.Contains("import \""))
                            {
                                var imported_folder = line;
                                imported_folder = imported_folder.Replace(imported_folder.Substring(0, 8), string.Empty);
                                imported_folder = imported_folder.Replace(imported_folder.Substring(imported_folder.Length - 1, 1), string.Empty);
                                var section_text = string.Empty;
                                if (Directory.Exists($"{fi.Directory.FullName}{Path.DirectorySeparatorChar}{imported_folder}"))
                                {
                                    foreach (var imported_path in
                                      Directory.GetFiles($"{fi.Directory.FullName}{Path.DirectorySeparatorChar}{imported_folder}", "*", SearchOption.AllDirectories))
                                    {
                                        var temp = !output_format_md ? tabs ? "\t+ " : "    + " : "+ ";
                                        var entry_lines = File.ReadAllLines(imported_path);
                                        foreach (var entry_line in entry_lines)
                                        {
                                            temp += entry_line;
                                        }

                                        Formatline(ref temp, tabs, output_format_md);
                                        temp += "\n";
                                        section_text += temp;
                                    }
                                }

                                if (!section_text.Equals(string.Empty))
                                {
                                    // save section text and then delete the folder.
                                    using (var section_file = File.OpenWrite($"{fi.Directory.FullName}{Path.DirectorySeparatorChar}{imported_folder}.section"))
                                    {
                                        section_file.Write(Encoding.UTF8.GetBytes(section_text), 0, Encoding.UTF8.GetByteCount(section_text));
                                    }

                                    foreach (var imported_path in Directory.GetFiles($"{fi.Directory.FullName}{Path.DirectorySeparatorChar}{imported_folder}", "*", SearchOption.AllDirectories))
                                    {
                                        File.Delete(imported_path);
                                    }

                                    Directory.Delete($"{fi.Directory.FullName}{Path.DirectorySeparatorChar}{imported_folder}");
                                }
                            }
                        }
                    }
                }
            }

            if (!found)
            {
                Console.Error.WriteLine("Fatal: no *.master file found.");
                Environment.Exit(1);
            }
        }

        internal static void Formatline(ref string input, bool tabs, bool output_format_md, int line_length = 80)
        {
            if (input.Length < line_length)
            {
                return;
            }

            var indent = !output_format_md ? (tabs ? "\t\t" : "        ") : (tabs ? "\t" : "    ");
            var tab_length = 8;
            var indent_line_length = line_length;
            var pos = 0;
            var last_pos = 0;
            var output = new StringBuilder();
            while (true)
            {
                if (pos > 0 && last_pos == pos)
                {
                    throw new Exception("bug");
                }

                last_pos = pos;
                if (input[pos] == ' ' && pos > 0)
                {
                    ++pos;
                }

                if (pos >= input.Length)
                {
                    break;
                }

                var sub_s = input.Substring(pos, input.Length - pos > indent_line_length ? indent_line_length : input.Length - pos);
                var last_space = sub_s.LastIndexOf(' ');
                if (last_space == 0 || last_space == int.MaxValue || pos + indent_line_length >= input.Length)
                {
                    last_space = sub_s.Length;
                }

                if (last_space != sub_s.Length)
                {
                    sub_s = sub_s.Substring(0, last_space);
                }

                _ = output.Append(sub_s);
                pos += last_space;
                if (pos >= input.Length)
                {
                    break;
                }

                _ = output.Append('\n' + indent);
                indent_line_length = line_length - tab_length;
            }

            input = output.ToString();
        }
    }
}
