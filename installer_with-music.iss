[Files]
Source: "..\installer\Bass.dll"; Flags: dontcopy
Source: "..\installer\AudioFile.mp3"; Flags: dontcopy
; For installer music

[Files]
Source: "..\installer\image_background.bmp"; Flags: dontcopy
; For installer background

[Code]
// Ref: http://stackoverflow.com/questions/12359859/playing-sound-during-an-inno-setup-install
const  
  BASS_SAMPLE_LOOP = 4;
  BASS_UNICODE = $80000000;
  BASS_CONFIG_GVOL_STREAM = 5;
const
  #ifndef UNICODE
    EncodingFlag = 0;
  #else
    EncodingFlag = BASS_UNICODE;
  #endif
type
  HSTREAM = DWORD;

function BASS_Init(device: LongInt; freq, flags: DWORD; 
  win: HWND; clsid: Cardinal): BOOL;
  external 'BASS_Init@files:bass.dll stdcall';
function BASS_StreamCreateFile(mem: BOOL; f: string; offset1: DWORD; 
  offset2: DWORD; length1: DWORD; length2: DWORD; flags: DWORD): HSTREAM;
  external 'BASS_StreamCreateFile@files:bass.dll stdcall';
function BASS_ChannelPlay(handle: DWORD; restart: BOOL): BOOL; 
  external 'BASS_ChannelPlay@files:bass.dll stdcall';
function BASS_SetConfig(option: DWORD; value: DWORD ): BOOL;
  external 'BASS_SetConfig@files:bass.dll stdcall';
function BASS_Free: BOOL;
  external 'BASS_Free@files:bass.dll stdcall';

// Background, ref: http://www.vincenzo.net/isxkb/index.php?title=Background_image_during_the_installation
function GetSystemMetrics(nIndex:Integer):Integer;
external 'GetSystemMetrics@user32.dll stdcall'; //end

procedure InitializeWizard;

var
  StreamHandle: HSTREAM;

  // Background code
  width,height: Integer;
  BackgroundBitmapImage: TBitmapImage;
  s: string; // end
begin
  ExtractTemporaryFile('AudioFile.mp3');
  if BASS_Init(-1, 44100, 0, 0, 0) then
  begin
    StreamHandle := BASS_StreamCreateFile(False, 
      ExpandConstant('{tmp}\AudioFile.mp3'), 0, 0, 0, 0, 
      EncodingFlag or BASS_SAMPLE_LOOP);
    BASS_SetConfig(BASS_CONFIG_GVOL_STREAM, 10000);
    BASS_ChannelPlay(StreamHandle, False);
  end;

  // Background code
  ExtractTemporaryFile('image_background.bmp');
  s:=ExpandConstant('{tmp}')+'\image_background.bmp';
  WizardForm.Position:=poScreenCenter;
  MainForm.BORDERSTYLE:=bsNone;
  width:=GetSystemMetrics(0);
  height:=GetSystemMetrics(1);
  MainForm.Width:=width;
  MainForm.Height:=height;
  width:=MainForm.ClientWidth;
  height:=MainForm.ClientHeight;
  MainForm.Left := 0;
  MainForm.Top := 0;

  BackgroundBitmapImage := TBitmapImage.Create(MainForm);
  BackgroundBitmapImage.Bitmap.LoadFromFile(s);
  BackgroundBitmapImage.Align := alClient;
  BackgroundBitmapImage.Parent := MainForm;
  BackgroundBitmapImage.Stretch :=True;
  MainForm.Visible:=True; // end
end;

procedure DeinitializeSetup;
begin
  BASS_Free;
end;