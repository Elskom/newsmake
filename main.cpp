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
    // look for an *.master file.
    if(p.path().extension() == ext())
    {
      // send processing message to user.
      std::cout << "Processing " << p.filename() << "..." << std::endl;
      // open *.master file.
      std::ifstream master_file(p);
      for (std::string line; std::getline(master_file, line);)
      {
        // process the *.master file settings.
        
        // process the *.master file’s imports.
        
        // open the imported files in the folder name of the import.
        // get imported files data.
        // close imported files.
        // write data to the output file set in the *.master file.
      }
      // close *.master file.
      master_file.close();
    }
  }
  return 0;
}
