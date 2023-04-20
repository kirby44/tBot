using System;
using System.Collections.Generic;

namespace tBot.OpenAI
{
    public class Chat
    {
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
        /// <summary>
        /// a request body for chat
        /// definition of parameters -> https://platform.openai.com/docs/api-reference/chat/create
        /// </summary>
        #region class RequestBody
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
            public float presence_penalty { get; set;} = 0;
            public float frequency_penalty { get; set;} = 0;
            public string user { get; set; }
    
            #endregion

            public RequestBody(string model, List<Message> messages)
            {
                this.model = model;
                this.messages = messages;
            }
        }
        #endregion
        /*public Chat()
    	{
    	}*/
    }
}
