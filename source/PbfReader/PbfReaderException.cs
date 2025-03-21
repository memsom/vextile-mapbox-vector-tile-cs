using System;

namespace Mapbox.VectorTile;

public class PbfReaderException : Exception
{
    public PbfReaderException(string message) : base(message) { }
    public PbfReaderException(string message, Exception innerException) : base(message, innerException) { }
}
