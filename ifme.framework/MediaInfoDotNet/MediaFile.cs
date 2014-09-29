/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.dll
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * Feature requests are welcome.
 * 
 * Use at your own risk, under the same license as MediaInfo itself.
 * 
 * If you want to make a donation to this project, instead make it to the 
 * MediaInfo project at: http://mediainfo.sourceforge.net
 * 
 * Copyright (C) Charles N. Burns
 * Contributers:
 *  Ryan Haney    : Support .NET Streams for input (rather than just files)
 *  Darrell Turner: Suggested I change the license to same as MediaInfo
 *                  Added originalHeight and originalWidth properties
 ******************************************************************************
 * 
 * MediaInfo.cs
 * 
 * Library entrypoint.
 * 
 * To make this work in your .NET project:
 * 1) Add this DLL, "MediaInfoDotNet.dll", to your project's "references"
 * 2) Copy MediaInfo.DLL into each subfolder of your project's "bin\" folder
 *    You can download it from http://mediainfo.sourceforge.net
 *    Do not try to add "MediaInfo.dll" to "references". Wrong type of DLL.
 */

using System;
using System.Collections.Generic;
using MediaInfoLib;
using MediaInfoDotNet.Models;
using System.IO;

namespace MediaInfoDotNet
{
	///<summary>Represents a media file.</summary>
	public sealed class MediaFile : GeneralStream
	{
		///<summary>MediaFile constructor for use with file paths.</summary>
		///<param name="filePath">Complete path and name of a file.</param>
		///<example>"c:\pics\me.jpg", "/home/charles/me.mkv"</example>
		public MediaFile(string filePath) : base(filePath) {
		}

		/// <summary>MediaFile constructor for use with streams.</summary>
		/// <param name="stream">The System.IO.Stream associated with the media data.</param>
		/// <param name="chunkSize">The size, in bytes, to read at any one time from the stream.</param>
		public MediaFile(Stream stream, int chunkSize = 65536)
			: base(stream, chunkSize) {
		}


		IDictionary<int, VideoStream> _Video;
		///<summary>Video streams in this file.</summary>
		public IDictionary<int, VideoStream> Video {
			get {
				if(_Video == null) {
					_Video = new Dictionary<int, VideoStream>(videoCount);
					for(int id = 0; id < videoCount; ++id) {
						_Video.Add(id, new VideoStream(mediaInfo, id));
					}
				}
				return _Video;
			}
		}

		IDictionary<int, AudioStream> _Audio;
		///<summary>Audio streams in this file.</summary>
		public IDictionary<int, AudioStream> Audio {
			get {
				if(_Audio == null) {
					_Audio = new Dictionary<int, AudioStream>(audioCount);
					for(int id = 0; id < audioCount; ++id) {
						_Audio.Add(id, new AudioStream(mediaInfo, id));
					}
				}
				return _Audio;
			}
		}


		IDictionary<int, TextStream> _Text;
		///<summary>Text streams in this file.</summary>
		public IDictionary<int, TextStream> Text {
			get {
				if(_Text == null) {
					_Text = new Dictionary<int, TextStream>(textCount);
					for(int id = 0; id < textCount; ++id) {
						_Text.Add(id, new TextStream(mediaInfo, id));
					}
				}
				return _Text;
			}
		}


		IDictionary<int, ImageStream> _Image;
		///<summary>Image streams in this file.</summary>
		public IDictionary<int, ImageStream> Image {
			get {
				if(_Image == null) {
					_Image = new Dictionary<int, ImageStream>(imageCount);
					for(int id = 0; id < imageCount; ++id) {
						_Image.Add(id, new ImageStream(mediaInfo, id));
					}
				}
				return _Image;
			}
		}


		IDictionary<int, ChapterStream> _Chapter;
		///<summary>Chapter streams in this file.</summary>
		public IDictionary<int, ChapterStream> Chapter {
			get {
				if(_Chapter == null) {
					_Chapter = new Dictionary<int, ChapterStream>(chapterCount);
					for(int id = 0; id < chapterCount; ++id) {
						_Chapter.Add(id, new ChapterStream(mediaInfo, id));
					}
				}
				return _Chapter;
			}
		}


		IDictionary<int, MenuStream> _Menu;
		///<summary>Menu streams in this file.</summary>
		public IDictionary<int, MenuStream> Menu {
			get {
				if(_Menu == null) {
					_Menu = new Dictionary<int, MenuStream>(menuCount);
					for(int id = 0; id < menuCount; ++id) {
						_Menu.Add(id, new MenuStream(mediaInfo, id));
					}
				}
				return _Menu;
			}
		}
	

	}
}
