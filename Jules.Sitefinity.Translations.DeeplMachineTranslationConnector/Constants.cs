namespace Jules.Sitefinity.Translations.DeeplMachineTranslationConnector
{
    internal class Constants
    {
        public const string Name = "DeeplMachineTranslatorConnector";
        public const string Title = "Machine translation connector for Deepl";

        public const int SendTranslationRetryCount = 2;

        internal class ExceptionMessages
        {
            public const string InvalidApiKeyExceptionMessage = "Invalid Deepl API key.";
            public const string NoApiKeyExceptionMessage = "No API key configured for DeepL translator";
			public const string InvalidParameterForDeeplRequestExceptionMessagePrefix = "Invalid parameter for DeepL API request.";
            public const string NullOrEmptyParameterExceptionMessageTemplate = "Parameter with name {0} cannot be null or empty.";
            public static readonly string InvalidParameterForDeeplRequestExceptionMessageTemplate = InvalidParameterForDeeplRequestExceptionMessagePrefix + " " + NullOrEmptyParameterExceptionMessageTemplate;

            public static readonly string UnexpectedErrorResponseFormat = $"{DeeplServerErrorMessage} {UnexpectedResponseFormat}";
            public static readonly string ErrorSerializingErrorResponseFromServer = $"{DeeplServerErrorMessage} {ErrorSerializingResponseFromServer}";
            public static readonly string ErrorSerializingResponseFromServer = "Could not serialize response from Deepl API.";
            public const string UnexpectedResponseFormat = "The response received was not in the expected format.";

            public const string DeeplServerErrorMessage = "An error ocurred with the Deepl endpoint.";
        }

        internal struct ConfigParameters
        {
			public const string BaseUrl = "baseURL";
            public const string ApiKey = "apiKey";
			public const string QueryString = "queryString";
        }

        internal struct DeeplEndpointConstants
        {
            public const string DefaultEndpointUrl = "https://api.deepl.com/v2";
            public const string DefaultTranslateEndpoint = "/translate";
            public const string TargetCultureQueryParam = "to";
            public const string SourceCultureQueryParam = "from";

            //DeepL API has a limit of 128KiB per translate request.
            public const int MaxTranslateRequestSize = 131072;
        }
    }
}