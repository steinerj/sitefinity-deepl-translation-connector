Sitefinity Machine Translation Connector for DeepL API
===========================================

This project was ~~largely copied~~ heavily inspired by https://github.com/Sitefinity/microsoft-machine-translation-connector/
I am not a .NET developer so please feel free to apply any quality improvements to any of my ~~grave mistakes~~ code idiosyncrasies 


>**Tested Sitefinity versions**: Sitefinity CMS 15.2.8425 (works down to Sitefinity 14.1 but untested)

>**Documentation articles**: [Custom translation connector](http://www.progress.com/documentation/sitefinity-cms/custom-translation-connector)

>**IMPORTANT**: This repository may not be compatible with the latest or your current Sitefinity CMS version. If you want to use the repository with a specific Sitefinity CMS version, either upgrade the code from this repository or your Sitefinity CMS project to ensure compatibility.<br/>

### Overview
In addition to the built-in *Translation* module connectors, you can implement your own translation connector with custom logic to serve your requirements.

This project provides a translation connector to work with the popular [DeepL V2 API service](https://developers.deepl.com/docs). 
**The Connector also works with the Free Tier plan from Deepl so [head over and sign up!](https://www.deepl.com).**
This project uses the DeepL REST API instead of the [DeepL .NET Library](https://github.com/DeepLcom/deepl-dotnet) due to lack of .NET Framework 4.8 compatibility.

### Prerequisites
- Licensed Sitefinity CMS installation/PaaS.
- Your setup complies with the system requirements.
 For more information, see the [System requirements](https://docs.sitefinity.com/system-requirements) for the respective Sitefinity CMS version.
- Signed up for DeepL's API service (Free Tier or Pro)
 Then you can use the DeepL API key to configure the connector in Sitefinity CMS.
 
### Installation
Add the DeepL translation connector sample project to your solution.
 To do this, perform the following:
1. Open your Sitefinity CMS solution in Visual Studio.
2. Add `Jules.Sitefinity.Translations.DeeplMachineTranslatorConnector` project to the same solution.
3. Ensure `Telerik.Sitefinity.Translations` nuget package is installed in _Jules.Sitefinity.Translations.DeeplMachineTranslatorConnector_.
4. In _SitefinityWebApp_, add a reference to the _Jules.Sitefinity.Translations.DeeplMachineTranslatorConnector_ project.
5. Build your solution.

### Updating for your Sitefinity version
Run this command in the Visual Studio Package Manager Console to update the Telerik.Sitefinity.Translations package reference in the connector project to match your Sitefinity version:

```
Update-Package Telerik.Sitefinity.Translations -Version 15.2.8438
```

Alternatively, create an assembly binding redirect in your web.config file matching the Sitefinity version you are using, for example:

```
<dependentAssembly>
  <assemblyIdentity name="Telerik.Sitefinity.Translations" publicKeyToken="b28c218413bdf563" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-15.2.8438" newVersion="15.2.8438" />
</dependentAssembly>
```

## Configure the connector
Using one of the generated API keys, configure the connector in the following way:
1. In Sitefinity CMS backend, navigate to _Administration » Settings » Advanced_.
2. In the treeview on the left, expand _Translations » Connectors » DeeplMachineTranslatorConnector » Parameters_.
3. Set the _apiKey_ parameter to the API key provided by DeepL.
4. Set the _baseURL_ to the URLs for DeepL API v2.  For Pro/paying DeepL users this is: https://api.deepl.com/v2 _(Omit the trailing slash!)_  For free Tier users it's https://api-free.deepl.com/v2 For more information, see https://developers.deepl.com/docs/api-reference/translate
5. The _queryString_ parameter is for future compatibility and should be left empty.
6. Navigate back to _DeeplMachineTranslatorConnector_.
7. Select _Enabled_ and deselect _Strip HTML tags_.
8. Save your changes.

