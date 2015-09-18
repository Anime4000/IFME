# Internet Friendly Media Encoder
## Introduction
### History
Back on April 2013, there are no **nVidia ShadowPlay** or even **AMD Gaming Evolve**. Originally developed for compressing FRAPS game recording by using x264, often friends showing that IFME simple and lightweight, from that IFME was born.

IFME using FFmpeg back-end to decode and encode source file.

### Version 5
This latest version very versatile and expandable, rework new GUI, better code. User can create own extension `*.dll` to simplified works such as **AviSynth**

### Version 4
You can find [here](https://github.com/Anime4000/IFME/tree/ifme4) or change `master` to `ifme4`.

## License
IFME Source Code under license [GPL 2.0](http://choosealicense.com/licenses/gpl-2.0/). However you are not allow to sell either in **Source Code** or **Binaries** form.

Artwork drawn by **Ray-en** aka [53C](http://53c.deviantart.com/) under license [Attribution-NonCommercial 4.0 International](http://creativecommons.org/licenses/by-nc/4.0/)

## Donation
Support this project! Even with little penny make this project alive and up-to-date!

You can donate via to [my paypal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U). If you made a donation, don't forget to tell us at [Facebook](https://www.facebook.com/internetfriendlymediaencoder) or  [Twitter](https://twitter.com/Anime4000), You will honored and listed on *Hall of Fame* and **About Program**

## You
### Requirement
#### Windows
* OS: Windows 7, 8, 8.1, 10 (32bit/64bit)
* CPU: Intel Core 2 Duo/AMD AthlonII X2
* RAM: 2GB (4K encoding require 4GB more)
* GPU: Any (OpenCL for FLAC encoder)
* HDD: 256MB
* Internet Connection
* .NET Framework 4.0

#### Linux
* OS: Ubuntu 14.04 or any latest distro (64bit)
* CPU: Intel Core 2 Duo/AMD AthlonII X2
* RAM: 2GB (4K encoding require 4GB more)
* GPU: *none*
* HDD: 256MB
* Internet Connection

### Downloads
You can download it via [SourceForge](https://sourceforge.net/projects/ifme/files/latest/download) or [GitHub](https://github.com/Anime4000/IFME/releases/latest)

## Development
### Windows
#### Compiler
IFME was written in C# 6.0 thus require latest compiler to compile. Do download Visual Studio 2015

#### Prerequisite
Run `prerequisite\deploy.cmd` to download all required files

> `deploy.cmd` scripts require:

> [Download](http://nebm.ist.utl.pt/~glopes/wget/) and extract wget.exe to `\Windows\System32`

> [Download](http://www.7-zip.org/download.html) and install 7-zip

After run `deploy.cmd`, please download these

* [7-Zip Extra](http://www.7-zip.org/download.html) extract `7za.exe` to `prerequisite\windows\32bit` & `x64\7za.exe` to `prerequisite\windows\64bit`
* [MediaInfo CLI](https://mediaarea.net/en/MediaInfo/Download/Windows) download & extract `Mediainfo.dll` 32bit @ `prerequisite\windows\32bit` & 64bit @ `prerequisite\windows\64bit`

#### References
Run `references\download.cmd` to download

* `INIFileParser.dll`
* `MediaInfoDotNet.dll`

#### Debugging
Before start, you need copy these file to `ifme\bin\Debug` to get working.

##### 64bit
* Copy `prerequisite\windows\64bit\Mediainfo.dll`
* Copy `prerequisite\windows\64bit\7za.exe`
* Copy `prerequisite\windows\64bit\plugins\`
And some 32bit stuff
* Copy `prerequisite\windows\32bit\plugins\avisynth\`
* Copy `prerequisite\windows\32bit\plugins\faac\`
* Copy `prerequisite\windows\32bit\plugins\mp4fpsmod\`
* Copy `prerequisite\windows\32bit\plugins\opus\`

##### 32bit
* Copy `prerequisite\windows\32bit\Mediainfo.dll`
* Copy `prerequisite\windows\32bit\7za.exe`
* Copy `prerequisite\windows\32bit\plugins\`

*You may skip copying `plugins` folder, IFME will download before run*

As usual, open `ifme.sln` :+1:

#### Packaging
[Download](http://www.jrsoftware.org/isdl.php) & install Inno Setup and open `installer.iss`

### Linux
#### Compiler
Install Mono 4.0+ [click here to install](http://www.mono-project.com/download/#download-lin). You might download all mono stuff including `-devel`.

#### Prerequisite
Get these package `p7zip-full` and `mediainfo` by entering command `sudo apt-get install p7zip-full mediainfo`. After that run `prerequisite/deploy.sh` to download all required files

#### References
Run `references/download.sh` to download

* `INIFileParser.dll`
* `MediaInfoDotNet.dll`

#### Debugging
Before start, you need copy these file to `ifme/bin/Debug` to get working.

* Copy `prerequisite/linux/64bit/Mediainfo.dll`
* Copy `prerequisite/linux/64bit/7za.exe`
* Copy `prerequisite/linux/64bit/plugins/`

*You may skip copying `plugins` folder, IFME will download before run*

As usual, open `ifme.sln` :+1:
