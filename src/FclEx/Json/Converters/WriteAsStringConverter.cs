using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FclEx.Json.Converters
{
    public class WriteAsStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsPrimitive;
        }

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
