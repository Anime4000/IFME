/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * 
 ******************************************************************************
 * MultiStreamCommon.cs
 * 
 * Presents information common to more than one stream type.
 * 
 ******************************************************************************
 */

using System;
using MediaInfoLib;
using System.Collections.Generic;

namespace MediaInfoDotNet.Models
{
	/// <summary>Functionality common to more than one stream type.</summary>
	public class MultiStreamCommon : Media
	{
		/// <summary>MultiStreamCommon constructor.</summary>
		///<param name="mediaInfo">A MediaInfo object.</param>
		/// <param name="kind">A MediaInfo StreamKind.</param>
		///<param name="id">The MediaInfo ID for this audio stream.</param>
		public MultiStreamCommon(MediaInfo mediaInfo, StreamKind kind, int id)
			: base(mediaInfo, id) {
			this.kind = kind;
		}

		#region AllStreamsCommon
		string _format;
		///<summary>The format or container of this file or stream.</summary>
		public string format {
			get {
				if(_format == null)
					_format = miGetString("Format");
				return _format;
			}
		}

		string _title;
		///<summary>The title of this stream.</summary>
		public string title {
			get {
				if(_title == null)
					_title = miGetString("Title");
				return _title;
			}
		}

		string _uniqueId;
		///<summary>This stream's globally unique ID (GUID).</summary>
		public string uniqueId {
			get {
				if(_uniqueId == null)
					_uniqueId = miGetString("UniqueID");
				return _uniqueId;
			}
		}
		int _Id;
		///<summary>This stream's position and sequence ID.</summary>
		public int Id {
			get {
			if (_Id == 0)
				_Id = miGetInt("ID");
			return _Id;
			}
		}
		#endregion

		#region GeneralVideoAudioTextImageCommon
		DateTime _encodedDate = DateTime.MinValue;
		///<summary>Date and time stream encoding completed.</summary>
		public DateTime encodedDate {
			get {
				if(_encodedDate == DateTime.MinValue)
					_encodedDate = miGetDateTime("Encoded_Date");
				return _encodedDate;
			}
		}

		string _encoderLibrary = null;
		///<summary>Software used to encode this stream.</summary>
		public string encoderLibrary {
			get {
				if(_encoderLibrary == null)
					_encoderLibrary = miGetString("Encoded_Library");
				return _encoderLibrary;
			}
		}

		string _internetMediaType = null;
		///<summary>Media type of stream, formerly called MIME type.</summary>
		public string internetMediaType {
			get {
				if(_internetMediaType == null)
					_internetMediaType = miGetString("InternetMediaType");
				return _internetMediaType;
			}
		}

		long _size = long.MinValue;
		///<summary>Size in bytes.</summary>
		public long size {
			get {
				if(_size == long.MinValue) {
					if(kind == StreamKind.General)
						_size = miGetLong("FileSize");
					else
						_size = miGetLong("StreamSize");
				}
				return _size;
			}
		}

		string _encoderSettingsRaw = null;
		///<summary>Encoder settings used for encoding this stream.
		///String format: name=value / name=value / ...</summary>
		public string encoderSettingsRaw {
			get {
				if(_encoderSettingsRaw == null)
					_encoderSettingsRaw
						= miGetString("Encoded_Library_Settings");
				return _encoderSettingsRaw;
			}
		}

		IDictionary<string, string> _encoderSettings = null;
		///<summary>Encoder settings used for encoding this stream.</summary>
		public IDictionary<string, string> encoderSettings {
			get {
				if(_encoderSettings == null) {
					_encoderSettings = new Dictionary<string, string>();
					String settings = encoderSettingsRaw;
					foreach(var setting in settings.Split('/')) {
						var keyValue = setting.Trim().Split('=');
						if(keyValue.Length == 2) {
							if(!_encoderSettings.ContainsKey(keyValue[0]))
								_encoderSettings.Add(keyValue[0], keyValue[1]);
						}
					}
				}
				return _encoderSettings;
			}
		}
		#endregion

		#region GeneralVideoAudioTextImageMenuCommon
		string _codecId = null;
		///<summary>Codec ID available from some codecs.</summary>
		///<example>AAC audio:A_AAC, h.264 video:V_MPEG4/ISO/AVC</example>
		public string codecId {
			get {
				if(_codecId == null)
					_codecId = miGetString("CodecID");
				return _codecId;
			}
		}

