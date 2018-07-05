using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FclEx.Http.Event
{
    public class ActionEventJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var token = JToken.ReadFrom(reader);
            var type = token[nameof(ActionEvent<object>.Type)].ToObject<ActionEventType>();
            var obj = token[nameof(ActionEvent<object>.Result)];
            var resultType = objectType.GenericTypeArguments[0];
            switch (type)
            {
                case ActionEventType.EvtOk:
                {
                    var o = obj.ToObject(resultType);
                    return Activator.CreateInstance(objectType, type, o);
                }
                case ActionEventType.EvtRepeat:
                case ActionEventType.EvtCanceled:
                    return Activator.CreateInstance(objectType, type, null);

                case ActionEventType.EvtError:
                case ActionEventType.EvtRetry:
                {
                    var e = obj.ToObject<Exception>();
                    return Activator.CreateInstance(objectType, type, e);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType
                   && objectType.GetGenericTypeDefinition() == typeof(ActionEvent<>);
        }
    }
}
