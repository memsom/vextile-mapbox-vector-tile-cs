using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Mapbox.VectorTile.Constants;
using Mapbox.VectorTile.Geometry;
using ValueType = Mapbox.VectorTile.Constants.ValueType;
using System.Linq;


namespace Mapbox.VectorTile;

public class VectorTileReaderException : Exception
{
    public VectorTileReaderException(string message) : base(message) { }
    public VectorTileReaderException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Mail vector tile reader class
/// </summary>
public class VectorTileReader
{
    /// <summary>
    /// Initialize VectorTileReader
    /// </summary>
    /// <param name="data">Byte array containing the raw (already unzipped) tile data</param>
    /// <param name="validate">If true, run checks if the tile contains valid data. Decreases decoding speed.</param>
    public VectorTileReader(byte[] data, bool validate = true)
    {
        if (null == data)
        {
            throw new VectorTileReaderException("Tile data cannot be null");
        }

        if (data.Length < 1)
        {
            throw new VectorTileReaderException("Tile data cannot be empty");
        }

        if (data[0] == 0x1f && data[1] == 0x8b)
        {
            throw new VectorTileReaderException("Tile data is zipped");
        }

        _validate = validate;
        layers(data);
    }


    private readonly Dictionary<string, byte[]> _layers = new();
    private readonly bool _validate;


