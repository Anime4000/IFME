# Internet Friendly Media Encoder
## Introduction
### History
Back on April 2013, there are no **nVidia ShadowPlay** or even **AMD Gaming Evolve**. Originally developed for compressing FRAPS game recording by using x264, often friends showing that IFME simple and lightweight, from that IFME was born.

IFME using FFmpeg back-end to decode and encode source file.

### Version 5
This latest version very versatile and expandable, rework new GUI, better code. User can create own extension `*.dll` to simplified works such as **AviSynth**

### Version 4
You can find [here](https://github.com/Anime4000/IFME/tree/ifme4) or change `master` to `ifme4`.

### Version 3
The source code never publish online, and written in VB.NET

## License
IFME Source Code under license [GPL 2.0](http://choosealicense.com/licenses/gpl-2.0/).<br>However you are not allow to sell either in **Source Code** or **Binaries** form.

Artwork drawn by **Ray-en** aka [53C](http://53c.deviantart.com/) under license [Attribution-NonCommercial 4.0 International](http://creativecommons.org/licenses/by-nc/4.0/)

## Donation
Support this project! Even with little penny make this project alive and up-to-date!

You can donate via to [my paypal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U). If you made a donation, don't forget to tell us at [Facebook](https://www.facebook.com/internetfriendlymediaencoder) or  [Twitter](https://twitter.com/Anime4000), You will honored and listed on *Hall of Fame* and **About Program**

## You
### Downloads
You can download it via [SourceForge](https://sourceforge.net/projects/ifme/files/latest/download) or [GitHub](https://github.com/Anime4000/IFME/releases/latest)

### Requirement
#### Windows
* OS: Windows 7, 8, 8.1, 10 (32bit/64bit)
* CPU: Intel Core 2 Duo/AMD AthlonII X2
* RAM: 2GB (4K encoding require 4GB more)
* GPU: Any (OpenCL for FLAC encoder)
* HDD: 256MB
* .NET Framework 4.0
* Internet Connection (for update)

#### Linux
* OS: Ubuntu 14.04, Linux Mint or any latest distro (64bit)
* CPU: Intel Core 2 Duo/AMD AthlonII X2
* RAM: 2GB (4K encoding require 4GB more)
* GPU: *none*
* HDD: 256MB
* Mono `mono-complete`
* Internet Connection (for update)

## Development
### Windows
#### Compiler
IFME was written in C# 6.0 thus require latest compiler to compile. Do download Visual Studio 2015

#### Getting Ready
Before proceed, please get these required item to work

* [Download](https://go.microsoft.com/fwlink/?LinkId=532606&clcid=0x409) Microsoft Visual Studio 2015
* [Download](http://www.jrsoftware.org/isdl.php) Inno Setup (Unicode)
* [Download](http://nebm.ist.utl.pt/~glopes/wget/) wget.exe to `\Windows\System32`
* [Download](http://www.7-zip.org/download.html) Install 7-zip

Execute `configure.cmd` script to check and download all required items.

Execute `Makefile.cmd` script to build and create installer

### Linux
#### Compiler
Install Mono 4.0+ [click here to install](http://www.mono-project.com/download/#download-lin). You might download all mono stuff including `-devel`.

#### Getting Ready
Before proceed, please prepared these item to work

* [Mono 4.0+](http://www.mono-project.com/download/#download-lin) *C# 6.0 compatible*
* MediaInfo `sudo apt-get install mediainfo`
* 7-zip `sudo apt-get install p7zip-full`
* A working `wget`

Execute `./configure` to check and download all required item to work.

Execute `make` to build and packaging, `make clean` to clean.
