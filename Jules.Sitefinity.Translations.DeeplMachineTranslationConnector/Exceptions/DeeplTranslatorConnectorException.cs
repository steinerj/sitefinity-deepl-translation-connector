using System;
using System.Runtime.Serialization;

namespace Jules.Sitefinity.Translations.DeeplMachineTranslationConnector.Exceptions
{
    /// <summary>
    /// Exceptions thrown when DeeplMachineTranslatorConnector requests to DeepL API receive error from server.
    /// </summary>
    [Serializable]
    public class DeeplTranslatorConnectorException : Exception
    {
        public DeeplTranslatorConnectorException()
        {
        }

        public DeeplTranslatorConnectorException(string message) : base(message)
        {
        }

        public DeeplTranslatorConnectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeeplTranslatorConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}