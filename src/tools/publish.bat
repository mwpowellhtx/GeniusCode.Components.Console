@echo off

setlocal

set nuget_exe=.nuget/NuGet.exe
set package_id=NDesk.Options.Extensions
set nuspec_file=%package_id%.nuspec
::TODO: TBD: packages?
set packages=*.nupkg
set version=%1
set verbosity=%2
set apikey=6d12ec3f-a9b3-44f6-9a29-99b3234b9dbe

::http://docs.nuget.org/docs/reference/command-line-reference

pushd ..

if "%verbosity%"=="" set verbosity=quiet

if "%version%" neq "" "%nuget_exe%" delete %package_id% %version% -NoPrompt -ApiKey %apikey% -Verbosity %verbosity%

"%nuget_exe%" pack "%nuspec_file%"

::Probably not a great idea to go committing this one on account of apikey spoofing.
"%nuget_exe%" push "%packages%" -ApiKey %apikey% -Verbosity %verbosity%

del "%packages%"

popd

endlocal
