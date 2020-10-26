﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Automate {
    public static class ImageArrayExtensions {
        /// <summary>
        /// Search for a <see cref="ImageArray"/> and return the first match
        /// </summary>
        /// <param name="tolerance">Minimum distance between 2 colors</param>
        /// <returns></returns>
        public static Point Locate(this ImageArray heystack, ImageArray needle, int tolerance = 0) {
            // tolerance squared
            tolerance *= tolerance;
            // h.GL(1) - n.GL(1) so the needle won't be outside of heystack ( same for GL(0) )
            for(int y1 = 0; y1 <= heystack.Height - needle.Height; y1++) {
                for(int x1 = 0; x1 <= heystack.Width - needle.Width; x1++) {
                    if(heystack.MatchesWith(x1, y1, needle, tolerance)) {
                        // return middle point
                        return new Point(x1 + needle.Width / 2, y1 + needle.Height / 2);
                    }
                }
            }
            return Point.Empty;
        }

        /// <summary>
        /// Search for a solid color and return the first match
        /// </summary>
        /// <param name="heystack"></param>
        /// <param name="rgba">{r, g, b, a}</param>
        /// <param name="width">Width of the solid color region</param>
        /// <param name="height">Width of the solid color region</param>
        /// <param name="tolerance">Minimum distance between 2 colors</param>
        /// <returns></returns>
        public static Point LocateColor(this ImageArray heystack, byte[] rgba, int width, int height, int tolerance = 0) {
            tolerance *= tolerance;
            for(int y1 = 0; y1 <= heystack.Height - height; y1++) {
                for(int x1 = 0; x1 < heystack.Width - width; x1++) {
                    if(heystack.MatchesWith(x1, y1, rgba, width, height, tolerance)) {
                        return new Point(x1 + width / 2, y1 + height / 2);
                    }
                }
            }
            return Point.Empty;
        }

        /// <summary>
        /// Search for a <see cref="ImageArray"/> and return all the results
        /// </summary>
        /// <param name="tolerance">Maximum distance between 2 colors</param>
        /// <param name="distance">Minimun distance between 2 found areas</param>
        /// <returns></returns>
        public static IEnumerable<Point> LocateAll(this ImageArray heystack, ImageArray needle, int tolerance = 0, int distance = 0) {
            tolerance *= tolerance;
            List<Rectangle> covered = new List<Rectangle>();
            for(int y1 = 0; y1 <= heystack.Height - needle.Height; y1++) {
                for(int x1 = 0; x1 <= heystack.Width - needle.Width; x1++) {
                    // Skip pixels that are in found areas
                    if(covered.Select(rect => rect.Contains(x1, y1)).Any()) continue;
                    // Add rect and return point if matches
                    if(heystack.MatchesWith(x1, y1, needle, tolerance)) {
                        // Add needle with minimum distance
                        covered.Add(new Rectangle(
                            x1 - distance,
                            y1 - distance,
                            needle.Width + distance,
                            needle.Height + distance
                        ));

                        yield return new Point(x1 + needle.Width / 2, y1 + needle.Height / 2);
                        // continue outside the needle
                        x1 += needle.Width;
                    }
                }
            }
        }

        /// <summary>
        /// Seach for a color and return all the results
        /// </summary>
        /// <param name="heystack"></param>
        /// <param name="rgba">{r, g, b, a}</param>
        /// <param name="width">Width of the solid color region</param>
        /// <param name="height">Width of the solid color region</param>
        /// <param name="tolerance">Minimum distance between 2 colors</param>
        /// <param name="distance">Minimun distance between 2 found areas</param>
        /// <returns></returns>
        public static IEnumerable<Point> LocateColorAll(this ImageArray heystack, byte[] rgba, int width, int height, int tolerance = 0, int distance = 0) {
            tolerance *= tolerance;
            List<Rectangle> coverd = new List<Rectangle>();
            for(int y1 = 0; y1 < heystack.Height - height; y1++) {
                for(int x1 = 0; x1 < heystack.Width - width; x1++) {
                    if(coverd.Select(rect => rect.Contains(x1, y1)).Any()) continue;

                    if(heystack.MatchesWith(x1, y1, rgba, width, height, tolerance)) {
                        coverd.Add(new Rectangle(
                            x1 - distance,
                            y1 - distance,
                            width + distance,
                            height + distance
                        ));

                        yield return new Point(x1 + width / 2, y1 + height / 2);
                        x1 += width;
                    }
                }
            }
        }
    }
}
