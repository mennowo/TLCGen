cd TLCGen
cd bin
cd Release
del /q Xceed.Wpf.AvalonDock.dll
del /q Xceed.Wpf.AvalonDock.Themes.Aero.dll
del /q Xceed.Wpf.AvalonDock.Themes.Metro.dll
del /q Xceed.Wpf.AvalonDock.Themes.VS2010.dll
del /q Xceed.Wpf.DataGrid.dll
rmdir /s /q Deps
rmdir /s /q Libs
rmdir /s /q de
rmdir /s /q es
rmdir /s /q fr
rmdir /s /q hu
rmdir /s /q it
rmdir /s /q pt-BR
rmdir /s /q ro
rmdir /s /q ru
rmdir /s /q sv
rmdir /s /q zh-Hans
mkdir Libs
move TLCGen*.dll .\Libs
move TLCGen*.dll.config .\Libs
mkdir Deps
move *.dll .\Deps
del /q *.pdb
del /q *.xml
pause