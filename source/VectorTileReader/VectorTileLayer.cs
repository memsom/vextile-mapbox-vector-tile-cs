using System.Collections.Generic;
using System.Diagnostics;

namespace VexTile.Mapbox.VectorTile;

/// <summary>
/// Class to access a vector tile layer
/// </summary>
[DebuggerDisplay("Layer {Name}")]
public class VectorTileLayer
{
    /// <summary>
    /// Class to access a vector tile layer
    /// </summary>
    private VectorTileLayer()
    {
        FeaturesData = new();
        Keys = new();
        Values = new();
    }


    /// <summary>
    /// Initialize vector tile layer with data
    /// </summary>
    /// <param name="data">Raw layer data as byte array</param>
    public VectorTileLayer(byte[] data) : this()
    {
        Data = data;
    }


    /// <summary>Raw byte array data</summary>
    public byte[] Data { get; private set; }


    /// <summary>
    /// Get number of features.
    /// </summary>
    /// <returns>Number of features in this layer</returns>
    public int FeatureCount()
    {
        return FeaturesData.Count;
    }


    /// <summary>
    /// Get a feature of this layer
    /// </summary>
    /// <param name="feature">Index of the feature to request</param>
    /// <param name="clipBuffer">
    /// <para>'null': returns the geometries unaltered as they are in the vector tile. </para>
    /// <para>Any value >=0 clips a border with the size around the tile. </para>
    /// <para>These are not pixels but the same units as the 'extent' of the layer. </para>
    /// </param>
    /// <returns></returns>
    public VectorTileFeature GetFeature(int feature, uint? clipBuffer = null, float scale = 1.0f) =>
        VectorTileReader.GetFeature(this, FeaturesData[feature], true, clipBuffer, scale);


    public void AddFeatureData(byte[] data) => FeaturesData.Add(data);


    /// <summary>Name of this layer https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L57</summary>
    public string Name { get; set; }


    /// <summary>Version of this layer https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L55</summary>
    public ulong Version { get; set; }


    /// <summary>Extent of this layer https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L70</summary>
    public ulong Extent { get; set; }


    /// <summary>Raw data of the features contained in this layer</summary>
    private List<byte[]> FeaturesData { get; set; }


    /// <summary>
    /// Outstanding: switch to 'dynamic' when Unity supports .Net 4.5
    /// Values contained in this layer
    /// </summary>
    public List<object> Values { get; set; }


    /// <summary>
    /// Keys contained in this layer
    /// </summary>
    public List<string> Keys { get; set; }
}
