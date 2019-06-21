cd C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Release
rmdir /s /q packed
mkdir packed
cd packed
mkdir Libs
mkdir Deps
cd ..
del /q Xceed.Wpf.AvalonDock.dll
del /q Xceed.Wpf.AvalonDock.Themes.Aero.dll
del /q Xceed.Wpf.AvalonDock.Themes.Metro.dll
del /q Xceed.Wpf.AvalonDock.Themes.VS2010.dll
del /q Xceed.Wpf.DataGrid.dll
copy TLCGen*.exe .\packed\
copy TLCGen*.exe.config .\packed\
copy TLCGen*.dll .\packed\Libs
copy TLCGen*.dll.config .\packed\Libs
copy *.dll .\packed\Deps
xcopy Plugins .\packed\Plugins\ /E
xcopy Docs .\packed\Docs\ /E
xcopy Settings .\packed\Settings\ /E
xcopy SourceFiles .\packed\SourceFiles\ /E
xcopy SourceFilesToCopy .\packed\SourceFilesToCopy\ /E
xcopy Updater .\packed\Updater\ /E
C:\Users\menno\CodingConnected\Repos\TLCGen\Tools\TLCGen.FolderVersionRenamer\bin\Release\TLCGen.FolderVersionRenamer.exe --tlcgen-folder "C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Release\packed" --target-folder "C:\Users\menno\CodingConnected\Repos\TLCGen\published"
rmdir /s /q packed