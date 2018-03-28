#include <experimental/filesystem>
#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>

void formatline(std::string &line, bool tabs)
{
  std::stringstream linestream;
  int line_width = 79;
  for (size_t i = 0; i < line.size(); i++)
  {
    // TODO: Make this never split words
    // that dtarts before line_width and
    // ends after line_width by moving
    // that word to an new line to
    // preserve readability.
    if (i % line_width == 0)
    {
      if (line_width == 79)
      {
        // Decrement this to always
        // ensure this always splits
        // the line at the 80th
        // character no matter what.
        line_width -= 8;
      }
      if (tabs)
      {
        linestream << "\n\t\t";
      }
      else
      {
        linestream << "\n        ";
      }
    }
    linestream << line[i];
  }
  line = linestream.str();
}

int main(int argc, char *argv[])
{
  std::string ext = ".master";
  std::string project_name;
  std::string outputfile_name;
  bool tabs = true;
  bool delete_files = true;
  bool first_import = true;
  bool found_master_file = false;
  std::vector<std::string> section_data;
  for (auto &p : std::experimental::filesystem::recursive_directory_iterator(
           std::experimental::filesystem::current_path()))
  {
    if (p.path().extension().string() == ext)
    {
      found_master_file = true;
      std::cout << "Processing " << p.path().filename().string() << "..."
                << std::endl;
      std::ifstream master_file(p);
      for (std::string line; std::getline(master_file, line);)
      {
        if (line.find("# ") != 0)
        {
          if (line.find("projname = \"") == 0)
          {
            project_name = line;
            project_name.erase(0, 12);
            project_name.erase(project_name.length() - 1, 1);
          }
          else if (line.find("projname = \"") != 0 && project_name.empty())
          {
            std::cout << "Error: Project name not set." << std::endl;
            return 1;
          }
          else if (line.find("genfilename = \"") == 0)
          {
            outputfile_name = line;
            outputfile_name.erase(0, 15);
            outputfile_name.erase(outputfile_name.length() - 1, 1);
          }
          else if (line.find("genfilename = \"") != 0 && outputfile_name.empty())
          {
            std::cout << "Error: generated output file name not set."
                      << std::endl;
            return 1;
          }
          else if (line.find("tabs = ") == 0)
          {
            tabs = line.compare("tabs = true") == 0;
          }
          else if (line.find("deletechunkentryfiles = ") == 0)
          {
            delete_files = line.compare("deletechunkentryfiles = true") == 0;
          }
          else if (line.find("import \"") == 0)
          {
            std::string imported_folder = line;
            imported_folder.erase(0, 8);
            imported_folder.erase(imported_folder.length() - 1, 1);
            std::string section_string;
            if (first_import)
            {
              section_string = "                          Whats new in v";
              section_string += imported_folder;
              section_string += "\n============================================"
                                "==================================\n";
              first_import = false;
            }
            else
            {
              section_string = "\n============================================="
                               "=================================\n";
              section_string += "                             ";
              section_string += project_name;
              section_string += " v";
              section_string += imported_folder;
              section_string += "\n============================================"
                                "==================================\n";
            }
            for (auto &imported_path :
                 std::experimental::filesystem::recursive_directory_iterator(
                     std::experimental::filesystem::path(
                         std::experimental::filesystem::current_path()
                             .string() +
                         "\\" + imported_folder)))
            {
              std::ifstream entry_item(imported_path);
              std::string temp;
              if (tabs)
              {
                temp = "\t+ ";
              }
              else
              {
                temp = "    + ";
              }
              for (std::string entry_line; std::getline(entry_item, entry_line);)
              {
                temp += entry_line;
              }
              formatline(temp, tabs);
              section_string += temp;
              section_string += "\n";
              entry_item.close();
            }
            section_data.push_back(section_string);
          }
        }
      }
      if (!outputfile_name.empty())
      {
        std::ofstream output_file(outputfile_name);
        for (const auto &section : section_data)
        {
          output_file << section;
        }
        output_file.close();
        std::cout << "Successfully Generated '" << outputfile_name << "'." << std::endl;
      }
      master_file.close();
    }
  }
  if (!found_master_file)
  {
    std::cout << "Error: no *.master file found." << std::endl;
    return 1;
  }
  return 0;
}
