using Google.Cloud.Translation.V2;

namespace tBot.GoogleTranslation
{
    static class Translator
    {
        private static string api_key = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

        /// <summary>
        /// Translates the input text into the specified target language using the Google Cloud Translation API.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="targetLanguage">The target language code (e.g., 'es' for Spanish).</param>
        /// <returns>The translated text in the target language.</returns>
        public static string TranslateText(string text, string targetLanguage)
        {
            TranslationClient client = TranslationClient.Create();
            TranslationResult result = client.TranslateText(text, targetLanguage);
            return result.TranslatedText;
        }
    }
}


//public class GoogleTranslation : MonoBehaviour
//{
//    private void Start()
//    {
//        // Set the environment variable
//        //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "path/to/your/service-account-key.json");
//        string api_key = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
//        Debug.Log(api_key);

//        // Translate text
//        string textToTranslate = "Hello, world!";
//        string targetLanguage = "es";
//        string translatedText = TranslateText(textToTranslate, targetLanguage);

//        Debug.Log(translatedText);
//    }

//    private string TranslateText(string text, string targetLanguage)
//    {
//        TranslationClient client = TranslationClient.Create();
//        TranslationResult result = client.TranslateText(text, targetLanguage);

//        return result.TranslatedText;
//    }
//}
