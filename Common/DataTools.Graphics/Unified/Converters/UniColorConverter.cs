using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;


namespace DataTools.Graphics
{

    public class UniColorConverter : JsonConverter<UniColor>
    {
        public override UniColor ReadJson(JsonReader reader, Type objectType, UniColor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string s)
            {
                return UniColor.Parse(s);
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, UniColor value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());

            // throw new NotImplementedException();
        }
    }

}
