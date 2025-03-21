using System.Diagnostics;
using System.Collections.ObjectModel;


namespace Mapbox.VectorTile;

/// <summary>
/// Class to access the tile data
/// </summary>
[DebuggerDisplay("{Zoom}/{TileColumn}/{TileRow}")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4545:\"DebuggerDisplayAttribute\" strings should reference existing members", Justification = "Third party code")]
public class VectorTileData
{
    /// <summary>
    /// Class to access the tile data
    /// </summary>
    /// <param name="data">Byte array containing the raw (already unzipped) tile data</param>
    /// <param name="validate">If true, run checks if the tile contains valid data. Decreases decoding speed.</param>
    public VectorTileData(byte[] data, bool validate = true)
    {
        _vtr = new VectorTileReader(data, validate);
    }


    private readonly VectorTileReader _vtr;


    /// <summary>
    /// Collection of layers contained in the tile
    /// </summary>
    /// <returns>Collection of layer names</returns>
    public ReadOnlyCollection<string> LayerNames()
    {
        return _vtr.LayerNames();
    }


    /// <summary>
    /// Get a tile layer by name
    /// </summary>
    /// <param name="layerName">Name of the layer to request</param>
    /// <returns>Decoded <see cref="VectorTileLayer"/></returns>
    public VectorTileLayer GetLayer(string layerName)
    {
        return _vtr.GetLayer(layerName);
    }
}
