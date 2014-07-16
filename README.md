# Internet Friendly Media Encoder
## Introduction
Internet Friendly Media Encoder (known as IFME) one x265 GUI encoder that support audio manipulation, subtitle and attachment support. Making user easy to convert their media files to latest format!

Binary (installer/Archive) can be [found here!](https://sourceforge.net/projects/ifme/)


## History
IFME was originally developed for compressing FRAPS game recording by using x264, often friends showing that IFME simple and lightweight, from that IFME was born


## Requirement
### Basics
IFME only release under 64bit and Windows XP is not supported (due WinXP end it's life)

IFME using .NET Framework 4.5 (Windows Vista and 7 require to install, meanwhile Windows 8 already build-in)

IFME need administrator access due changing encoder CPU Priority and Affinity, if installed on Program File folder, it need write access.


### Other related
IFME require these file to work:
* MediaInfo.dll (used for detecting video and audio properties)
* 7za.exe (used for download main program updates)


## Contribution
### Translation
IFME will more happy if have multiple language support, allow non-English user can be use!


### Addons
IFME 4.0 support addons/plugins style, every-time IFME starts, always check new version, if available, download and update immediately. You can add your own, by read sample on `addons` folder or read our documentation


## Development
### Languag migrating
First IFME was written in VB.NET from version 1.0 until 3.2 and version 4.0 written in C#, completely start from scratch.


### IDE
Using Microsoft VisualStudio 2013 (.NET 4.5)


### Known bugs
Currently IFME compiled under "Debug". x265 encoder has issue with "Release", the symptom is still unknown.