/**
Heavily inspired by https://github.com/Sitefinity/microsoft-machine-translation-connector/
 */

using Jules.Sitefinity.Translations.DeeplMachineTranslationConnector;
using Jules.Sitefinity.Translations.DeeplMachineTranslationConnector.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Translations;


[assembly: TranslationConnector(name: Constants.Name,
                                connectorType: typeof(DeeplMachineTranslatorConnector),
                                title: Constants.Title,
                                enabled: false,
                                removeHtmlTags: false,
                                parameters: new string[] { Constants.ConfigParameters.ApiKey,
                                    Constants.ConfigParameters.BaseUrl,
                                    Constants.ConfigParameters.QueryString})]
namespace Jules.Sitefinity.Translations.DeeplMachineTranslationConnector
{
    /// <summary>
    /// Connector for DeepL's Translation API Services - https://developers.deepl.com/docs 
    /// </summary>
    public class DeeplMachineTranslatorConnector : MachineTranslationConnector
    {
        protected virtual HttpClient GetClient()
        {
            return new HttpClient();
        }

        /// <summary>
        /// Configures the connector instance
        /// </summary>
        /// <param name="config">apiKey key should contain the DeepL API key</param>
        protected override void InitializeConnector(NameValueCollection config)
        {
            var key = config.Get(Constants.ConfigParameters.ApiKey);
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Constants.ExceptionMessages.NoApiKeyExceptionMessage);
            }

            this.key = key;

            var baseURL = config.Get(Constants.ConfigParameters.BaseUrl);
            if (string.IsNullOrEmpty(baseURL))
            {
                baseURL = Constants.DeeplEndpointConstants.DefaultEndpointUrl;
            }
            this.baseUrl = baseURL;

            var queryString = config.Get(Constants.ConfigParameters.QueryString);
            this.queryString = queryString;

            this.translateEndpoint = Constants.DeeplEndpointConstants.DefaultTranslateEndpoint;
    
