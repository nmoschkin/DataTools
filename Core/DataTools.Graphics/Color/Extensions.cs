using DataTools.MathTools.Polar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Graphics.Extensions
{
    /// <summary>
    /// Extensions for converting between polar coordinates and screen-compatible coordinates
    /// </summary>
    public static class PolarCoordinatesExtensions
    {
        /// <summary>
        /// Get point structure from polar coordinates and the specified bounding rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static System.Drawing.Point ToScreenCoordinates(this PolarCoordinates p, System.Drawing.Rectangle rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new System.Drawing.Point((int)pt.X, (int)pt.Y);
        }

        /// <summary>
        /// Get point structure from polar coordinates and the specified bounding rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static System.Drawing.PointF ToScreenCoordinates(this PolarCoordinates p, System.Drawing.RectangleF rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new System.Drawing.PointF((float)pt.X, (float)pt.Y);
        }

        /// <summary>
        /// Get point structure from polar coordinates and the specified bounding rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static UniPoint ToScreenCoordinates(this PolarCoordinates p, UniRect rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new UniPoint(pt.X, pt.Y);
        }
    }
}