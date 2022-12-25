﻿using DataTools.MathTools.Polar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Windows.Extensions
{
    public static class PolarCoordinatesExtensions
    {

        public static System.Windows.Point ToScreenCoordinates(this PolarCoordinates p, System.Windows.Rect rect)
        {
            var rc = new LinearRect(rect.Left, rect.Top, rect.Width, rect.Height);
            var pt = PolarCoordinates.ToLinearCoordinates(p, rc);
            return new System.Windows.Point(pt.X, pt.Y);
        }

      
    }
}
