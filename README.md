# newsmake

[![NuGet Badge](https://buildstats.info/nuget/newsmake?includePreReleases=true)](https://www.nuget.org/packages/newsmake/)

News / changelog making tool.

![Build Status](https://github.com/Elskom/newsmake/workflows/.NET%20Core%20%28build%20%26%20publish%20release%29/badge.svg)
![Build Status](https://github.com/Elskom/newsmake/workflows/.NET%20Core/badge.svg)

To build:

    dotnet build

To install:

    dotnet tool install -g newsmake

To update:

    dotnet tool update -g newsmake

To uninstall:

    dotnet tool uninstall -g newsmake


Any bugs can be filed in this repository's built in bug tracker.

This is for taking an changelog master config file with folders of
the program's versions and then outputting every entry to those
versions to the configured output file. The result is an perfectly
autogenerated changelog.
