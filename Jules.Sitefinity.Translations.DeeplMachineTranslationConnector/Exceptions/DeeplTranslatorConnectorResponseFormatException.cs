using System;
using System.Runtime.Serialization;

namespace Jules.Sitefinity.Translations.DeeplMachineTranslationConnector.Exceptions
{
    [Serializable]
    internal class DeeplTranslatorConnectorResponseFormatException : DeeplTranslatorConnectorException
    {
        public DeeplTranslatorConnectorResponseFormatException()
        {
        }

        public DeeplTranslatorConnectorResponseFormatException(string message) : base(message)
        {
        }

        public DeeplTranslatorConnectorResponseFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeeplTranslatorConnectorResponseFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}