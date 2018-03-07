using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.Serialization.Xml
{
    public class XmlSerializationService : ISerializationService
    {
        public T Deserialize<T>(string serializedObject)
        {
            if (string.IsNullOrWhiteSpace(serializedObject))
            {
                throw new ArgumentNullException("Serialized Object string must not be null or whitespace.");
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var reader = new StringReader(serializedObject))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("Stream must be initialized.");
            }

            var serializer = new XmlSerializer(typeof(T));

            return (T) serializer.Deserialize(stream);
        }

        public string Serialize<T>(T objectToSerialize)
        {
            var serializer = new XmlSerializer(objectToSerialize.GetType());

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, objectToSerialize);
                return writer.ToString();
            }
        }

        public void Serialize<T>(T objectToSerialize, Stream stream)
        {
            var serializer = new XmlSerializer(objectToSerialize.GetType());

            serializer.Serialize(stream, objectToSerialize);
        }
    }
}
