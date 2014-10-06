# Internet Friendly Media Encoder
## Introduction
### History
Back on April 2013, there are no nVidia ShadowPlay or even AMD Gaming Evolve. Originally developed for compressing FRAPS game recording by using x264, often friends showing that IFME simple and lightweight, from that IFME was born.

IFME using FFmpeg back-end to decode and encode source file.

### License
IFME using [GNU GPL v2](http://choosealicense.com/licenses/gpl-2.0/). for both `Source Code` and `Artwork`, for more info, [click here](http://ifme.sourceforge.net/index.html?page/rights.html).

### Donation
Support this project! Even with little penny make this project alive and up-to-date!

You can donate via to [my paypal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U). If you made a donation, don't forget to tell us at [Facebook](https://www.facebook.com/internetfriendlymediaencoder) or  [Twitter](https://twitter.com/Anime4000), You will honored and listed on *Hall of Fame*

### Requirement
#### Windows
* 64bit Windows XP, Vista, 7, 8, 8.1, 10
* .NET Framework 4.0

#### Linux
* Ubuntu 14, Linux Mint 17, Kali Linux 1.0.8, any 64bit Linux Desktop with *Multimedia Support*
* Latest Mono Runtime (`apt-get install mono-complete`)
* Latest MediaInfo (`apt-get install mediainfo`)
* Latest libavformat (`apt-get install libavformat-dev`) *ffmsindex*
* Latest libswscale (`apt-get install libswscale-dev`) *ffmsindex*
* Latest libboost (`apt-get install libboost-all-dev`) *mkvtoolnix*

### Uses
Internet Friendly Media Encoder (known as IFME) one x265 GUI encoder that support audio manipulation, subtitle and attachment support.

Making user easy to convert their media files and standardise their collection to the latest format!

IFME capable detect interlace video by **detecting metadata** and automatically de-interlaced as is. Sometime interlace video can be very frustrated if improper encoding, If you backup DVD, [MakeMKV](http://makemkv.com/) capable to keep interlace data that IFME can de-interlaced properly.

De-interlaced quality follow by video configuration preset:

| x265 preset | De-interlaced | Value |
| :---------: | :-----------: | :---: |
| ultrafast   | fast          | 0     |
| superfast   |               |       |
| veryfast    |               |       |
| faster      | medium        | 1     |
| fast        |               |       |
| medium      |               |       |
| slow        | slow          | 2     |
| slower      |               |       |
| veryslow    |               |       |
| placebo     | extra_slow    | 3     |

IFME is a queue based converter, in order easy to use, IFME will keep original properties, such as:

* Video Resolution
* Video Bit Depth
* Video Frame Rate value
* Video Frame Rate mode (CFR/VFR)
* MKV Subtitle
* MKV Attachment
* MKV Chapters

### Downloads
Binary (Installer/Archive) can be [found here!](http://ifme.sourceforge.net/?page/download.html)

## Contribution
### Fork
If your fork behind, follow [this guide](https://help.github.com/articles/syncing-a-fork) to update your repo.

### Translation
IFME will more happy if have multiple language support, allow non-English user can be use!

* Fork `ifme` if you not yet or sync.
* Make a copy `eng.ini` or any file that you can refer with.
* Change `iso` code, please refer language code [here](http://en.wikipedia.org/wiki/List_of_ISO_639-2_codes).
* Merge request. :green_heart:


### Addons
IFME 4.0 support addons/plugins style, every-time IFME starts, always check new version, if available, download and update immediately. You can [add your own](https://github.com/Anime4000/IFME/blob/master/ifme/addons/null/addon.ini), by read sample on `addons` folder or read our documentation

### Prerequisite
#### Binary (Windows)
Put these at `prerequisite\windows` folder:

* [MediaInfo (64bit DLL)](http://mediaarea.net/en/MediaInfo/Download/Windows) (used for detecting video and audio properties)
* [7za (Command-line)](http://downloads.sourceforge.net/sevenzip/7za920.zip) (used for extract stuff)
* [wget (Command-Line](https://osspack32.googlecode.com/files/wget-1.14.exe) (used for download updates)

#### Binary (Linux)
Extract these at `prerequisite/linux` folder:

* [libgpac.so.3 (64bit)](http://gpac.wp.mines-telecom.fr/downloads/gpac-nightly-builds/#Linux%20x86%2064%20bits)
* [libmozjs185.so.1.0](http://rpm.pbone.net/index.php3/stat/4/idpl/18522795/dir/opensuse_12.x/com/libmozjs185-1_0-32bit-1.8.5-9.2.2.x86_64.rpm.html)
* 7za (command-line) *download via `apt-get` and copy from `/usr/lib/p7zip/7za`*

#### Addons
* First, create `addons` folder inside `prerequisite` either `windows` or `linux`.
* Get all addons [here (windows)](https://sourceforge.net/projects/ifme/files/addons/) or [here (linux)](https://sourceforge.net/projects/ifme/files/addons/linux) and [extract](http://www.7-zip.org/).


## Development
### Supported IDE
* Microsoft VisualStudio 2012/2013
* MonoDevelop/Xamarin Studio


### Debugging
* To make IFME fully working, get `MediaInfo` and `unpack` (`7za` renamed) in root folder (where `ifme` is located)
* Don't forget about `addons` stuff, put everything in `addons` folder


### Release
#### Windows
Make sure all prerequisite stuff in `prerequisite\windows` folder is fulfil

* If release a latest version, change File and Assembly version for `ifme` properties.
* Run `make.cmd` to start compile (require MSBuild 12.0, this included via Visual Studio 2012/2013)
* New folder `_build` will created.
* Create an installer by opening `installer.iss` script, this require [InnoSetup](http://www.jrsoftware.org/isinfo.php) to be installed

#### Linux
Make sure all prerequisite stuff in `prerequisite/linux` folder is fulfil

* If release a latest version, change File and Assembly version for `ifme` properties.
* Run `make.sh` to start compile (require mono-xbuild)
* New folder `_build` will created.
