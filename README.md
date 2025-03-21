# vector-tile-cs

This project is based ont he last version of https://github.com/mapbox/vector-tile-cs. That project has not been moved to modern dotnet and therefore is hard to consume now. We have taken that version and updated the code to be compatible with .Net Standard, 8 and onwards.

C# library for decoding [`Mapbox Vector Tiles @ v2.x`](https://www.mapbox.com/vector-tiles/) ([vector tile specification](https://github.com/mapbox/vector-tile-spec)).

_**Decoding tiles created according to `Mapbox Vector Tile Specification @ v1.x` is not supported!**_

Available as nuget package: [![nuget.org](https://img.shields.io/nuget/v/Mapbox.VectorTile.svg)](https://www.nuget.org/packages/Mapbox.VectorTile)

Vector tile parsers in other languages:
* JavaScript: https://github.com/mapbox/vector-tile-js
* C++: https://github.com/mapbox/vector-tile

#Example

```c#
byte[] data = //raw unzipped vectortile
VectorTile vt = new VectorTile(data);
//get available layer names
foreach(var lyrName in vt.LayerNames()) {
    //get layer by name
    VectorTileLayer lyr = vt.GetLayer(lyrName);
    //iterate through all features
    for(int i = 0; i < lyr.FeatureCount(); i++) {
        Debug.WriteLine("{0} lyr:{1} feat:{2}", fileName, lyr.Name, i);
        //get the feature
        VectorTileFeature feat = lyr.GetFeature(i);
        //get feature properties
        var properties = feat.GetProperties();
        foreach(var prop in properties) {
            Debug.WriteLine("key:{0} value:{1}", prop.Key, prop.Value);
        }
        //or get property value if you already know the key
        //object value = feat.GetValue(prop.Key);
        //iterate through all geometry parts
        //requesting coordinates as ints
        foreach(var part in feat.Geometry<int>()) {
            //iterate through coordinates of the part
            foreach(var geom in part) {
                Debug.WriteLine("geom.X:{0} geom.Y:{1}", geom.X, geom.Y);
            }
        }
    }
}

```
