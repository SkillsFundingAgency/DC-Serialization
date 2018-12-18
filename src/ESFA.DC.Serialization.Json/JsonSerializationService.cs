using System;
using System.IO;
using System.Text;
using ESFA.DC.Serialization.Interfaces;
using Newtonsoft.Json;

namespace ESFA.DC.Serialization.Json
{
    public class JsonSerializationService : IJsonSerializationService, ISerializationService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly Encoding _utf8WithoutBom;

        private readonly object _locker;

        private JsonSerializer _jsonSerialiser;

        public JsonSerializationService()
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            _utf8WithoutBom = new UTF8Encoding(false);
            _locker = new object();
        }

        public T Deserialize<T>(string serializedObject)
        {
            if (string.IsNullOrWhiteSpace(serializedObject))
            {
                throw new ArgumentNullException("Serialized Object string must not be null or whitespace.");
            }

            return JsonConvert.DeserializeObject<T>(serializedObject, _jsonSerializerSettings);
        }

        public T Deserialize<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("Stream must be initialized.");
            }

            stream.Seek(0, SeekOrigin.Begin);

            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                {
                    return GetJsonSerialiser().Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string Serialize<T>(T objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("Object To Serialize must not be Null.");
            }

            return JsonConvert.SerializeObject(objectToSerialize, _jsonSerializerSettings);
        }

        public void Serialize<T>(T objectToSerialize, Stream stream)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("Object To Serialize must not be Null.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("Stream must be initialized.");
            }

            stream.Seek(0, SeekOrigin.Begin);

            using (StreamWriter streamWriter = new StreamWriter(stream, _utf8WithoutBom, 1024, true))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    GetJsonSerialiser().Serialize(jsonTextWriter, objectToSerialize);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
        }

        private JsonSerializer GetJsonSerialiser()
        {
            lock (_locker)
            {
                if (_jsonSerialiser == null)
                {
                    _jsonSerialiser = JsonSerializer.Create(_jsonSerializerSettings);
                }
            }

            return _jsonSerialiser;
        }
    }
}
