using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace tBot.OpenAI
{

    public class Chat
    {
        #region field

        private string uri = "https://api.openai.com/v1/chat/completions";
        private string model = "gpt-3.5-turbo";
        private Message userMessage;
        private TMP_Text textField;

        public List<Message> messages = new List<Message>();
        private Bot bot;

        #endregion
        #region class
        #region class Message
        public class Message
        {
            /// <summary>
            /// The role of the author of this message. One of system, user, or assistant.
            /// </summary>
            public string role { get; set; }
            public string content { get; set; }
            public string name { get; set; }

            public Message(string role, string content, string name = null)
            {
                this.role = role; this.content = content; this.name = name;
            }

        }
        #endregion
        #region class RequestBody

        /// <summary>
        /// a request body for chat
        /// definition of parameters -> https://platform.openai.com/docs/api-reference/chat/create
        /// </summary>
        public class RequestBody
        {


            #region property

            public string model { get; set; } = "gpt-3.5-turbo";
            public List<Message> messages;
            public float temperature { get; set; } = 1f;
            public float top_p { get; set; } = 1f;
            public int n { get; set; } = 1;
            public bool stream { get; set; } = false;
            public string stop { get; set; } = null;
            public int max_tokens { get; set; } = 200;
            public float presence_penalty { get; set; } = 0;
            public float frequency_penalty { get; set; } = 0;
            public string user { get; set; }

            #endregion

            public RequestBody(string model, List<Message> messages)
            {
                this.model = model;
                this.messages = messages;
            }
        }
        #endregion
        #endregion

        public Chat(Bot bot)
        {
            this.bot = bot;
        }
        public async Task ChatAsync(Message userMessage)
        {
            this.userMessage = userMessage;
            messages.Add(userMessage);
            bot.textField.text = string.Format("You: {0}", userMessage.content);
            await SendRequestAsync();
        }

        private async Task SendRequestAsync()
        {
            byte[] boundary = UnityWebRequest.GenerateBoundary();

            List<IMultipartFormSection> form = new List<IMultipartFormSection>();
                Chat.RequestBody requestBody = new Chat.RequestBody(model, messages);
            string jsonContent = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            string contentAsString = await stringContent.ReadAsStringAsync();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(contentAsString);

            using (UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST))
            {
                request.SetRequestHeader("Content-Type", $"application/json; boundary={Encoding.UTF8.GetString(boundary)}");
                request.SetRequestHeader("Authorization", $"Bearer {bot.bearerToken}");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                AsyncOperation asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone) await Task.Yield();
                JObject jObject = JObject.Parse(request.downloadHandler.text);
                string responseRole = jObject["choices"][0]["message"]["role"].ToString();
                string responseContent = jObject["choices"][0]["message"]["content"].ToString();
                Message responseMessage = new Message(responseRole, responseContent);
                Debug.Log(string.Format("role:{0} {1}", responseRole, responseContent));
                bot.textField.text = string.Format("You: {0}\n\nGuard: {1}", userMessage.content, responseContent);
            }
        }
    }
}
