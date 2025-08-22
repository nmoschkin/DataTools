using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DataTools.Graphics
{
    /// <summary>
    /// Contains information used when a color hit event is fired
    /// </summary>
    public class ColorHitEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the color that was hit
        /// </summary>
        public UniColor Color { get; private set; }

        /// <summary>
        /// Create a new color hit object with the specified raw color interger value
        /// </summary>
        /// <param name="rawColor"></param>
        public ColorHitEventArgs(int rawColor)
        {
            Color = rawColor;
        }

        /// <summary>
        /// Create a new color hit object with the specified color value
        /// </summary>
        /// <param name="color"></param>
        public ColorHitEventArgs(Color color)
        {
            Color = new UniColor(color.ToArgb());
        }
    }

}
