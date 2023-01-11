using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    public class TranslationKeyAttribute : Attribute
    {
        public string Key { get; }

        public TranslationKeyAttribute(string key)
        {
            Key = key;
        }
    }
}