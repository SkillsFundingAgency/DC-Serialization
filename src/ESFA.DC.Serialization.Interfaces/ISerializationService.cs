using System.IO;

namespace ESFA.DC.Serialization.Interfaces
{
    public interface ISerializationService
    {
        string Serialize<T>(T objectToSerialize);

        void Serialize<T>(T objectToSerialize, Stream stream);

        T Deserialize<T>(string serializedObject);

        T Deserialize<T>(Stream stream);
    }
}
