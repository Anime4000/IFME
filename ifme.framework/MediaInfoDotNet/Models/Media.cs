/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * 
 * Official source code: https://code.google.com/p/mediainfo-dot-net/
 * 
 ******************************************************************************
 * StreamBaseClass.cs
 * 
 * Represents information and functionality common to all streams.
 * 
 ******************************************************************************
 */

using System;
using MediaInfoLib;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace MediaInfoDotNet.Models
{
	///<summary>Implements features common to all media types.</summary>
	public abstract class Media : IDisposable
	{
		///<summary>Constructor to create a MediaInfo StreamKind-specific object, such as an audio
		///stream, for use by an existing MediaFile object.</summary>
		///<param name="mediaInfo">A pre-initialized MediaInfo object.</param>
		///<param name="id">The MediaInfo ID for this stream.</param>
		public Media(MediaInfo mediaInfo, int id) {
			string errorText;
			this.mediaInfo = mediaInfo;
			if(mediaInfo == null) {
				errorText = "MediaInfo object cannot be null.";
				throw new ArgumentNullException(errorText);
			}
			else if(!isMediaInfoDllCompatible()) {
				errorText = "Incompatible version of MediaInfo.DLL";
				throw new InvalidOperationException(errorText);
			}
			else {
				this.id = id;
			}
		}

		/// <summary>Constructor to create a MediaInfo stream using a .NET System.IO.Stream instead of a file.</summary>
		/// <param name="stream">A System.IO.Stream associated with the media data.
		/// Not to be confused with the MediaInfo stream types like "ImageStream" or "AudioStream"</param>
		/// <param name="chunkSize">The size, in bytes, to read at any one time from the stream.</param>
		// Submitted by Ryan Haney. Untested.
		public Media(Stream stream, int chunkSize) {
			this.disposed = false;
			this.mediaInfo = new MediaInfo();
			var buffer = new byte[chunkSize];
			int bytesRead;
			this.mediaInfo.Open_Buffer_Init(stream.Length, 0);

			do {
				bytesRead = stream.Read(buffer, 0, chunkSize);
				GCHandle GC = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				IntPtr ipBuffer = GC.AddrOfPinnedObject();

				if(this.mediaInfo.Open_Buffer_Continue(ipBuffer, (IntPtr)bytesRead) == 0) {
					GC.Free();
					break;
				}
				GC.Free();

				if(stream.CanSeek && this.mediaInfo.Open_Buffer_Continue_GoTo_Get() != -1) {
					long position = stream.Seek(
						this.mediaInfo.Open_Buffer_Continue_GoTo_Get(),
						SeekOrigin.Begin
					);
					this.mediaInfo.Open_Buffer_Init(stream.Length, position);
				}
			} while(bytesRead > 0);

			this.mediaInfo.Open_Buffer_Finalize();
		}


		///<summary>Complete path to the current media file.</summary>
		public string filePath { get; private set; }

		///<summary>Initializes the initial MediaInfo object.</summary>
		///<param name="filePath">Complete path and name of a file.</param>
		public Media(string filePath) {
			if(filePath == null)
				throw new ArgumentNullException("File name cannot be null.");
			this.mediaInfo = new MediaInfo();
			mediaInfo.Open(filePath);
			this.filePath = filePath;
		}

		///<summary>MediaInfo object</summary>
		protected MediaInfo mediaInfo { get; private set; }

		///<summary>MediaInfo ID for this stream kind.</summary>
		protected int id { get; private set; }

		///<summary>MediaInfo stream kind.</summary>
		protected StreamKind kind;

		#region MediaInfoLibraryCalls
		/// <summary>Returns information about MediaInfo.</summary>
		/// <param name="parameter">Option such as Info_Parameters</param>
		public string miOption(string parameter) {
			string miResult = mediaInfo.Option(parameter);
			return miResult == null ? string.Empty : miResult;
		}

		///<summary>Returns a MediaInfo value, "" if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		public string miGetString(string parameter) {
			string miResult = mediaInfo.Get(kind, id, parameter);
			return miResult == null ? string.Empty : miResult;
		}


		///<summary>Returns a MediaInfo value as a long, 0 if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		protected long miGetLong(string parameter) {
			long parsedValue;
			string miResult = mediaInfo.Get(kind, id, parameter);
			long.TryParse(miResult, out parsedValue);
			return parsedValue;
		}


		///<summary>Returns a MediaInfo value as an int, 0 if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		protected int miGetInt(string parameter) {
			int parsedValue;
			string miResult = mediaInfo.Get(kind, id, parameter);
			int.TryParse(miResult, out parsedValue);
			return parsedValue;
		}


		///<summary>Returns a MediaInfo value as a float, 0.0 if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		protected float miGetFloat(string parameter) {
			float parsedValue;
			string miResult = mediaInfo.Get(kind, id, parameter);
			float.TryParse(miResult, out parsedValue);
			return parsedValue;
		}


		///<summary>Returns a MediaInfo value as a float, 0.0 if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		protected double miGetDouble(string parameter) {
			double parsedValue;
			string miResult = mediaInfo.Get(kind, id, parameter);
			double.TryParse(miResult, out parsedValue);
			return parsedValue;
		}


		///<summary>Returns a MediaInfo date, Minval if error.</summary>
		///<param name="parameter">The MediaInfo parameter.</param>
		///<param name="format">The DateTime format for TryParseExact()</param>
		protected DateTime miGetDateTime(string parameter,
			string format = "'UTC' yyyy-MM-dd HH:mm:ss") {
			DateTime parsedValue = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
			string miResult = mediaInfo.Get(kind, id, parameter);
			DateTime.TryParseExact(miResult, format,
				DateTimeFormatInfo.InvariantInfo,
				DateTimeStyles.AssumeUniversal, out parsedValue);
			return parsedValue.ToUniversalTime();
		}


		///<summary>Returns true if MediaInfo.dll is compatible.</summary>
		bool isMediaInfoDllCompatible() {
			String ToDisplay =
				mediaInfo.Option("Info_Version", "0.7.0.0;MediaInfo.Net;0.1");
			return (ToDisplay.Length > 0 ? true : false);
		}
		#endregion

		#region IDisposable
		///<summary>Destructor. Disposes of resources.</summary>
		~Media() {
			Dispose();
		}


		bool disposed = false;
		///<summary>Ensures resource disposal.</summary>
		public void Dispose() {
			if(disposed == false) {
				disposed = true;
				mediaInfo.Close();
				GC.SuppressFinalize(this);
			}
		}
		#endregion
	}
}
