using DataTools.MathTools.PolarMath;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Desktop.Extensions
{
    public static class PolarCoordinatesExtensions
    {
        /// <summary>
        /// Converts radians to screen coordinates.
        /// </summary>
        /// <param name="p">Radians value.</param>
        /// <param name="rect">Bounding rectangle.</param>
        /// <returns></returns>
        public static System.Drawing.Point ToScreenCoordinates(this PolarCoordinates p, System.Drawing.Rectangle rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new System.Drawing.Point((int)pt.X, (int)pt.Y);
        }

        /// <summary>
        /// Converts radians to screen coordinates.
        /// </summary>
        /// <param name="p">Radians value.</param>
        /// <param name="rect">Bounding rectangle.</param>
        /// <returns></returns>
        public static System.Drawing.PointF ToScreenCoordinates(this PolarCoordinates p, System.Drawing.RectangleF rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new System.Drawing.PointF((float)pt.X, (float)pt.Y);
        }

    }
}
