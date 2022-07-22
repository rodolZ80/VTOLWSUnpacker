# VTOL VR Workshop Unpacker
Tool to unpack scenario files, campaigns, maps or images, generated when publishing these elements in the Steam Workshop.
This tool is able to unpack .vtsb, .vtcb, .vtmb, .pngb and .jpgb files.
# Requeriments
The tool has been developed in .Net Core 3.1, you will need the ".NET Runtime 3.1.x" to run it, you can download it for different operating systems at https://dotnet.microsoft.com/en-us/download/dotnet/3.1.
# Build

## Windows
```dotnet publish --configuration Release --framework netcoreapp3.1 --runtime win-x64 --self-contained false .\VTOLWSUnpacker.csproj```
## Linux 
```dotnet publish --configuration Release --framework netcoreapp3.1 --runtime linux-x64 --self-contained false .\VTOLWSUnpacker.csproj```
## MacOs
``` dotnet publish --configuration Release --framework netcoreapp3.1 --runtime osx.10.11-x64 --self-contained false .\VTOLWSUnpacker.csproj```

# How to unpack
- The elements downloaded from the Steam Workshop are stored in DRIVE:\SteamLibrary\steamapps\workshop\content\xxxx (667970 for VTOL VR).
- In order to edit the files later you must move them to the corresponding folders in VTOL VR.
- You can unpack a file or a complete folder.
- To use the tool: 
```
./VTOLWSUnpacker <path>

```
**path:** Path of the file or folder you want to unpack, the original packed files will be removed so that there are no conflicts in VTOL VR.

# License
This software does not have any kind of license, but it has been made for learning purposes, don't be an idiot and use it to copy other people's work and then attribute it to you.

Enjoy [VTOL VR!](https://store.steampowered.com/app/667970/VTOL_VR/) 