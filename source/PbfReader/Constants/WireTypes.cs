namespace VexTile.Mapbox.VectorTile.Constants;

/// <summary>
/// PBF wire types
/// </summary>
public enum WireTypes
{
    VARINT = 0, // varint: int32, int64, uint32, uint64, sint32, sint64, bool, enum
    FIXED64 = 1, // 64-bit: double, fixed64, sfixed64
    BYTES = 2, // length-delimited: string, bytes, embedded messages, packed repeated fields
    FIXED32 = 5, // 32-bit: float, fixed32, sfixed32
    UNDEFINED = 99
}
