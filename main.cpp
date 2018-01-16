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
      // process the *.master file settings.
      
      // process the *.master file’s imports.
      
      // close *.master file.
      master_file.close();
      // write data to the output file set in the *.master file.
    }
  }
  return 0;
}
