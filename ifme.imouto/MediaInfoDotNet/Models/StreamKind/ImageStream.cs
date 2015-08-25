/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * 
 ******************************************************************************
 *
 * ImageStream.cs
 * 
 * Presents information and functionality specific to an image stream.
 * 
 ******************************************************************************
 */

using System;
using MediaInfoLib;
using MediaInfoDotNet;
using System.Collections.Generic;

namespace MediaInfoDotNet.Models
{
	///<summary>Represents a single image stream.</summary>
	public sealed class ImageStream : Media
	{
		readonly MultiStreamCommon streamCommon;

		///<summary>ImageStream constructor.</summary>
		///<param name="mediaInfo">A MediaInfo object.</param>
		///<param name="id">The MediaInfo ID for this image stream.</param>
		public ImageStream(MediaInfo mediaInfo, int id)
			: base(mediaInfo, id) {
			kind = StreamKind.Image;
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

		#region VideoAudioTextImageCommon
		///<summary>Compression mode (lossy or lossless).</summary>
		public string compressionMode { get { return streamCommon.compressionMode; } }

		///<summary>Ratio of current size to uncompressed size.</summary>
		public string compressionRatio { get { return streamCommon.compressionRatio; } }

		///<example>Stream bit depth (16, 24, 32...)</example>
		public int bitDepth { get { return streamCommon.bitDepth; } }
		#endregion

		#region VideoAudioTextImageMenuCommon
		///<summary>2-letter (if available) or 3-letter ISO code.</summary>
		public string language { get { return streamCommon.language; } }

		///<summary>3-letter ISO 639-2 if exists, else empty.</summary>
		public string languageThree { get { return streamCommon.languageThree; } }
		#endregion

		#region VideoImageCommon
		///<summary>Ratio of pixel width to pixel height.</summary>
		public float pixelAspectRatio { get { return streamCommon.pixelAspectRatio; } }
		#endregion

		#region VideoTextImageCommon
		///<summary>Height in pixels.</summary>
		public int height { get { return streamCommon.height; } }

		///<summary>Width in pixels.</summary>
		public int width { get { return streamCommon.width; } }
		#endregion
	}
}
