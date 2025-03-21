using System;
using System.Globalization;


namespace Mapbox.VectorTile.Geometry;

/// <summary>
/// Structure to hold a 2D point coordinate pair
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "<Pending>")]
public struct Point2d<T>
{
    public Point2d(T x, T y)
    {
        X = x;
        Y = y;
    }


    public T X; //performance: field instead of property
    public T Y; //performance: field instead of property


    public LatLng ToLngLat(ulong z, ulong x, ulong y, ulong extent, bool checkLatLngMax = false)
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

        LatLng latLng = new LatLng()
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

    //#if !PORTABLE

    //		public static explicit operator Point2d<T>(Point2d<float> v) {
    //			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
    //			Point2d<T> pnt = new Point2d<T>();
    //			pnt.X = (T)converter.ConvertTo(v.X, typeof(T));
    //			pnt.Y = (T)converter.ConvertTo(v.Y, typeof(T));
    //			return pnt;
    //		}

    //		public static explicit operator Point2d<T>(Point2d<int> v) {
    //			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
    //			Point2d<T> pnt = new Point2d<T>();
    //			pnt.X = (T)converter.ConvertTo(v.X, typeof(T));
    //			pnt.Y = (T)converter.ConvertTo(v.Y, typeof(T));
    //			return pnt;
    //		}

    //		public static explicit operator Point2d<T>(Point2d<long> v) {
    //			TypeConverter converter = TypeDescriptor.GetConverter(typeof(long));
    //			Point2d<T> pnt = new Point2d<T>();
    //			pnt.X = (T)converter.ConvertTo(v.X, typeof(T));
    //			pnt.Y = (T)converter.ConvertTo(v.Y, typeof(T));
    //			return pnt;
    //		}
    //#else
    //		public static explicit operator Point2d<T>(Point2d<long> v) {
    //			bool bla = typeof(IConvertible).IsAssignableFrom(T);

    //			Point2d<T> pnt = new Point2d<T>();
    //			return pnt;
    //		}
    //#endif
}
