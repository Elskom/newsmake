/*
 * newsmake - News / changelog making tool.
 * licenced under GPL. See LICENSE for details.
 */
#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
#include "fs.hpp"
#if _has_include(<filesystem>)
#include <filesystem>
#ifdef HAVE_STD_FS
namespace fs = std::filesystem;
#elif HAVE_STD_EXP_FS
namespace fs = std::experimental::filesystem;
#endif
#else
#include <experimental/filesystem>
namespace fs = std::experimental::filesystem;
#endif

void formatline(std::string &input, bool tabs)
{
  // TODO: count the spaces and tabs upon new line.
  size_t pos = 0;
  size_t last_pos = 0;
  std::stringstream output;

  //first pass ie. first line
  last_pos = pos;
  pos = input.substr(last_pos, 80).find_last_of(' '); //todo: verify this check includes the last character.. it should, but in case it doesn't
  /*
  size_t tab_pos;
  do
  {
    //count tabs
    //count * (4-1)
    //outer loops counts each tab already once
  }
  while (tab_pos != std::string::npos);
  */
  output << input.substr(last_pos, pos - last_pos);
  if (tabs)
  {
    output << "\n\t\t";
  }
  else
  {
    output << "\n        ";
  }

  //second pass, and subsequent passes. ie. not first line    
  do
  {
    last_pos = pos;
    pos = input.substr(last_pos, 72).find_last_of(' '); //todo: verify this check includes the last character.. it should, but in case it doesn't
    /*
    size_t tab_pos;
    do
    {
      //count tabs
      //count * (4-1)
      //outer loops counts each tab already once
    }
    while (tab_pos != std::string::npos);
    */
    output << input.substr(last_pos, pos - last_pos);
    if (tabs)
    {
      output << "\n\t\t";
    }
    else
    {
      output << "\n        ";
    }
  }
  while (pos != std::string::npos);
  input = output.str();
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
  for (auto &p : fs::recursive_directory_iterator(
           fs::current_path()))
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
            std::string section_text;
            for (auto &imported_path :
                 fs::recursive_directory_iterator(
                     fs::path(
                         fs::current_path()
                             .string() +
                         '/' + imported_folder)))
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
              temp += '\n';
              section_text += temp;
              entry_item.close();
            }
            if (delete_file && !section_text.empty())
            {
              // save section text and then delete the folder.
            }
            if (section_text.empty())
            {
              // load saved *.section file.
            }
            section_string += section_text;
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
