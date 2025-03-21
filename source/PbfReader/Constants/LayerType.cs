namespace Mapbox.VectorTile.Constants;

/// <summary>
/// Types contained in a layer https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L50-L73
/// </summary>
public enum LayerType
{
    Version = 15,
    Name = 1,
    Features = 2,
    Keys = 3,
    Values = 4,
    Extent = 5
}
