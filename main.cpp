#include <experimental/filesystem>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

// formats the data stored in the changelog entry files
// to always be no greater than 79 characters per line.
void formatline(std::string &line, bool tabs)
{
  size_t loops = line.size() / 79;
  if (loops > 0)
  {
    for (size_t i = 1; i < loops; i++)
    {
      size_t line_off =
          (79 * i) + 1; // to get position to split into an new line.
      while (line.compare(line_off, 1, " ") != 0)
      {
        line_off -= 1; // rewind by 1 character until space.
      }
      if (tabs)
      {
        line.insert(line_off, "\n\t\t");
      }
      else
      {
        line.insert(line_off, "\n        ");
      }
    }
  }
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
          else
          {
            std::cout << "Error: Project name not set." << std::endl;
            return 1;
          }
          if (line.find("genfilename = \"") == 0)
          {
            outputfile_name = line;
            outputfile_name.erase(0, 15);
            outputfile_name.erase(outputfile_name.length() - 1, 1);
          }
          else
          {
            std::cout << "Error: generated output file name not set."
                      << std::endl;
            return 1;
          }
          if (line.find("tabs = ") == 0)
          {
            tabs = line.c_str() == "tabs = true";
          }
          if (line.find("deletechunkentryfiles = ") == 0)
          {
            delete_files = line.c_str() == "deletechunkentryfiles = true";
          }
          if (line.find("import \"") == 0)
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
                                "==================================";
              first_import = false;
            }
            else
            {
              section_string = "\n============================================="
                               "=================================";
              section_string += "                             ";
              section_string += project_name;
              section_string += " v";
              section_string += imported_folder;
              section_string += "\n============================================"
                                "==================================";
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
              while (entry_item >> temp)
              {
                // do nothing here still reading...
              }
              formatline(temp, tabs);
              // entry item prefix. Important to not leave out.
              if (tabs)
              {
                section_string += "\t+ ";
              }
              else
              {
                section_string += "    + ";
              }
              section_string += temp;
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
