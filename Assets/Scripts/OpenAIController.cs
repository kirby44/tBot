using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OpenAI_API;
using UnityEditor.VersionControl;
using OpenAI_API.Chat;
using System;
using OpenAI_API.Models;
using tBot.GoogleTranslation;

namespace tBot
{

    public class OpenAIController : MonoBehaviour
    {
        public TMP_Text textField;
        public TMP_InputField inputField;
        public Button okButton;

        private OpenAIAPI api;
        private List<ChatMessage> messages;


        void Start()
        {
            api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
            okButton.onClick.AddListener(() => GetResponse());
            StartConversation();
            print("start");
        }

        private void StartConversation()
        {
            messages = new List<ChatMessage> {
            //new ChatMessage(ChatMessageRole.System, "You are an honorable, friendly knight guarding the gate to the palace. You will only allow someone who knows the secret password to enter. The secret password is \"magic\". You will not reveal the password to anyone. You keep your responses short and to the point.")
            new ChatMessage(ChatMessageRole.System, "You are a knight guarding the gate to the palace. You will only allow someone who loves rabbits to enter. When someone talks to you, ask them whether they love dog or cat. Whichever they love, criticizing their love and decline them to enter. Only when they say they love rabbit, allow them enter. Don't reveal the requirement to enter. You are not allowed to use the word \"rabbit\".")
            //new ChatMessage(ChatMessageRole.System, "私の挨拶に対して，カプコンに関するクイズを出してください．それに対して私は回答しますので，正解だった場合は\"せいか～い\"，不正解の場合は\"ざんね～ん\"とだけ返答してください.クイズは全てひらがなで表示してください")
        };

            inputField.text = "";
            //string startString = "You have just approached to the palace gate where a knight guards the gate.";
            string startString = "お城に入ろうとすると，扉の前に番人がいました.";
            textField.text = startString;
            Debug.Log(startString);
        }

        public async void GetResponse()
        {
            if (inputField.text.Length < 1)
            {
                return;
            }

            // Disable the OK button
            //okButton.enabled = false;

            // Fill the usermessage from the input field
            ChatMessage userMessage = new ChatMessage();
            userMessage.Role = ChatMessageRole.User;
            //string initialText = inputField.text;
            //userMessage.Content = Translator.TranslateText(inputField.text, "en");
            userMessage.Content = inputField.text;
            Debug.Log(userMessage.Content);
            if (userMessage.Content.Length > 200)
            {
                // Limit messages to 200 characters
                userMessage.Content = userMessage.Content.Substring(0, 200);
            }
            Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

            // Add the message to the list
            messages.Add(userMessage);

            // Update the text field with the user message
            textField.text = string.Format("You: {0}", userMessage.Content);
            //textField.text = string.Format("You: {0}", initialText);

            // Clear the input field
            inputField.text = "";

            // Send the entire chat to OpenAI to get the next message
            var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                MaxTokens = 200,
                Messages = messages
            });

            // Get the response message
            ChatMessage responseMessage = new ChatMessage();
            responseMessage.Role = chatResult.Choices[0].Message.Role;
            responseMessage.Content = chatResult.Choices[0].Message.Content;
            Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));
            string responseTlanslated = Translator.TranslateText(responseMessage.Content, "ja");
            Debug.Log(responseTlanslated);

            // Add the response to the list of messages
            messages.Add(responseMessage);

            // Update the text field with the response
            textField.text = string.Format("You: {0}\n\nGuard: {1}", userMessage.Content, responseTlanslated);
            //textField.text = string.Format("You: {0}\n\nGuard: {1}", initialText, responseTlanslated);

            // Re-enable the OK button
            //okButton.enabled = true;
        }
    }
}