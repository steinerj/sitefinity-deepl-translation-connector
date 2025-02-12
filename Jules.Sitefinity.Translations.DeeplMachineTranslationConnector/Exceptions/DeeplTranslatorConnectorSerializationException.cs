using System;
using System.Runtime.Serialization;

namespace Jules.Sitefinity.Translations.DeeplMachineTranslationConnector.Exceptions
{
    [Serializable]
    internal class DeeplTranslatorConnectorSerializationException : DeeplTranslatorConnectorException
    {
        public DeeplTranslatorConnectorSerializationException()
        {
        }

        public DeeplTranslatorConnectorSerializationException(string message) : base(message)
        {
        }

        public DeeplTranslatorConnectorSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeeplTranslatorConnectorSerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}