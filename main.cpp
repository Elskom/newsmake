/*
 * newsmake - News / changelog making tool.
 * licenced under GPL. See LICENSE for details.
 */
#include <exception>
#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
#if __has_include(<filesystem>)
#include <filesystem>
#else
#include <experimental/filesystem>
#endif
#ifdef __cpp_lib_filesystem
namespace fs = std::filesystem;
#elif defined(__cpp_lib_experimental_filesystem)
namespace fs = std::experimental::filesystem;
#elif !defined(__cpp_lib_filesystem) &&                                        \
    !defined(__cpp_lib_experimental_filesystem) &&                             \
    (__has_include(                                                            \
        <filesystem>) || __has_include(<experimental/filesystem>)) && defined(_WIN32)
// Visual Studio 2017 (15.7.x) seems to
// not have a c++ std::experimental::filesystem macro
// when std::filesystem is not present,
// and an std::filesystem macro when
// std::experimental::filesystem is
// not present. Because of this I guess
// fallback to std::experimental::filesystem
// and hope it works (/tableflip).
// However it seems the current preview (15.8.x) does.
namespace fs = std::experimental::filesystem;
#endif

void formatline(std::string &input, bool tabs, bool output_format_md,
                const size_t line_length = 80) {
  if (input.length() < line_length) {
    return;
  }
  const char *indent = (!output_format_md ? (tabs ? "\t\t" : "        ")
                                          : (tabs ? "\t" : "    "));
  const int tab_length = 8;
  int indent_line_length = line_length;
  size_t pos = 0;
  size_t last_pos = 0;
  std::stringstream output;
  while (true) {
    if (pos > 0 && last_pos == pos) {
      throw new std::runtime_error("bug");
    }
    last_pos = pos;
    if (input[pos] == ' ' && pos > 0)
      ++pos;
    if (pos >= input.length()) {
      break;
    }
    std::string sub_s = input.substr(pos, indent_line_length);
    size_t last_space = sub_s.find_last_of(' ');
    if (last_space == 0 || last_space == std::string::npos ||
        pos + indent_line_length >= input.length()) {
      last_space = sub_s.length();
    }
    if (last_space != sub_s.length()) {
      sub_s = sub_s.substr(0, last_space);
    }
    output << sub_s;
    pos += last_space;
    if (pos >= input.length()) {
      break;
    }
    output << '\n' << indent;
    indent_line_length = line_length - tab_length;
  }
  input = output.str();
}

int main(int argc, char *argv[]) {
  std::string ext = ".master";
  std::string project_name;
  std::string outputfile_name;
  bool tabs = true;
  bool devmode = false;
  bool delete_files = true;
  bool first_import = true;
  bool found_master_file = false;
  bool output_format_md = false;
  std::vector<std::string> section_data;
  for (auto &p : fs::recursive_directory_iterator(fs::current_path())) {
    if (p.path().extension().string() == ext) {
      found_master_file = true;
      std::cout << "Processing " << p.path().filename().string() << "..."
                << std::endl;
      std::ifstream master_file(p.path().string());
      for (std::string line; std::getline(master_file, line);) {
        if (line.find("# ") != 0) {
          if (line.find("projname = \"") == 0) {
            project_name = line;
            project_name.erase(0, 12);
            project_name.erase(project_name.length() - 1, 1);
          } else if (line.find("projname = \"") != 0 && project_name.empty()) {
            std::cout << "Error: Project name not set." << std::endl;
            return 1;
          } else if (line.find("devmode = ") == 0) {
            devmode = line.compare("devmode = true") == 0;
          } else if (line.find("genfilename = \"") == 0) {
            outputfile_name = line;
            outputfile_name.erase(0, 15);
            outputfile_name.erase(outputfile_name.length() - 1, 1);
          } else if (line.find("genfilename = \"") != 0 &&
                     outputfile_name.empty()) {
            std::cout << "Error: generated output file name not set."
                      << std::endl;
            return 1;
          } else if (line.find("tabs = ") == 0) {
            tabs = line.compare("tabs = true") == 0;
          } else if (line.find("deletechunkentryfiles = ") == 0) {
            if (!devmode) {
              delete_files = line.compare("deletechunkentryfiles = true") == 0;
            } else {
              delete_files = false;
            }
          }
          // user can now output as markdown.
          else if (line.find("outputasmd = ") == 0) {
            output_format_md = line.compare("outputasmd = true") == 0;
          } else if (line.find("import \"") == 0) {
            std::string imported_folder = line;
            imported_folder.erase(0, 8);
            imported_folder.erase(imported_folder.length() - 1, 1);
            std::string section_string;
            if (first_import) {
              if (output_format_md) {
                section_string = "Whats new in v";
              } else {
                section_string = "                          Whats new in v";
              }
              section_string += imported_folder;
              section_string += "\n============================================"
                                "==================================\n";
              first_import = false;
            } else {
              if (!output_format_md) {
                // if the section divider is not here the resulting markdown
                // would look like trash.
                section_string =
                    "\n============================================="
                    "=================================\n";
                section_string += "                             ";
              }
              section_string += project_name;
              section_string += " v";
              section_string += imported_folder;
              section_string += "\n============================================"
                                "==================================\n";
            }
            std::string section_text;
            if (fs::exists(fs::path(fs::current_path().string() + '/' +
                                    imported_folder))) {
              for (auto &imported_path :
                   fs::recursive_directory_iterator(fs::path(
                       fs::current_path().string() + '/' + imported_folder))) {
                std::ifstream entry_item(imported_path.path().string());
                std::string temp;
                if (!output_format_md) {
                  if (tabs) {
                    temp = "\t+ ";
                  } else {
                    temp = "    + ";
                  }
                } else {
                  temp = "+ ";
                }
                for (std::string entry_line;
                     std::getline(entry_item, entry_line);) {
                  temp += entry_line;
                }
                formatline(temp, tabs, output_format_md);
                temp += '\n';
                section_text += temp;
                entry_item.close();
              }
            }
            if (delete_files && !section_text.empty()) {
              // save section text and then delete the folder.
              std::ofstream section_file(fs::current_path().string() + '/' +
                                         imported_folder + ".section");
              section_file << section_text;
              section_file.close();
              for (auto &imported_path :
                   fs::recursive_directory_iterator(fs::path(
                       fs::current_path().string() + '/' + imported_folder))) {
                fs::remove(imported_path);
              }
              fs::remove(fs::path(fs::current_path().string() + '/' +
                                  imported_folder));
            }
            if (section_text.empty()) {
              // load saved *.section file.
              if (fs::exists(fs::path(fs::current_path().string() + '/' +
                                      imported_folder + ".section"))) {
                std::ifstream section_file(fs::current_path().string() + '/' +
                                           imported_folder + ".section");
                for (std::string entry_line;
                     std::getline(section_file, entry_line);) {
                  section_text += entry_line + '\n';
                }
                section_file.close();
              }
            }
            section_string += section_text;
            section_data.push_back(section_string);
          }
        }
      }
      if (!outputfile_name.empty()) {
        if (!fs::exists(fs::path(outputfile_name))) {
          fs::create_directories(fs::path(outputfile_name).parent_path());
        }
        std::ofstream output_file(outputfile_name);
        for (const auto &section : section_data) {
          output_file << section;
        }
        output_file.close();
        std::cout << "Successfully Generated '" << outputfile_name << "'."
                  << std::endl;
      }
      master_file.close();
    }
  }
  if (!found_master_file) {
    std::cout << "Error: no *.master file found." << std::endl;
    return 1;
  }
  return 0;
}
