using System.ComponentModel;

namespace VexTile.Mapbox.VectorTile.Geometry;

/// <summary>
/// Available geometry types
/// </summary>
public enum GeometryType
{
    Unknown = 0,
    [Description("Point")] Point = 1,
    [Description("LineString")] Linestring = 2,
    [Description("Polygon")] Polygon = 3
}
