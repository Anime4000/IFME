# Internet Friendly Media Encoder
## Introduction
Internet Friendly Media Encoder (known as IFME) one x265 GUI encoder that support audio manipulation, subtitle and attachment support. Making user easy to convert their media files to latest format!

Binary (Installer/Archive) can be [found here!](https://sourceforge.net/projects/ifme/)

## License
* Source Code are under [GNU GPL v2](http://choosealicense.com/licenses/gpl-2.0/).
* Mascot, Illustration and graphics under [Creative Common Attribution-ShareAlike 4.0 International](http://creativecommons.org/licenses/by-sa/4.0/).
* For more info, [click here](http://ifme.sourceforge.net/index.html?page/rights.html).

## History
IFME was originally developed for compressing FRAPS game recording by using x264, often friends showing that IFME simple and lightweight, from that IFME was born.


## Contribution
### Translation
IFME will more happy if have multiple language support, allow non-English user can be use!


### Addons
IFME 4.0 support addons/plugins style, every-time IFME starts, always check new version, if available, download and update immediately. You can add your own, by read sample on `addons` folder or read our documentation



## Requirement
### Basics
* IFME only release under 64bit and Windows XP is not supported (due WinXP end it's life).
* IFME using .NET Framework 4.5 (Windows Vista and 7 require to install, meanwhile Windows 8 already build-in).
* IFME need administrator access due changing encoder CPU Priority and Affinity, if installed on Program File, it need write access.


### Prerequisite
IFME require these file to work:

#### Binary
* [MediaInfo (64bit DLL)](http://mediaarea.net/en/MediaInfo/Download/Windows) (used for detecting video and audio properties)
* [7za (Command-line)](http://downloads.sourceforge.net/sevenzip/7za920.zip) (used for download main program updates)

#### Addons
* First, create `addons` folder inside `prerequisite`.
* Get all addons [here](https://sourceforge.net/projects/ifme/files/addons/) and [extract](http://www.7-zip.org/) to `prerequisite` > `addons`, structure will be like this:
![alt text](http://ifme.sourceforge.net/images/preq.png)

* Or you can create your own, example `addons\mp3\addon.ini`.
```
[addon]
type = audio

[profile]
name = MPEG Layer 3 (MP3)
dev = LAME
version = 3.99.5
homepage = http://lame.sf.net
container = mp4

[provider]
name = Gamedude
update = http://ifme.sourceforge.net/update/addons/mp3.txt
download = http://master.dl.sourceforge.net/project/ifme/addons/mp3.ifz

; Please refer to IFME documentation on project page (http://ifme.sf.net)
; {0} for basic command, such as quality or bitrate
; {1} output file, require | in between, it will converted to "
; {2} for input file, also require |
; {3} used for extra command (adv)
[data]
app = lame.exe
cmd = {3} -b {0} |{2}| |{1}.mp3| 
adv = --preset insane
quality = 32,45,64,80,96,112,128,160,192,224,256,320
default = 128
```


## Development
### Language migrating
First IFME was written in VB.NET from version 1.0 until 3.2 and version 4.0 written in C#, completely start from scratch.


### IDE
Using Microsoft VisualStudio 2013 (.NET 4.5)


### Known bugs
Currently IFME compiled under "Debug". x265 encoder has issue with "Release", the symptom is still unknown.


### Debugging
* To make IFME fully working, get `MediaInfo.dll` and `za.dll` (7za.exe renamed) in `ifme.exe` root folder (where `ifme.exe` is located, put there)
* Don't forget about `addons` stuff, put everything in `addons` folder


### Building
Make sure all prerequisite stuff in `prerequisite` folder is fulfill

* If release a latest version, change File and Assembly version for `ifme` and `ifme.hitoha` properties.
* Before start compiling, run `build_clean_bin.cmd` script to remove last compiled.
* Then start compiling.
* Run `build.cmd` to start copy required file (including `addons` inside `prerequisite` folder).
* New folder `_build` created.
* Create an installer by opening `build_installer.iss` script, this require [InnoSetup](http://www.jrsoftware.org/isinfo.php) to be installed