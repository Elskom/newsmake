// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal static class Program
    {
        internal static void Formatline(ref string input, bool tabs, bool output_format_md, int line_length = 80)
        {
            if (input.Length < line_length)
            {
                return;
            }

            string indent = !output_format_md ? (tabs ? "\t\t" : "        ") : (tabs ? "\t" : "    ");
            int tab_length = 8;
            int indent_line_length = line_length;
            int pos = 0;
            int last_pos = 0;
            StringBuilder output = new StringBuilder();
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

                Console.WriteLine($"pos: {pos}");
                Console.WriteLine($"indent_line_length: {indent_line_length}");
                Console.WriteLine($"input.Length - pos: {input.Length - pos}");
                string sub_s = input.Substring(pos, input.Length - pos < indent_line_length ? indent_line_length : input.Length - pos);
                int last_space = sub_s.LastIndexOf(' ');
                if (last_space == 0 || last_space == int.MaxValue || pos + indent_line_length >= input.Length)
                {
                    last_space = sub_s.Length;
                }

                if (last_space != sub_s.Length)
                {
                    sub_s = sub_s.Substring(0, last_space);
                }

                output.Append(sub_s);
                pos += last_space;
                if (pos >= input.Length)
                {
                    break;
                }

                output.Append('\n' + indent);
                indent_line_length = line_length - tab_length;
            }

            input = output.ToString();
        }

        internal static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                string command = args[1];
                if (command.Equals("--version"))
                {
                    Console.WriteLine("Version: 1.0.3");
                }
            }
            else
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
          // hack to make this all work as intended.
          var line = master_file[i];
          if (!line.Contains("# "))
          {
            do
            {
              // get environment variable name.
              // find the "$(" and make a substring then find the first ")"
              int env_open = line.IndexOf("$(");
              int env_close = line.IndexOf(")");
              if (env_open != env_close)
              {
                env_open += 2;
                string envvar = line.Substring(env_open, env_close - env_open);
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
                  return 1;
                }
              }
            }
            while (line.Contains("$("));

            if (line.Contains("projname = \""))
            {
              project_name = line;
              project_name = project_name.Erase(0, 12);
              project_name = project_name.Erase(project_name.Length - 1, 1);
            }
            else if (!line.Contains("projname = \"") && project_name.Equals(string.Empty))
            {
              Console.Error.WriteLine("Fatal: Project name not set.");
              return 1;
            }
            else if (line.Contains("devmode = "))
            {
              devmode = line.Equals("devmode = true");
            }
            else if (line.Contains("genfilename = \""))
            {
              outputfile_name = line;
              outputfile_name = outputfile_name.Erase(0, 15);
              outputfile_name = outputfile_name.Erase(outputfile_name.Length - 1, 1);
            }
            else if (!line.Contains("genfilename = \"") && outputfile_name.Equals(string.Empty))
            {
              Console.Error.WriteLine("Fatal: generated output file name not set.");
              return 1;
            }
            else if (line.Contains("tabs = "))
            {
              tabs = line.Equals("tabs = true");
            }
            else if (line.Contains("deletechunkentryfiles = "))
            {
              if (!devmode)
              {
                delete_files = line.Equals("deletechunkentryfiles = true");
              }
              else
              {
                delete_files = false;
              }
            }
            else if (line.Contains("outputasmd = "))
            {
              output_format_md = line.Equals("outputasmd = true");
            }
            else if (line.Contains("import \""))
            {
              string imported_folder = line;
              imported_folder = imported_folder.Erase(0, 8);
              imported_folder = imported_folder.Erase(imported_folder.Length - 1, 1);
              var section_string = string.Empty;
              if (first_import)
              {
                if (output_format_md)
                {
                  section_string = "Whats new in v";
                }
                else
                {
                  section_string = "                          Whats new in v";
                }

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
                  string temp;
                  if (!output_format_md)
                  {
                    if (tabs)
                    {
                      temp = "\t+ ";
                    }
                    else
                    {
                      temp = "    + ";
                    }
                  }
                  else
                  {
                    temp = "+ ";
                  }

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
                var section_file = File.OpenWrite(Directory.GetCurrentDirectory() + "/" + imported_folder + ".section");
                section_file.Write(Encoding.UTF8.GetBytes(section_text), 0, Encoding.UTF8.GetByteCount(section_text));
                section_file.Dispose();
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

          var output_file = finfo.OpenWrite();
          foreach (var section in section_data)
          {
            output_file.Write(Encoding.UTF8.GetBytes(section), 0, Encoding.UTF8.GetByteCount(section));
          }

          output_file.Dispose();
          Console.WriteLine($"Successfully Generated '{outputfile_name}'.");
        }
                    }
                }

                if (!found_master_file)
                {
                    Console.Error.WriteLine("Fatal: no *.master file found.");
                    return 1;
                }
            }

            return 0;
        }
    }
}
