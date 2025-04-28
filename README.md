# TLCGen

Application to specify and generate (Dutch) Traffic Light Controller programs.

## Getting TLCGen

The latest stable binary of TLCGen can be downloaded from the CodingConnected website:

https://www.codingconnected.eu/software/tlcgen/

The latest binary does not always contain all functionality present in the current
sources, as may be expected from a stable build.

## Building TLCGen

To build TLCGen, download and install the latest version of Visual Studio (the 
Community edition is perfectly OK). Be sure to select the Windows Desktop 
Applications development tools during installation, since TLCGen is build using
WPF in NET8.

Clone the sources, then install [paket](https://fsprojects.github.io/Paket/get-started.html):

    dotnet new tool-manifest
    dotnet tool install paket
    dotnet tool restore

Now do a restore:

    dotnet restore
    dotnet paket restore

This should restore all needed nuget packages for the entire solution. Build 
the application. If it won't work, try `dotnet paket install` and/or `dotnet paket update`.

## Licensing

TLCGen is provided under the MIT license, please refer to the LICENSE.md file 
for details. Use at your own risk.
