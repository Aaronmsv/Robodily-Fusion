using Microsoft.Speech.Recognition;

namespace KinectApp.Speech
{
    public abstract class SpeechRecognizer
    {
        public abstract void Recognize(RecognitionResult result);

        public abstract void LoadGrammar();

        public delegate void SpeechRecognizedEventHandler(object sender, SpeechArgs args);

        public event SpeechRecognizedEventHandler Recognized;

        protected SpeechRecognizer(SpeechRecognitionEngine speechRecognitionEngine)
        {
            SpeechRecognitionEngine = speechRecognitionEngine;
        }

        public SpeechRecognitionEngine SpeechRecognitionEngine { get; set; }

        protected virtual void OnSpeechRecognized(object sender, SpeechArgs e)
        {
            if (sender == null)
            {
                sender = this;
            }

            if (Recognized != null)
            {                
                Recognized(sender, e);
            }
        }
    }
}
