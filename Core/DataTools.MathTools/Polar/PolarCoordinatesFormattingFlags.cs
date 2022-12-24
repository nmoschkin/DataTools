/**************************************************
 * DataTools C# Utility Library (Universal)
 *
 * Module: PolarMath
 *         Structures for containing and converting
 *         to and from polar coordinates.
 *
 * Copyright (C) 2011-2022 Nathan Moschkin
 * All Rights Reserved
 *
 * Licensed Under the Apache 2.0 License
 **************************************************/

namespace DataTools.MathTools.Polar
{
    /// <summary>
    /// Flags to be used for formatting the display of a <see cref="PolarCoordinates"/> structure utilizing the <see cref="PolarCoordinates.ToString(PolarCoordinatesFormattingFlags, int)"/> function.
    /// </summary>
    public enum PolarCoordinatesFormattingFlags
    {
        /// <summary>
        /// Basic formatting
        /// </summary>
        Default = 0x0,

        /// <summary>
        /// Use the degrees symbol
        /// </summary>
        UseDegreeSymbol = 0x1,

        /// <summary>
        /// Use the polar symbol
        /// </summary>
        UsePolarSymbol = 0x2,

        /// <summary>
        /// Use the pi symbol
        /// </summary>
        UsePiSymbol = 0x4,

        /// <summary>
        /// Use the radians indicator
        /// </summary>
        UseRadianIndicator = 0x8,

        /// <summary>
        /// Use parentheses
        /// </summary>
        UseParenthesis = 0x10,

        /// <summary>
        /// Use brackets
        /// </summary>
        UseBrackets = 0x20,

        /// <summary>
        /// Display the value in radians
        /// </summary>
        DisplayInRadians = 0x40,

        /// <summary>
        /// Display the value in standard notation
        /// </summary>
        Standard = 0x1,

        /// <summary>
        /// Display the value in formatted radians
        /// </summary>
        Radians = 0x4 | 0x8 | 0x40,

        /// <summary>
        /// Display the value in standard scientific notation
        /// </summary>
        StandardScientific = 0x2,

        /// <summary>
        /// Display the value in scientific radians notation
        /// </summary>
        RadiansScientific = 0x2 | 0x4 | 0x8 | 0x40,
    }
}