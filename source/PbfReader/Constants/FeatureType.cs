namespace VexTile.Mapbox.VectorTile.Constants;

/// <summary>
/// Types contained in a feature https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L31-L47
/// </summary>
public enum FeatureType
{
    Id = 1,
    Tags = 2,
    Type = 3,
    Geometry = 4,
    Raster = 5
}
