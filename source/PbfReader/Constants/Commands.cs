namespace Mapbox.VectorTile.Constants;

/// <summary>
/// Vector tile geometry commands https://github.com/mapbox/vector-tile-spec/tree/master/2.1#431-command-integers
/// </summary>
public enum Commands
{
    MoveTo = 1,
    LineTo = 2,
    ClosePath = 7
}
