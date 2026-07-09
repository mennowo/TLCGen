# TLCGen

Application to specify and generate (Dutch) Traffic Light Controller programs.

## Getting TLCGen

The latest stable binary of TLCGen can be downloaded from the CodingConnected website:

https://www.codingconnected.eu/software/tlcgen/

The latest binary does not always contain all functionality present in the current
sources, as may be expected from a stable build.

## Building TLCGen

To build TLCGen, download and install Visual Studio 2026 (aka. 'Insiders'; the Community 
edition is perfectly OK). Be sure to select the Windows Desktop Applications development 
tools during installation, since TLCGen is build using WPF in NET10.

Clone the sources, then do a restore:

    dotnet restore

This should restore all needed nuget packages for the entire solution, since we are
using global package management. After restore completes, build the application.

## Licensing

TLCGen is provided under the MIT license, please refer to the LICENSE.md file 
for details. Use at your own risk.

### Licensing for building TLCGen.Setup

Note that with version 7 of WiX, FireGiant changed the licensing conditions for 
the WiX Toolset. If you want to build the TLCGen.Setup installer, you are obliged
to contribute to the WiX Toolset project. Please refer to the WiX Toolset repository 
for details on how to contribute. If you do not want to contribute, you can still 
build TLCGen, but you are not legally allowed to build and use the installer.
