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
          if (line.find("projname = \"") == 0)
          {
            project_name = line;
            project_name.erase(0, 12);
            project_name.erase(project_name.length() -1, 1);
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
            outputfile_name.erase(outputfile_name.length() -1, 1);
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
            std::string imported_folder = line;
            // get the import folder name stripping the extra stuff.
            imported_folder.erase(0, 8);
            imported_folder.erase(imported_folder.length() -1, 1);
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
