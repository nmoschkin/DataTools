using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Helpers
{
    /// <summary>
    /// Object Merging, Diffing, and Comparing Tools
    /// </summary>
    public static class ObjectMerge
    {
        /// <summary>
        /// List every type between <paramref name="type"/> and <see cref="object"/>, inclusive.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetTypeAncestors(Type type)
        {
            var t = new List<Type>
            {
                type
            };

            do
            {
                type = type.BaseType;
                t.Add(type);
            } while (type != typeof(object));

            return t.ToArray();
        }

        /// <summary>
        /// Merge any two objects with a common base class.
        /// </summary>
        /// <param name="srcObj">The source data object</param>
        /// <param name="destObj">The destination object</param>
        /// <param name="requiredAttributes">Required property attributes (optional).  If set, then the found properties must have all of these attributes attached to be considered.</param>
        /// <returns>True if successful. False if the objects do not share a common base class.</returns>
        /// <remarks>
        /// This method will walk the resolution order of both object types looking for the<br />
        /// most recent common ancestor, and find all properties.
        /// <br /><br />
        /// This method will then grab properties from the source object if they are not null and
        /// invoke the setter for the same property on the destination object.
        /// <br /><br />
        /// "New" properties will not be copied.
        /// <br /><br />
        /// This method is useful for: <br />
        /// <br />1. Cloning a class into any related class.
        /// <br />2. Preserving the unique values of a related object while updating the core data with refreshed values.
        /// </remarks>
        public static bool MergeObjects(object srcObj, object destObj, Type[] requiredAttributes = null)
        {
            var res1 = GetTypeAncestors(srcObj.GetType());
            var res2 = GetTypeAncestors(destObj.GetType());

            var commonTypes = res1.Distinct().Intersect(res2.Distinct()).ToList();

            if (commonTypes.Count < 2)
            {
                res1 = srcObj.GetType().GetInterfaces();
                res2 = destObj.GetType().GetInterfaces();

                commonTypes = res1.Distinct().Intersect(res2.Distinct()).ToList();
            }

            if (commonTypes.Count > 1)
            {
                var mostRecent = commonTypes[0];
                var pinfo = mostRecent.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                if (requiredAttributes != null && requiredAttributes.Length == 0) requiredAttributes = null;

                foreach (var pi in pinfo)
                {
                    try
                    {
                        if (!pi.CanWrite) continue;

                        if (requiredAttributes != null)
                        {
                            foreach (var req in requiredAttributes)
                            {
                                if (req == null) continue;

                                var attrTest = pi.GetCustomAttribute(req);
                                if (attrTest == null) continue;
                            }
                        }

                        object obj = null;

                        try
                        {
                            obj = pi.GetValue(srcObj);
                        }
                        catch
                        {
                        }

                        if (obj != null)
                        {
                            try
                            {
                                pi.SetValue(destObj, obj);
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Merge an ancestor object into a derived object.
        /// </summary>
        /// <typeparam name="T">The type of the base object</typeparam>
        /// <typeparam name="U">The type of the derived object</typeparam>
        /// <param name="baseObj">The base object</param>
        /// <param name="derivedObj">The derived object</param>
        /// <param name="requiredAttributes">Required property attributes (optional).  If set, then the found properties must have all of these attributes attached to be considered.</param>
        /// <remarks>
        /// This method will grab properties from the base object if they are not null and
        /// invoke the setter for the same property on the derived object.
        /// <br /><br />
        /// "New" properties will not be copied.
        /// <br /><br />
        /// This method is useful for: <br />
        /// <br />1. Cloning a base class into a derived object.
        /// <br />2. Preserving the unique values of a derived object while updating the core data with refreshed values.
        /// </remarks>
        public static void MergeObjects<T, U>(T baseObj, U derivedObj, Type[] requiredAttributes = null)
            where T : class
            where U : class, T
        {
            var pinfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (requiredAttributes != null && requiredAttributes.Length == 0) requiredAttributes = null;

            foreach (var pi in pinfo)
            {
                try
                {
                    if (!pi.CanWrite) continue;

                    if (requiredAttributes != null)
                    {
                        foreach (var req in requiredAttributes)
                        {
                            if (req == null) continue;

                            var attrTest = pi.GetCustomAttribute(req);
                            if (attrTest == null) continue;
                        }
                    }

                    object obj = null;

                    try
                    {
                        obj = pi.GetValue(baseObj);
                    }
                    catch
                    {
                    }

                    if (obj != null)
                    {
                        try
                        {
                            pi.SetValue(derivedObj, obj);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}