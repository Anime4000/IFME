/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * 
 ******************************************************************************
 *
 * OtherStream.cs
 * 
 * Presents information and functionality specific "other" streams.
 * Presumably this includes all obscure stream types that do not warrant
 * their own StreamKind.
 * "Chapters" is an example of a StreamKind now in "Other".
 * 
 ******************************************************************************
 */

using System;
using MediaInfoLib;
using MediaInfoDotNet.Models;

namespace MediaInfoDotNet.Models
{
	///<summary>Represents a single stream of type "Other" (none of the other types).</summary>
	public sealed class ChapterStream : Media
	{
		readonly MultiStreamCommon streamCommon;

		///<summary>ChapterStream constructor.</summary>
		///<param name="mediaInfo">A MediaInfo object.</param>
		///<param name="id">The MediaInfo ID for this stream.</param>
		public ChapterStream(MediaInfo mediaInfo, int id)
			: base(mediaInfo, id) {
			kind = StreamKind.Other;
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
	}
}