		string _codecCommonName = null;
		///<summary>Common name of the codec.</summary>
		public string codecCommonName {
			get {
				if(_codecCommonName == null)
					_codecCommonName = miGetString("CodecID/Hint");
				return _codecCommonName;
			}
		}
		#endregion

		#region GeneralVideoAudioTextMenu
		int _delay = int.MinValue;
		///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
		public int delay {
			get {
				if(_delay == int.MinValue)
					_delay = miGetInt("Delay");
				return _delay;
			}
		}

		int _duration = int.MinValue;
		///<summary>Duration of the stream in milliseconds.</summary>
		public int duration {
			get {
				if(_duration == int.MinValue)
					_duration = miGetInt("Duration");
				return _duration;
			}
		}
		#endregion

		#region VideoAudioTextCommon
		int _bitRate = int.MinValue;
		///<summary>The bit rate of this stream, in bits per second</summary>
		public int bitRate {
			get {
				if(_bitRate == int.MinValue)
					_bitRate = miGetInt("BitRate");
				return _bitRate;
			}
		}

		int _bitRateMaximum = int.MinValue;
		///<summary>The maximum bitrate of this stream in BPS.</summary>
		public int bitRateMaximum {
			get {
				if(_bitRateMaximum == int.MinValue)
					_bitRateMaximum = miGetInt("BitRate_Maximum");
				return _bitRateMaximum;
			}
		}

		int _bitRateMinimum = int.MinValue;
		///<summary>The minimum bitrate of this stream in BPS.</summary>
		public int bitRateMinimum {
			get {
				if(_bitRateMinimum == int.MinValue)
					_bitRateMinimum = miGetInt("BitRate_Minimum");
				return _bitRateMinimum;
			}
		}

		int _bitRateNominal = int.MinValue;
		///<summary>The maximum allowed bitrate, in BPS, with the encoder
		/// settings used. Some encoders report the average BPS.</summary>
		public int bitRateNominal {
			get {
				if(_bitRateNominal == int.MinValue)
					_bitRateNominal = miGetInt("BitRate_Nominal");
				return _bitRateNominal;
			}
		}

		string _bitRateMode = null;
		///<summary>Mode (CBR, VBR) used for bit allocation.</summary>
		public string bitRateMode {
			get {
				if(_bitRateMode == null)
					_bitRateMode = miGetString("BitRate_Mode");
				return _bitRateMode;
			}
		}

		string _muxingMode = null;
		///<summary>How the stream is muxed into the container.</summary>
		public string muxingMode {
			get {
				if(_muxingMode == null)
					_muxingMode = miGetString("MuxingMode");
				return _muxingMode;
			}
		}

		int _frameCount = int.MinValue;
		///<summary>The total number of frames (e.g. video frames).</summary>
		public int frameCount {
			get {
				if(_frameCount == int.MinValue)
					_frameCount = miGetInt("FrameCount");
				return _frameCount;
			}
		}

		float _frameRate = float.MinValue;
		///<summary>Frame rate of the stream in frames per second.</summary>
		public float frameRate {
			get {
				if(_frameRate == float.MinValue)
					_frameRate = miGetFloat("FrameRate");
				return _frameRate;
			}
		}

		float _frameRateOri = float.MinValue;
		/// <summary>Original frame rate of the stream in frames per second.</summary>
		public float frameRateOri{
			get	{
				if (_frameRateOri == float.MinValue)
					_frameRateOri = miGetFloat("FrameRate_Original");
				return _frameRateOri;
			}
		}

		float _frameRateNom = float.MinValue;
		/// <summary>Nominal frame rate of the stream in frames per second.</summary>
		public float frameRateNom {
			get	{
				if (_frameRateNom == float.MinValue)
					_frameRateNom = miGetFloat("FrameRate_Nominal");
				return _frameRateNom;
			}
		}

		float _frameRateMin = float.MinValue;
		/// <summary>Minimum frame rate of the stream in frames per second.</summary>
		public float frameRateMin {
			get	{
				if (_frameRateMin == float.MinValue)
					_frameRateMin = miGetFloat("FrameRate_Minimum");
				return _frameRateMin;
			}
		}

		float _frameRateMax = float.MinValue;
		/// <summary>Maximum frame rate of the stream in frames per second.</summary>
		public float frameRateMax {
			get {
				if (_frameRateMax == float.MinValue)
					_frameRateMax = miGetFloat("FrameRate_Maximum");
				return _frameRateMax;
			}
		}
		#endregion

