﻿using System.Collections.Generic;
using Mapbox.VectorTile.Geometry.InteralClipperLib;

using Polygon = System.Collections.Generic.List<Mapbox.VectorTile.Geometry.InteralClipperLib.InternalClipper.IntPoint>;
using Polygons = System.Collections.Generic.List<System.Collections.Generic.List<Mapbox.VectorTile.Geometry.InteralClipperLib.InternalClipper.IntPoint>>;


namespace Mapbox.VectorTile.Geometry;

/// <summary>
/// Geometry related helper methods
/// </summary>
public static class GeomUtils
{
    /// <summary>
    /// TO BE REMOVED!!! Processing geometries is out of scope.
    /// Clip geometries extending beyond the tile border.
    /// </summary>
    /// <param name="geoms">Raw tile geometries of the feature</param>
    /// <param name="geomType">Geometry type of the feature</param>
    /// <param name="extent">Extent of the layer </param>
    /// <param name="bufferSize">Units (in internal tile coordinates) to go beyond the tile border. Pass '0' to clip exactly at the tile border</param>
    /// <param name="scale">Factor for scaling the geometries</param>
    /// <returns></returns>
    public static List<List<Point2d<long>>> ClipGeometries(
        List<List<Point2d<long>>> geoms,
        GeomType geomType,
        long extent,
        uint bufferSize,
        float scale
    )
    {
        List<List<Point2d<long>>> retVal = new List<List<Point2d<long>>>();

        //points: simply remove them if one part of the coordinate pair is out of bounds:
        // <0 || >extent
        if (geomType == GeomType.Point)
        {
            foreach (var geomPart in geoms)
            {
                List<Point2d<long>> outGeom = new List<Point2d<long>>();
                foreach (var geom in geomPart)
                {
                    if (
                        geom.X < (0L - bufferSize)
                        || geom.Y < (0L - bufferSize)
                        || geom.X > (extent + bufferSize)
                        || geom.Y > (extent + bufferSize)
                    )
                    {
                        continue;
                    }

                    outGeom.Add(geom);
                }

                if (outGeom.Count > 0)
                {
                    retVal.Add(outGeom);
                }
            }

            return retVal;
        }

        //use clipper for lines and polygons
        bool closed = geomType != GeomType.Linestring;

        Polygons subjects = new Polygons();
        Polygons clip = new Polygons(1);
        Polygons solution = new Polygons();

        clip.Add(new Polygon(4));
        clip[0].Add(new InternalClipper.IntPoint(0L - bufferSize, 0L - bufferSize));
        clip[0].Add(new InternalClipper.IntPoint(extent + bufferSize, 0L - bufferSize));
        clip[0].Add(new InternalClipper.IntPoint(extent + bufferSize, extent + bufferSize));
        clip[0].Add(new InternalClipper.IntPoint(0L - bufferSize, extent + bufferSize));

        foreach (var geompart in geoms)
        {
            Polygon part = new Polygon();

            foreach (var geom in geompart)
            {
                part.Add(new InternalClipper.IntPoint(geom.X, geom.Y));
            }

            subjects.Add(part);
        }

        InternalClipper.Clipper c = new InternalClipper.Clipper();
        c.AddPaths(subjects, InternalClipper.PolyType.ptSubject, closed);
        c.AddPaths(clip, InternalClipper.PolyType.ptClip, true);

        bool succeeded = false;
        if (geomType == GeomType.Linestring)
        {
            InternalClipper.PolyTree lineSolution = new InternalClipper.PolyTree();
            succeeded = c.Execute(
                InternalClipper.ClipType.ctIntersection
                , lineSolution
                , InternalClipper.PolyFillType.pftNonZero
                , InternalClipper.PolyFillType.pftNonZero
            );
            if (succeeded)
            {
                solution = InternalClipper.Clipper.PolyTreeToPaths(lineSolution);
            }
        }
        else
        {
            succeeded = c.Execute(
                InternalClipper.ClipType.ctIntersection
                , solution
                , InternalClipper.PolyFillType.pftNonZero
                , InternalClipper.PolyFillType.pftNonZero
            );
        }

        if (succeeded)
        {
            retVal = new List<List<Point2d<long>>>();
            foreach (var part in solution)
            {
                List<Point2d<long>> geompart = new List<Point2d<long>>();
                // HACK:
                // 1. clipper may or may not reverse order of vertices of LineStrings
                // 2. clipper semms to drop the first vertex of a Polygon
                // * We don't care about 1.
                // * Added a check for 2 and insert a copy of last vertex as first
                foreach (var geom in part)
                {
                    geompart.Add(new Point2d<long>()
                    {
                        X = geom.X,
                        Y = geom.Y
                    });
                }

                if (geomType == GeomType.Polygon && !geompart[0].Equals(geompart[geompart.Count - 1]))
                {
                    geompart.Insert(0, geompart[geompart.Count - 1]);
                }

                retVal.Add(geompart);
            }

            return retVal;
        }
        else
        {
            //if clipper was not successfull return original geometries
            return geoms;
        }
    }
}