            this.maxTranslateRequestSize = Constants.DeeplEndpointConstants.MaxTranslateRequestSize;
        }

        protected override List<string> Translate(List<string> input, ITranslationOptions translationOptions)
        {
            if (translationOptions == null)
            {
                throw new ArgumentException(GetTranslаteArgumentExceptionMessage(nameof(translationOptions)));
            }

            var fromLanguageCode = translationOptions.SourceLanguage;
            var toLanguageCode = translationOptions.TargetLanguage;

            if (string.IsNullOrWhiteSpace(fromLanguageCode))
            {
                throw new ArgumentException(GetTranslаteArgumentExceptionMessage($"{nameof(translationOptions)}.{nameof(translationOptions.SourceLanguage)}"));
            }

            if (string.IsNullOrWhiteSpace(toLanguageCode))
            {
                throw new ArgumentException(GetTranslаteArgumentExceptionMessage($"{nameof(translationOptions)}.{nameof(translationOptions.TargetLanguage)}"));
            }

            if (input == null || input.Count == 0)
            {
                throw new ArgumentException(GetTranslаteArgumentExceptionMessage(nameof(input)));
            }

            if (fromLanguageCode == toLanguageCode)
            {
                // TODO: Could potentially use DeepL's Improve text service in case of same language, but not sure if that has side effects - https://developers.deepl.com/docs/api-reference/improve-text
                return input;
            }

            return TranslateCore(input, translationOptions);
        }

        private List<string> TranslateCore(List<string> input, ITranslationOptions translationOptions)
        {
            int currentRetry = 0;
            var translations = new List<string>();

            while (true)
            {
                try
                {
                    translations = TryTranslate(input, translationOptions);
                    break;
                }
                catch (Exception ex)
                {
                    currentRetry++;

                    if (currentRetry > Constants.SendTranslationRetryCount)
                    {
                        throw ex;
                    }
                }
            }

            return translations;
        }

        /**
         * https://developers.deepl.com/docs/getting-started/your-first-api-request
        */
        private List<string> TryTranslate(List<string> input, ITranslationOptions translationOptions)
        {
            var fromLanguageCode = translationOptions.SourceLanguage;
            var toLanguageCode = translationOptions.TargetLanguage;
            string uri = this.baseUrl + this.translateEndpoint;

            var requestBodyObj = new
            {
                text = input.ToArray(),
                target_lang = toLanguageCode,
                source_lang = fromLanguageCode
            };
            var serializer = new JavaScriptSerializer();
            string requestBody = serializer.Serialize(requestBodyObj);
            
            if (Encoding.UTF8.GetByteCount(requestBody) >= this.maxTranslateRequestSize)
            {
                throw new DeeplTranslatorConnectorSerializationException("DeepL API request payload too large. Maximum request size is " + this.maxTranslateRequestSize + " bytes.");
            }

            using (var client = this.GetClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", "DeepL-Auth-Key " + this.key);

                var response = new HttpResponseMessage();
                var responseTask = client.SendAsync(request).ContinueWith(r => response = r.Result);
                responseTask.Wait();

                var responseBody = string.Empty;
                var responseBodyTask = response.Content.ReadAsStringAsync().ContinueWith(r => responseBody = r.Result);
                responseBodyTask.Wait();

                if (!response.IsSuccessStatusCode)
                {
                    this.HandleApiError(responseBody, response);
                }

                dynamic result;
                try
                {
                    result = serializer.DeserializeObject(responseBody);
                }
                catch (Exception ex)
                {
                    if (IsSerializationException(ex))
                    {
                        throw new DeeplTranslatorConnectorSerializationException($"{Constants.ExceptionMessages.ErrorSerializingResponseFromServer} Server response: {response.StatusCode} {response.ReasonPhrase} {responseBody}");
                    }

                    throw;
                }

                var translations = new List<string>();
                try
                {
                    for (int i = 0; i < input.Count(); i++)
                    {
                        // currently Sitefinity does not support sending multiple languages at once, only multiple strings
                        var translation = result["translations"][i]["text"];
                        translations.Add(translation);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is KeyNotFoundException || ex is NullReferenceException)
                    {
                        throw new DeeplTranslatorConnectorResponseFormatException($"{Constants.ExceptionMessages.UnexpectedResponseFormat} Server response: {response.StatusCode} {response.ReasonPhrase} {responseBody}");
                    }

                    throw;
                }

                return translations;
            }
        }


        protected virtual bool IsRemoveHtmlTagsEnabled()
        {
            return this.RemoveHtmlTags;
        }

        private static string GetTranslаteArgumentExceptionMessage(string paramName)
        {
            return string.Format(Constants.ExceptionMessages.InvalidParameterForDeeplRequestExceptionMessageTemplate, paramName);
        }

        private void HandleApiError(string responseBody, HttpResponseMessage response)
        {
            var serializer = new JavaScriptSerializer();
            dynamic jsonResponse;
            try
            {
                jsonResponse = serializer.DeserializeObject(responseBody);
            }
            catch (Exception ex)
            {
                if (IsSerializationException(ex))
                {
                    throw new DeeplTranslatorConnectorSerializationException($"{Constants.ExceptionMessages.ErrorSerializingErrorResponseFromServer} Server response: {response.StatusCode} {response.ReasonPhrase} {responseBody}");
                }

                throw;
            }

            try
            {
                throw new DeeplTranslatorConnectorException(jsonResponse["error"]["message"]);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is NullReferenceException)
                {
                    throw new DeeplTranslatorConnectorResponseFormatException($"{Constants.ExceptionMessages.UnexpectedErrorResponseFormat} Server response: {response.StatusCode} {response.ReasonPhrase} {responseBody}");
                }
                throw;
            }
        }

        private static bool IsSerializationException(Exception ex)
        {
            return ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException;
        }

        private string key;
        private string baseUrl;
        private string translateEndpoint;
        private string queryString;
        private int maxTranslateRequestSize;
    }
}