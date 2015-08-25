/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * 
 ******************************************************************************
 *
 * GeneralStream.cs
 * 
 * Presents information and functionality at the file-level.
 * 
 ******************************************************************************
 */

using System;
using MediaInfoLib;
using MediaInfoDotNet.Models;
using System.Collections.Generic;
using System.IO;

namespace MediaInfoDotNet.Models
{
	///<summary>For inheritance by classes representing media files.</summary>
	public abstract class GeneralStream : Media
	{
		readonly MultiStreamCommon streamCommon;

		///<summary>GeneralStream constructor.</summary>
		///<param name="filePath">Complete path and name of a file.</param>
		public GeneralStream(string filePath)
			: base(filePath) {
			kind = StreamKind.General;
			streamCommon = new MultiStreamCommon(mediaInfo, kind, id);
		}

		/// <summary>Constructor to create a MediaInfo stream using a .NET System.IO.Stream instead of a file.</summary>
		/// <param name="stream">A System.IO.Stream associated with the media data.
		/// Not to be confused with the MediaInfo stream types like "ImageStream" or "AudioStream"</param>
		/// <param name="chunkSize">The size, in bytes, to read at any one time from the stream.</param>
		public GeneralStream(Stream stream, int chunkSize)
			: base(stream, chunkSize) {
			kind = StreamKind.General;
			streamCommon = new MultiStreamCommon(mediaInfo, kind, id);
		}


		#region AllStreamsCommon
		///<summary>The format or container of this file or stream.</summary>
		public string format { get { return streamCommon.format; } }

		///<summary>The title of this stream.</summary>
		public string title { get { return streamCommon.title; } }

		///<summary>This stream's globally unique ID (GUID).</summary>
		public string uniqueId { get { return streamCommon.uniqueId; } }
		#endregion

		#region GeneralVideoAudioTextImageCommon
		///<summary>Date and time stream encoding completed.</summary>
		public DateTime encodedDate { get { return streamCommon.encodedDate; } }

		///<summary>Software used to encode this stream.</summary>
		public string encodedLibrary { get { return streamCommon.encoderLibrary; } }

		///<summary>Media type of stream, formerly called MIME type.</summary>
		public string internetMediaType { get { return streamCommon.internetMediaType; } }

		///<summary>Size in bytes.</summary>
		public long size { get { return streamCommon.size; } }

		///<summary>Encoder settings used for encoding this stream.
		///String format: name=value / name=value / ...</summary>
		public string encoderSettingsRaw { get { return streamCommon.encoderSettingsRaw; } }

		///<summary>Encoder settings used for encoding this stream.</summary>
		public IDictionary<string, string> encoderSettings { get { return streamCommon.encoderSettings; } }
		#endregion

		#region GeneralVideoAudioTextImageMenuCommon
		///<summary>Codec ID available from some codecs.</summary>
		///<example>AAC audio:A_AAC, h.264 video:V_MPEG4/ISO/AVC</example>
		public string codecId { get { return streamCommon.codecId; } }

		///<summary>Common name of the codec.</summary>
		public string codecCommonName { get { return streamCommon.codecCommonName; } }
		#endregion

		#region GeneralVideoAudioTextMenu
		///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
		public int delay { get { return streamCommon.delay; } }

		///<summary>Duration of the stream in milliseconds.</summary>
		public int duration { get { return streamCommon.duration; } }
		#endregion

		#region General
		string _encodedBy;
		///<summary>Name of the person/group who encoded this file.</summary>
		public string encodedBy {
			get {
				if(_encodedBy == null)
					_encodedBy = miGetString("EncodedBy");
				return _encodedBy;
			}
		}

		string _album;
		///<summary>Album name, if the file represents an album.</summary>
		public string album {
			get {
				if(_album == null)
					_album = miGetString("Album");
				return _album;
			}
		}

		string _iTunesGrouping = null;
		///<summary>The grouping used by iTunes.</summary>
		public string iTunesGrouping {
			get {
				if(_iTunesGrouping == null)
					_iTunesGrouping = miGetString("Grouping");
				return _iTunesGrouping;
			}
		}

		string _iTunesCompilation = null;
		///<summary>The compilation used by iTunes.</summary>
		public string iTunesCompilation {
			get {
				if(_iTunesCompilation == null)
					_iTunesCompilation = miGetString("Compilation");
				return _iTunesCompilation;
			}
		}

		int _bitRate = int.MinValue;
		///<summary>Overall bitrate of all streams.</summary>
		public int bitRate {
			get {
				if(_bitRate == int.MinValue)
					_bitRate = miGetInt("OverallBitRate");
				return _bitRate;
			}
		}

		int _bitRateMaximum = int.MinValue;
		///<summary>Maximum overall bitrate of all streams.</summary>
		public int bitRateMaximum {
			get {
				if(_bitRateMaximum == int.MinValue)
					_bitRateMaximum = miGetInt("OverallBitRate_Maximum");
				return _bitRateMaximum;
			}
		}

		int _bitRateMinimum = int.MinValue;
		///<summary>Minimum overall bitrate of all streams.</summary>
		public int bitRateMinimum {
			get {
				if(_bitRateMinimum == int.MinValue)
					_bitRateMinimum = miGetInt("OverallBitRate_Minimum");
				return _bitRateMinimum;
			}
		}

		int _bitRateNominal = int.MinValue;
		///<summary>Maximum allowed overall bitrate of all streams.</summary>
		public int bitRateNominal {
			get {
				if(_bitRateNominal == int.MinValue)
					_bitRateNominal = miGetInt("OverallBitRate_Nominal");
				return _bitRateNominal;
			}
		}
		#endregion

		#region InternalUse
		int _videoCount = int.MinValue;
		///<summary>Number of video streams in this file.</summary>
		protected int videoCount {
			get {
				if(_videoCount == int.MinValue)
					_videoCount = miGetInt("VideoCount");
				return _videoCount;
			}
		}

		int _audioCount = int.MinValue;
		///<summary>Number of audio streams in this file.</summary>
		protected int audioCount {
			get {
				if(_audioCount == int.MinValue)
					_audioCount = miGetInt("AudioCount");
				return _audioCount;
			}
		}

		int _textCount = int.MinValue;
		///<summary>Number of subtitles or other texts in this file.</summary>
		protected int textCount {
			get {
				if(_textCount == int.MinValue)
					_textCount = miGetInt("TextCount");
				return _textCount;
			}
		}

		int _imageCount = int.MinValue;
		///<summary>Number of images in this file.</summary>
		protected int imageCount {
			get {
				if(_imageCount == int.MinValue)
					_imageCount = miGetInt("ImageCount");
				return _imageCount;
			}
		}

		int _chapterCount = int.MinValue;
		///<summary>Number of chapters in this file.</summary>
		protected int chapterCount {
			get {
				if(_chapterCount == int.MinValue)
					_chapterCount = miGetInt("ChaptersCount");
				return _chapterCount;
			}
		}

		int _menuCount = int.MinValue;
		///<summary>Number of menu streams in this file.</summary>
		protected int menuCount {
			get {
				if(_menuCount == int.MinValue)
					_menuCount = miGetInt("MenuCount");
				return _menuCount;
			}
		}
		#endregion
	}
}
