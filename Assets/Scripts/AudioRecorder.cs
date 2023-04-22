using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

namespace tBot
{
	public class AudioRecorder : MonoBehaviour
	{
        #region field

        [SerializeField] public Bot bot;
        private Text textField;
        private Image progressBar;
        private bool isRecording;
        private readonly int duration = 5;
        private AudioClip clip;
        private float time;
        private TaskCompletionSource<AudioClip> tcs;

        #endregion

        #region method

        private void Start()
        {
            textField = bot.textField;
            progressBar = bot.progressBar;
        }

        public async Task<AudioClip> RecordAsync()
        {
            tcs = new TaskCompletionSource<AudioClip>();
            StartRecording();
            return await tcs.Task;
        }

        private void StartRecording()
        {
            isRecording = true;
            clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
        }

        private void EndRecording()
        {
            textField.text = "Transcripting...";
            Microphone.End(null);
            progressBar.fillAmount = 0;
            string fileName = "output.wav";
            byte[] data = SaveWav.Save(fileName, clip);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, data);
            print("Audio saved to: " + filePath);
            tcs.SetResult(clip);
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
        #endregion
    }
}

