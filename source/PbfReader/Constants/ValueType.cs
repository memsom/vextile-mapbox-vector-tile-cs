namespace Mapbox.VectorTile.Constants;

/// <summary>
/// Available ypes for values https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L17-L28
/// </summary>
public enum ValueType
{
    String = 1,
    Float = 2,
    Double = 3,
    Int = 4,
    UInt = 5,
    SInt = 6,
    Bool = 7
}
