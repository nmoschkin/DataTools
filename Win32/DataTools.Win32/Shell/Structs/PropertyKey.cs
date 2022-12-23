// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeShell
//         Wrappers for native and COM shell interfaces.
//
// Some enum documentation copied from the MSDN (and in some cases, updated).
// Some classes and interfaces were ported from the WindowsAPICodePack.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System.Reflection;

//using DataTools.Hardware.MessageResources;
//using DataTools.Hardware;
//using DataTools.Hardware.Native;

namespace DataTools.Shell.Native
{
    // Summary:
    // Defines a unique key for a Shell Property
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        //
        // Summary:
        // PropertyKey Constructor
        //
        // Parameters:
        // formatId:
        // A unique GUID for the property
        //
        // propertyId:
        // Property identifier (PID)
        public PropertyKey(Guid formatId, int propertyId)
        {
            this.formatId = formatId;
            this.propertyId = propertyId;
        }

        //
        // Summary:
        // PropertyKey Constructor
        //
        // Parameters:
        // formatId:
        // A string represenstion of a GUID for the property
        //
        // propertyId:
        // Property identifier (PID)
        public PropertyKey(string formatId, int propertyId)
        {
            this.formatId = new Guid(formatId);
            this.propertyId = propertyId;
        }

        // Summary:
        // Implements the != (inequality) operator.
        //
        // Parameters:
        // propKey1:
        // First property key to compare
        //
        // propKey2:
        // Second property key to compare.
        //
        // Returns:
        // true if object a does not equal object b. false otherwise.
        public static bool operator !=(PropertyKey propKey1, PropertyKey propKey2)
        {
            return !propKey1.Equals(propKey2);
        }

        //
        // Summary:
        // Implements the == (equality) operator.
        //
        // Parameters:
        // propKey1:
        // First property key to compare.
        //
        // propKey2:
        // Second property key to compare.
        //
        // Returns:
        // true if object a equals object b. false otherwise.
        public static bool operator ==(PropertyKey propKey1, PropertyKey propKey2)
        {
            return propKey1.Equals(propKey2);
        }

        // Summary:
        // A unique GUID for the property
        private Guid formatId;

        public Guid FormatId
        {
            get
            {
                return formatId;
            }
        }

        //
        // Summary:
        // Property identifier (PID)
        private int propertyId;

        public int PropertyId
        {
            get
            {
                return propertyId;
            }
        }

        public string Name
        {
            get => PropertyKeys.GetPropertyName(this);
        }

        public string LocalizedDescription
        {
            get
            {
                PropertyInfo? pi = typeof(Resources.LocalizedProperties).GetProperty(Name);

                if (pi != null)
                {
                    return (string?)pi.GetValue(null) ?? PropertyKeys.GetPropertyName(this);
                }
                else
                {
                    return PropertyKeys.GetPropertyName(this);
                }
            }
        }

        // Summary:
        // Returns whether this object is equal to another. This is vital for performance
        // of value types.
        //
        // Parameters:
        // obj:
        // The object to compare against.
        //
        // Returns:
        // Equality result.
        public override bool Equals(object obj)
        {
            if (!(obj is PropertyKey pk)) return false;
            else return Equals(pk);
        }

        //
        // Summary:
        // Returns whether this object is equal to another. This is vital for performance
        // of value types.
        //
        // Parameters:
        // other:
        // The object to compare against.
        //
        // Returns:
        // Equality result.
        public bool Equals(PropertyKey other)
        {
            if (other.FormatId != formatId)
                return false;
            if (other.PropertyId != propertyId)
                return false;
            return true;
        }

        //
        // Summary:
        // Returns the hash code of the object. This is vital for performance of value
        // types.
        public override int GetHashCode()
        {
            var i = default(int);
            var b = formatId.ToByteArray();
            foreach (var by in b)
                i += by;
            i += propertyId;
            return i;
        }

        //
        // Summary:
        // Override ToString() to provide a user friendly string representation
        //
        // Returns:
        // String representing the property key
        public override string ToString()
        {
            return Name ?? formatId.ToString("B").ToUpper() + "[" + propertyId + "]";
        }
    }
}