    private void layers(byte[] data)
    {
        PbfReader tileReader = new PbfReader(data);
        while (tileReader.NextByte())
        {
            if (_validate && !ConstantsAsDictionary.TileType.ContainsKey(tileReader.Tag))
            {
                throw new VectorTileReaderException($"Unknown tile tag: {tileReader.Tag}");
            }

            if (tileReader.Tag == (int)TileType.Layers)
            {
                string name = null;
                byte[] layerMessage = tileReader.View();
                PbfReader layerView = new PbfReader(layerMessage);
                while (layerView.NextByte())
                {
                    if (layerView.Tag == (int)LayerType.Name)
                    {
                        ulong strLen = (ulong)layerView.Varint();
                        name = layerView.GetString(strLen);
                    }
                    else
                    {
                        layerView.Skip();
                    }
                }

                if (_validate)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new VectorTileReaderException("Layer missing name");
                    }

                    if (_layers.ContainsKey(name))
                    {
                        throw new VectorTileReaderException($"Duplicate layer names: {name}");
                    }
                }

                _layers.Add(name, layerMessage);
            }
            else
            {
                tileReader.Skip();
            }
        }
    }


    /// <summary>
    /// Collection of layers contained in the tile
    /// </summary>
    /// <returns>Collection of layer names</returns>
    public ReadOnlyCollection<string> LayerNames()
    {
#if NET20 || PORTABLE || WINDOWS_UWP
            string[] lyrNames = new string[_Layers.Keys.Count];
            _Layers.Keys.CopyTo(lyrNames, 0);
            return new ReadOnlyCollection<string>(lyrNames);
#else
        return _layers.Keys.ToList().AsReadOnly();
#endif
    }

    /// <summary>
    /// Get a tile layer by name
    /// </summary>
    /// <param name="layerName">Name of the layer to request</param>
    /// <returns>Decoded <see cref="VectorTileLayer"/></returns>
    public VectorTileLayer GetLayer(string name)
    {
        if (!_layers.ContainsKey(name))
        {
            return null;
        }

        return getLayer(_layers[name]);
    }


    private VectorTileLayer getLayer(byte[] data)
    {
        VectorTileLayer layer = new VectorTileLayer(data);
        PbfReader layerReader = new PbfReader(layer.Data);
        while (layerReader.NextByte())
        {
            int layerType = layerReader.Tag;
            if (_validate && !ConstantsAsDictionary.LayerType.ContainsKey(layerType))
            {
                throw new VectorTileReaderException($"Unknown layer type: {layerType}");
            }

            switch ((LayerType)layerType)
            {
                case LayerType.Version:
                    ulong version = (ulong)layerReader.Varint();
                    layer.Version = version;
                    break;
                case LayerType.Name:
                    ulong strLength = (ulong)layerReader.Varint();
                    layer.Name = layerReader.GetString(strLength);
                    break;
                case LayerType.Extent:
                    layer.Extent = (ulong)layerReader.Varint();
                    break;
                case LayerType.Keys:
                    byte[] keyBuffer = layerReader.View();
                    string key = Encoding.UTF8.GetString(keyBuffer, 0, keyBuffer.Length);
                    layer.Keys.Add(key);
                    break;
                case LayerType.Values:
                    byte[] valueBuffer = layerReader.View();
                    PbfReader valReader = new PbfReader(valueBuffer);
                    while (valReader.NextByte())
                    {
                        switch ((ValueType)valReader.Tag)
                        {
                            case ValueType.String:
                                byte[] stringBuffer = valReader.View();
                                string value = Encoding.UTF8.GetString(stringBuffer, 0, stringBuffer.Length);
                                layer.Values.Add(value);
                                break;
                            case ValueType.Float:
                                float snglVal = valReader.GetFloat();
                                layer.Values.Add(snglVal);
                                break;
                            case ValueType.Double:
                                double dblVal = valReader.GetDouble();
                                layer.Values.Add(dblVal);
                                break;
                            case ValueType.Int:
                                long i64 = valReader.Varint();
                                layer.Values.Add(i64);
                                break;
                            case ValueType.UInt:
                                long u64 = valReader.Varint();
                                layer.Values.Add(u64);
                                break;
                            case ValueType.SInt:
                                long s64 = valReader.Varint();
                                layer.Values.Add(s64);
                                break;
                            case ValueType.Bool:
                                long b = valReader.Varint();
                                layer.Values.Add(b == 1);
                                break;
                            default:
                                throw new VectorTileReaderException(string.Format(
                                    NumberFormatInfo.InvariantInfo
                                    , "NOT IMPLEMENTED valueReader.Tag:{0} valueReader.WireType:{1}"
                                    , valReader.Tag
                                    , valReader.WireType
                                ));
                            //uncomment the following lines when not throwing!!
                            //valReader.Skip();
                            //break;
                        }
                    }

                    break;
                case LayerType.Features:
                    layer.AddFeatureData(layerReader.View());
                    break;
                default:
                    layerReader.Skip();
                    break;
            }
        }

        if (_validate)
        {
            if (string.IsNullOrEmpty(layer.Name))
            {
                throw new VectorTileReaderException("Layer has no name");
            }

            if (0 == layer.Version)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}] has invalid version. Only version 2.x of 'Mapbox Vector Tile Specification' (https://github.com/mapbox/vector-tile-spec) is supported.");
            }

            if (2 != layer.Version)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}] has invalid version: {layer.Version}. Only version 2.x of 'Mapbox Vector Tile Specification' (https://github.com/mapbox/vector-tile-spec) is supported.");
            }

            if (0 == layer.Extent)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}] has no extent.");
            }

            if (0 == layer.FeatureCount())
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}] has no features.");
            }

            if (layer.Values.Count != layer.Values.Distinct().Count())
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}]: duplicate attribute values found");
            }
        }

        return layer;
    }


    /// <summary>
    /// Get a feature of the <see cref="VectorTileLayer"/>
    /// </summary>
    /// <param name="layer"><see cref="VectorTileLayer"/> containing the feature</param>
    /// <param name="data">Raw byte data of the feature</param>
    /// <param name="validate">If true, run checks if the tile contains valid data. Decreases decoding speed.</param>
    /// <param name="clippBuffer">
    /// <para>'null': returns the geometries unaltered as they are in the vector tile. </para>
    /// <para>Any value >=0 clips a border with the size around the tile. </para>
    /// <para>These are not pixels but the same units as the 'extent' of the layer. </para>
    /// </param>
    /// <returns></returns>
    public static VectorTileFeature GetFeature(
        VectorTileLayer layer
        , byte[] data
        , bool validate = true
        , uint? clipBuffer = null
        , float scale = 1.0f
    )
    {
        PbfReader featureReader = new PbfReader(data);
        VectorTileFeature feat = new VectorTileFeature(layer, clipBuffer, scale);
        bool geomTypeSet = false;
        while (featureReader.NextByte())
        {
            int featureType = featureReader.Tag;
            if (validate && !ConstantsAsDictionary.FeatureType.ContainsKey(featureType))
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}] has unknown feature type: {featureType}");
            }

            switch ((FeatureType)featureType)
            {
                case FeatureType.Id:
                    feat.Id = (ulong)featureReader.Varint();
                    break;
                case FeatureType.Tags:
                    List<int> tags = featureReader.GetPackedUnit32().Select(t => (int)t).ToList();

                    feat.Tags = tags;
                    break;
                case FeatureType.Type:
                    int geomType = (int)featureReader.Varint();
                    if (validate && !ConstantsAsDictionary.GeomType.ContainsKey(geomType))
                    {
                        throw new VectorTileReaderException($"Layer [{layer.Name}] has unknown geometry type tag: {geomType}");
                    }

                    feat.GeometryType = (GeomType)geomType;
                    geomTypeSet = true;
                    break;
                case FeatureType.Geometry:
                    if (null != feat.GeometryCommands)
                    {
                        throw new VectorTileReaderException($"Layer [{layer.Name}], feature already has a geometry");
                    }

                    //get raw array of commands and coordinates
                    feat.GeometryCommands = featureReader.GetPackedUnit32();
                    break;
                default:
                    featureReader.Skip();
                    break;
            }
        }

        if (validate)
        {
            if (!geomTypeSet)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}]: feature missing geometry type");
            }

            if (null == feat.GeometryCommands)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}]: feature has no geometry");
            }

            if (0 != feat.Tags.Count % 2)
            {
                throw new VectorTileReaderException($"Layer [{layer.Name}]: uneven number of feature tag ids");
            }

            if (feat.Tags.Count > 0)
            {
                int maxKeyIndex = feat.Tags.Where((key, idx) => idx % 2 == 0).Max();
                int maxValueIndex = feat.Tags.Where((key, idx) => (idx + 1) % 2 == 0).Max();

                if (maxKeyIndex >= layer.Keys.Count)
                {
                    throw new VectorTileReaderException($"Layer [{layer.Name}]: maximum key index equal or greater number of key elements");
                }

                if (maxValueIndex >= layer.Values.Count)
                {
                    throw new VectorTileReaderException($"Layer [{layer.Name}]: maximum value index equal or greater number of value elements");
                }
            }
        }

        return feat;
    }
}
