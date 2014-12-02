@echo off

setlocal

set nuget_exe=.nuget/NuGet.exe
set nuspec_file=NDesk.Options.Extensions.nuspec
set packages=*.nupkg

pushd ..

"%nuget_exe%" pack "%nuspec_file%"
::Probably not a great idea to go committing this one on account of apikey spoofing.
::"%nuget_exe%" push "%packages%" -apikey 6d12ec3f-a9b3-44f6-9a29-99b3234b9dbe
::del "%packages%"

popd

endlocal
