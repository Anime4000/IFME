# Prerequisite
This place for all required files for IFME to works.

## Get
### 7-zip
#### Windows
You can get standalone [here](http://www.7-zip.org/download.html) and place it to:

* Windows 32bit `windows\32bit\7za.exe`
* Windows 64bit `windows\64bit\7za.exe`

#### Linux
for Linux, you need install `p7zip-full` via `apt-get`. No need place into this folder, `compile.sh` will copy from your `\usr` folder.

### MediaInfo
#### Windows
You can get `DLL` version [here](https://mediaarea.net/en/MediaInfo/Download/Windows) depending on your bit.

* Windows 32bit `windows\32bit\MediaInfo.dll`
* Windows 64bit `windows\64bit\MediaInfo.dll`

#### Linux
for Linux , you need install `mediainfo` via `apt-get`. No need place into this folder, `compile.sh` will copy from your `\usr` folder.

### Plugins
You can get all [here](https://sourceforge.net/projects/ifme/files/plugins/) depending on your OS and bit.

* Windows 32bit `windows\32bit\plugins\*`
* Windows 64bit `windows\64bit\plugins\*`
* Linux 64bit `linux/64bit/plugins/*`

## Deployment
Simply execute `deploy.cmd` or `deploy.sh`, it will do all *plugins* works.