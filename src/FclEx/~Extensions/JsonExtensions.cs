﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace FclEx
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings _ignoreSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private static readonly JsonSerializer _defaultJsonSerializer = JsonSerializer.CreateDefault();

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

        public static XmlDocument ToXmlNode(this JToken token, string deserializeRootElementName, bool writeArrayAttribute)
        {
            var converter = new XmlNodeConverter
            {
                DeserializeRootElementName = deserializeRootElementName,
                WriteArrayAttribute = writeArrayAttribute
            };
            return token.ToObject<XmlDocument>(JsonSerializer.Create(new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { converter }
            }));
        }

        public static XDocument ToXNode(this JToken token, string deserializeRootElementName, bool writeArrayAttribute)
        {
            var converter = new XmlNodeConverter
            {
                DeserializeRootElementName = deserializeRootElementName,
                WriteArrayAttribute = writeArrayAttribute
            };
            return token.ToObject<XDocument>(JsonSerializer.Create(new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { converter }
            }));
        }

        public static JToken ToJToken(this object obj, JsonSerializer jsonSerializer = null)
        {
            return JToken.FromObject(obj, jsonSerializer ?? _defaultJsonSerializer);
        }

        public static JObject ToJObject(this object obj, JsonSerializer jsonSerializer = null)
        {
            return JObject.FromObject(obj, jsonSerializer ?? _defaultJsonSerializer);
        }
    }
}
