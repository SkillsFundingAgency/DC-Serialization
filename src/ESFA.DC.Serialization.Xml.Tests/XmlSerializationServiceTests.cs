using System;
using System.IO;
using ESFA.DC.Serialization.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Serialization.Xml.Tests
{
    public class XmlSerializationServiceTests
    {
        [Fact]
        public void DeserializeFromString()
        {
            string xmlString = File.ReadAllText(@"TestData\Data.xml");

            var service = NewService();

            var deserializedObject = service.Deserialize<Root>(xmlString);

            deserializedObject.Should().NotBeNull();
            deserializedObject.CollectionField.Should().HaveCount(3);
            deserializedObject.MandatoryStringField.Should().Be("MandatoryStringField1");
        }

        [Fact]
        public void DeserializeFromString_Null()
        {
            var service = NewService();

            Action action = () => service.Deserialize<Root>((string)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeFromString_Whitespace()
        {
            var service = NewService();

            Action action = () => service.Deserialize<Root>(" ");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeFromStream()
        {
            var service = NewService();
            var xmlString = File.ReadAllText(@"TestData\Data.xml");

            var deserializedObject = service.Deserialize<Root>(GenerateStreamFromString(xmlString));

            deserializedObject.Should().NotBeNull();
            deserializedObject.CollectionField.Should().HaveCount(3);
            deserializedObject.MandatoryStringField.Should().Be("MandatoryStringField1");
        }

        [Fact]
        public void DeserializeFromStream_Null()
        {
            var service = NewService();

            Action action = () => service.Deserialize<Root>((Stream)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SerializeToString()
        {
            var service = NewService();

            var objectToSerialize = new Root()
            {
                MandatoryStringField = "One",
                ComplexField = new RootComplexField()
                {
                    IntegerField = "Two",
                    StringField = "Three",
                },
                CollectionField = new RootCollectionField[]
                {
                    new RootCollectionField()
                    {
                        DecimalField = 1,
                        PositiveIntegerField = "Four",
                        StringField = "Five"
                    }
                }
            };

            service.Serialize(objectToSerialize).Should().Be(File.ReadAllText(@"TestData/SerializedDataEncoding.xml"));
        }

        [Fact]
        public void SerializeToString_Null()
        {
            var service = NewService();

            Action action = () => service.Serialize<Root>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SerializeToStream()
        {
            var service = NewService();

            var objectToSerialize = new Root()
            {
                MandatoryStringField = "One",
                ComplexField = new RootComplexField()
                {
                    IntegerField = "Two",
                    StringField = "Three",
                },
                CollectionField = new RootCollectionField[]
                {
                    new RootCollectionField()
                    {
                        DecimalField = 1,
                        PositiveIntegerField = "Four",
                        StringField = "Five"
                    }
                }
            };

            string serializedString;

            using (var stream = new MemoryStream())
            {
                service.Serialize(objectToSerialize, stream);

                stream.Position = 0;

                serializedString = new StreamReader(stream).ReadToEnd();
            }

            serializedString.Should().Be(File.ReadAllText(@"TestData/SerializedData.xml"));
        }

        [Fact]
        public void SerializeToStream_ObjectToSerializeNull()
        {
            var service = NewService();

            using (var stream = new MemoryStream())
            {
                Action action = () => service.Serialize<Root>(null, stream);

                action.Should().Throw<ArgumentNullException>();
            }
        }

        [Fact]
        public void SerializeToStream_StreamNull()
        {
            var service = NewService();

            var objectToSerialize = new Root();

            Action action = () => service.Serialize(objectToSerialize, null);

            action.Should().Throw<ArgumentNullException>();
        }

        private XmlSerializationService NewService()
        {
            return new XmlSerializationService();
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