		#region VideoAudioTextImageCommon
		string _compressionMode = null;
		///<summary>Compression mode (lossy or lossless).</summary>
		public string compressionMode {
			get {
				if(_compressionMode == null)
					_compressionMode = miGetString("Compression_Mode");
				return _compressionMode;
			}
		}

		string _compressionRatio = null;
		///<summary>Ratio of current size to uncompressed size.</summary>
		public string compressionRatio {
			get {
				if(_compressionRatio == null)
					_compressionRatio = miGetString("Compression_Ratio");
				return _compressionRatio;
			}
		}

		int _bitDepth = int.MinValue;
		///<example>Stream bit depth (16, 24, 32...)</example>
		public int bitDepth {
			get {
				if(_bitDepth == int.MinValue)
					_bitDepth = miGetInt("BitDepth");
				if(_bitDepth == 0)
					_bitDepth = 8;
				return _bitDepth;
			}
		}
		#endregion

		#region VideoAudioTextImageMenuCommon
		string _language = null;
		///<summary>2-letter (if available) or 3-letter ISO code.</summary>
		public string language {
			get {
				if(_language == null)
					_language = miGetString("Language");
				return _language;
			}
		}

		string _languageThree = null;
		///<summary>3-letter ISO 639-2 if exists, else empty.</summary>
		public string languageThree
		{
			get
			{
				if (_languageThree == null)
					_languageThree = miGetString("Language/String3");
				return _languageThree;
			}
		}
		#endregion

		#region VideoImageCommon
		float _pixelAspectRatio = float.MinValue;
		///<summary>Ratio of pixel width to pixel height.</summary>
		public float pixelAspectRatio {
			get {
				if(_pixelAspectRatio == float.MinValue)
					_pixelAspectRatio = miGetFloat("PixelAspectRatio");
				return _pixelAspectRatio;
			}
		}

		string _scanType = null;
		/// <summary>Scan type of source video either Progressive or Interlaced</summary>
		public string scanType {
			get {
				if (_scanType == null)
					_scanType = miGetString("ScanType");
				return _scanType;
			}
		}

		string _scanOder = null;
		/// <summary>Scan order either Top Field First or Bottom Field First</summary>
		public string scanOrder {
			get {
				if (_scanOder == null)
					_scanOder = miGetString("ScanOrder");
				return _scanOder;
			}
		}

		string _colorspace = null;
		/// <summary>Color Space such as YUV, RGB</summary>
		public string colorSpace {
			get {
				if (_colorspace == null)
					_colorspace = miGetString("ColorSpace");
				return _colorspace;
			}
		}

		string _chromasubsample = null;
		/// <summary>Chroma Subsample such as 4:2:0, 4:2:2, 4:4:4</summary>
		public string chromaSubSampling {
			get {
				if (_chromasubsample == null)
					_chromasubsample = miGetString("ChromaSubsampling");
				return _chromasubsample;
			}
		}
		#endregion

		#region VideoTextCommon
		string _frameRateMode = null;
		///<summary>Frame rate mode (CFR, VFR) of stream.</summary>
		public string frameRateMode {
			get {
				if(_frameRateMode == null)
					_frameRateMode = miGetString("FrameRate_Mode");
				return _frameRateMode;
			}
		}
		#endregion

		#region VideoTextImageCommon
		int _height = int.MinValue;
		///<summary>Height in pixels.</summary>
		public int height {
			get {
				if(_height == int.MinValue)
					_height = miGetInt("Height");
				return _height;
			}
		}

		int _width = int.MinValue;
		///<summary>Width in pixels.</summary>
		public int width {
			get {
				if(_width == int.MinValue)
					_width = miGetInt("Width");
				return _width;
			}
		}

		// Darrell Turner submitted the code for originalHeight and originalWidth
		int _originalheight = int.MinValue;
		///<summary>Original Height in pixels. For use with, e.g. ProRes files with aperture set.</summary>
		public int originalHeight {
			get {
				if(_originalheight == int.MinValue)
					_originalheight = miGetInt("Height_Original");
				return _originalheight;
			}
		}

		// Darrell Turner submitted the code for originalHeight and originalWidth
		int _originalwidth = int.MinValue;
		///<summary>Original Width in pixels. For use with, e.g. ProRes files with aperture set.</summary>
		public int originalWidth {
			get {
				if(_originalwidth == int.MinValue)
					_originalwidth = miGetInt("Width_Original");
				return _originalwidth;
			}
		}

		#endregion
	}
}
