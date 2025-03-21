using System;

namespace Mapbox.VectorTile;

public class VectorTileException : Exception
{
    public VectorTileException(string message) : base(message) { }
    public VectorTileException(string message, Exception innerException) : base(message, innerException) { }
}
