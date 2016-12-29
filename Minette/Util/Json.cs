using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Minette.Util
{
    public enum JsonCase { None = 0, Camel, Snake };
    public static class Json
    {
        /// <summary>
        /// Object from JSON
        /// </summary>
        /// <param name="jsonString">Serialized object</param>
        /// <returns>Dynamic data object</returns>
        public static dynamic Decode(string jsonString)
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        }

        /// <summary>
        /// Specified type object from JSON
        /// </summary>
        /// <typeparam name="T">Type of serialized object</typeparam>
        /// <param name="jsonString">Serialized object</param>
        /// <returns>Type specified object</returns>
        public static T Decode<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Object to JSON
        /// </summary>
        /// <param name="data">Object to serialize</param>
        /// <returns>Serialized object</returns>
        public static string Encode(dynamic data)
        {
            return Encode(data, JsonCase.None);
        }

        /// <summary>
        /// Object to case specified JSON
        /// </summary>
        /// <param name="data">Object to serialize</param>
        /// <param name="jsonCase">Case format</param>
        /// <returns>Serialized object</returns>
        public static string Encode(dynamic data, JsonCase jsonCase)
        {
            var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Converters.Add(new StringEnumConverter());
            if (jsonCase == JsonCase.Camel)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            else if (jsonCase == JsonCase.Snake)
            {
                settings.ContractResolver = new SnakeCaseContractResolver();
            }
            return JsonConvert.SerializeObject(data, settings);
        }

        /// <summary>
        /// Convert JObject to type specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToObject<T>(dynamic json, T defaultValue)
        {
            if(json is JObject)
            {
                return ((JObject)json).ToObject<T>();
            }
            else
            {
                return defaultValue;
            }
        }

        private class SnakeCaseContractResolver : DefaultContractResolver
        {
            private const string SnakeDelimiter = "_";
            protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
                => base.CreateProperties(type, memberSerialization).Select(ConvertSnakeCasePropertyName).ToList();

            private static JsonProperty ConvertSnakeCasePropertyName(JsonProperty p)
            {
                var target = p.PropertyName;

                p.PropertyName = (target.Substring(0, 1) +
                    string.Concat(target.ToCharArray(1, target.Length - 1).Select(c => char.IsUpper(c) ? SnakeDelimiter + c.ToString() : c.ToString()))
                    ).ToLower();
                return p;
            }
        }
    }
}
