#include <iostream>
#include <vector>
#include <string>
#include <fstream>
#include <experimental/filesystem>


int main(int argc, char *argv[])
{
  std::string ext(".master");
  std::string project_name;
  std::string outputfile_name;
  bool tabs;
  bool delete_files;
  std::vector<std::string> section_data;
  for(auto& p: std::experimental::filesystem::recursive_directory_iterator(std::experimental::filesystem::current_path())
  {
    if(p.path().extension() == ext())
    {
      std::cout << "Processing " << p.filename() << "..." << std::endl;
      std::ifstream master_file(p);
      for (std::string line; std::getline(master_file, line);)
      {
        if (line.find("# ") != 0)
        {
          //projname = "Els_kom"
          //genfilename = "../../bin/x86/Release/news.txt"
          if (line.find("projname = \"") == 0)
          {
            // project name set.
          }
          else
          {
            std::cout << "Error: Project name not set." << std::endl;
            return 1;
          }
          if (line.find("genfilename = \"") == 0)
          {
            // generated output file name set.
          }
          else
          {
            std::cout << "Error: generated output file name not set." << std::endl;
            return 1;
          }
          if (line.find("tabs = ") == 0)
          {
            tabs = line.c_str() == "tabs = true";
          }
          else
          {
            tabs = true;
          }
          if (line.find("deletechunkentryfiles = ") == 0)
          {
            delete_files = line.c_str() == "deletechunkentryfiles = true";
          }
          else
          {
            delete_files = true;
          }
          if (line.find("import \"") == 0)
          {
            // an folder was imported we must
            // recursive loop through all files
            // in it and open them and process the data.
          }
          // open the imported files in the folder name of the import.
          
          // get imported files data.
          
          // close imported files.
          
          // write data to the output file set in the *.master file.
        }
      }
      master_file.close();
    }
  }
  return 0;
}
