using System.ComponentModel;

namespace Mapbox.VectorTile.Geometry;

/// <summary>
/// Available geometry types
/// </summary>
public enum GeomType
{
    Unknown = 0,
    [Description("Point")] Point = 1,
    [Description("LineString")] Linestring = 2,
    [Description("Polygon")] Polygon = 3
}
