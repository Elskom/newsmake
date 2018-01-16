#include <iostream>
#include <string>
#include <fstream>
#include <experimental/filesystem>

namespace fs = std::experimental::filesystem;

int main(int argc, char *argv[])
{
  std::string ext(".master");
  for(auto& p: fs::recursive_directory_iterator(fs::current_path())
  {
    if(p.path().extension() == ext())
    {
      std::cout << "Processing " << p.filename() << "..." << std::endl;
      std::ifstream master_file(p);
      for (std::string line; std::getline(master_file, line);)
      {
        if (line.find("# ") == 0)
        {
          // skip the comments in the master file.
        }
        else
        {
          // process the *.master file’s settings.
          
          // process the *.master file’s imports.
          
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
