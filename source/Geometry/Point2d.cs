using System;
using System.Globalization;

namespace VexTile.Mapbox.VectorTile.Geometry;

/// <summary>
/// Structure to hold a 2D point coordinate pair
/// </summary>
public struct Point2d<T>(T x, T y)
{
    public T X = x; //performance: field instead of property
    public T Y = y; //performance: field instead of property

    public LatLong ToLngLat(ulong z, ulong x, ulong y, ulong extent, bool checkLatLngMax = false)
    {
        double size = (double)extent * Math.Pow(2, (double)z);
        double x0 = (double)extent * (double)x;
        double y0 = (double)extent * (double)y;

        double dblY = Convert.ToDouble(Y);
        double dblX = Convert.ToDouble(X);
        double y2 = 180d - (dblY + y0) * 360d / size;
        double lng = (dblX + x0) * 360d / size - 180d;
        double lat = 360d / Math.PI * Math.Atan(Math.Exp(y2 * Math.PI / 180d)) - 90d;

        if (checkLatLngMax)
        {
            if (lng < -180d || lng > 180d)
            {
                throw new ArgumentException("Longitude out of range");
            }

            if (lat < -85.051128779806589d || lat > 85.051128779806589d)
            {
                throw new ArgumentException("Latitude out of range");
            }
        }

        LatLong latLng = new()
        {
            Lat = lat,
            Lng = lng
        };

        return latLng;
    }

    public override string ToString()
    {
        return string.Format(NumberFormatInfo.InvariantInfo, "{0}/{1}", X, Y);
    }
}
