using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FclEx
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings _ignoreSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public static string ToJson(this object obj, Formatting formatting = Formatting.None, bool ignoreNull = false)
        {
            return JsonConvert.SerializeObject(obj, formatting, ignoreNull ? _ignoreSettings : null);
        }

        public static JToken ToJToken(this string str)
        {
            return JToken.Parse(str);
        }

        public static JObject ToJObject(this JToken token)
        {
            return token.ToObject<JObject>();
        }

        public static JArray ToJArray(this JToken token)
        {
            return token.ToObject<JArray>();
        }

        public static string ToSimpleString(this JToken obj)
        {
            return obj.ToString(Formatting.None);
        }

        public static int ToInt(this JToken token)
        {
            return token.ToObject<int>();
        }

        public static long ToLong(this JToken token)
        {
            return token.ToObject<long>();
        }

        public static T ToEnum<T>(this JToken token, T defaultVaule = default(T))
            where T : struct, IConvertible
        {
            return token.ToString().ToEnum(defaultVaule);
        }
    }
}
