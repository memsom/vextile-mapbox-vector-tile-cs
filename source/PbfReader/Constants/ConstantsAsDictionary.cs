using System.Collections.Generic;

namespace Mapbox.VectorTile.Constants;

/// <summary>
/// [wip] Investigate how to increase decoding speed. Looking up values in enums is slow
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Bug", "S3887:Mutable, non-private fields should not be \"readonly\"", Justification = "Third party code")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2386:Mutable fields should not be \"public static\"", Justification = "<Pending>")]
public static class ConstantsAsDictionary
{
    /// <summary>
    /// Root types contained in the vector tile. Currently just 'Layers' https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L75
    /// </summary>
    public static readonly Dictionary<int, string> TileType = new()
    {
        {
            3, "Layers"
        }
    };


    /// <summary>
    /// Types contained in a layer https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L50-L73
    /// </summary>
    public static readonly Dictionary<int, string> LayerType = new()
    {
        {
            15, "Version"
        },
        {
            1, "Name"
        },
        {
            2, "Features"
        },
        {
            3, "Keys"
        },
        {
            4, "Values"
        },
        {
            5, "Extent"
        }
    };

    /// <summary>
    /// Types contained in a feature https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L31-L47
    /// </summary>
    public static readonly Dictionary<int, string> FeatureType = new()
    {
        {
            1, "Id"
        },
        {
            2, "Tags"
        },
        {
            3, "Type"
        },
        {
            4, "Geometry"
        },
        {
            5, "Raster"
        }
    };


    /// <summary>
    /// Available geometry types https://github.com/mapbox/vector-tile-spec/tree/master/2.1#434-geometry-types
    /// </summary>
    public static readonly Dictionary<int, string> GeomType = new()
    {
        {
            0, "Unknown"
        },
        {
            1, "Point"
        },
        {
            2, "LineString"
        },
        {
            3, "Polygon"
        }
    };
}
