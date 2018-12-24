# newsmake
News / changelog making tool.

Current: [![Build status](https://ci.appveyor.com/api/projects/status/h93bwvkrh8cfvgde?svg=true)](https://ci.appveyor.com/project/Elskom/newsmake)

Master: [![Build status](https://ci.appveyor.com/api/projects/status/h93bwvkrh8cfvgde/branch/master?svg=true)](https://ci.appveyor.com/project/Elskom/newsmake/branch/master)

To build:

    dotnet build

To install:

    dotnet tool install -g newsmake

Any bugs can be filed in this repository's built in bug tracker.

This is for taking an changelog master config file with folders of
the program's versions and then outputting every entry to those
versions to the configured output file. The result is an perfectly
autogenerated changelog.
