//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace FclEx.Json
//{
//    public class KeyValuePairsConverter : JsonConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            var list = value as IEnumerable<KeyValuePair<string, object>>;
//            writer.WriteStartArray();
//            foreach (var item in list)
//            {
//                writer.WriteStartObject();
//                writer.WritePropertyName(item.Key);
//                writer.WriteValue(item.Value);
//                writer.WriteEndObject();
//            }
//            writer.WriteEndArray();
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            var colType = objectType.GetGenericInterface(typeof(IEnumerable<>));
//            if (colType == null) return false;
//            var itemType = colType.GenericTypeArguments[0].GetGenericTypeDefinition();
//            if (itemType != typeof(KeyValuePair<,>)) return false;
//            return itemType.GetGenericArguments()[0] == typeof(string);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            var jObject = JObject.Load(reader);
//   var list = new List<>();


//        }
//    }
//}
