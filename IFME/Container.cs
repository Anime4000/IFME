using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum MediaContainer
{
    [EnumMember(Value = "3GP")]
    ThreeGP,
    AVI,
    MP4,
    MKV,
    TS,
    M2TS,
    MPG,
    MPEG,
    WEBM,
    WMV,

    MP2,
    MP3,
    M4A,
    OGG,
    OPUS,
    FLAC,
    WMA
}

[Flags]
public enum Mp4MuxFlags
{
    None = 0,
    FragKeyframe = 1 << 0,  // 1
    EmptyMoov = 1 << 1,  // 2
    SeparateMoof = 1 << 2   // 4
}

