using System;
using System.Diagnostics;
using System.Linq;
using KinectApp.Properties;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KinectApp.Speech
{
    public class SpeechRecognition
    {
        private const double ConfidenceThreshold = 0.1;

        private KinectAudioSource _audioSource;
        private RecognizerInfo _ri;
        private SpeechRecognitionEngine _speechRecognitionEngine;
        private SpeechRecognizer _speechRecognizer;

        public SpeechRecognition(KinectAudioSource audioSource)
        {
            RecognizerInfo = GetKinectRecognizer();

            //load audioSource
            KinectAudioSource = audioSource;

            if (_ri != null)
            {
                SpeechRecognitionEngine = new SpeechRecognitionEngine(_ri.Id);

                SpeechRecognizer = new NaoSpeechRecognizer(SpeechRecognitionEngine);
                SpeechRecognizer.LoadGrammar();
            }
            else
            {
                Console.WriteLine(Resources.Debug_RiNull);
            }
        }

        public void Start()
        {
            if (null == _audioSource)
            {
                Console.WriteLine(Resources.Debug_NoAudioSource);
                return;
            }

            _audioSource.NoiseSuppression = true;
            _audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            _audioSource.SoundSourceAngleChanged += SoundSourceChanged;

            _speechRecognitionEngine.SpeechRecognized += _spRecEng_SpeechRecognized;
            _speechRecognitionEngine.SpeechRecognitionRejected += _spRecEng_SpeechRecognitionRejected;

            var kinectStream = _audioSource.Start();
            _speechRecognitionEngine.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void _spRecEng_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine(Resources.Debug_DidntUnderstand);
        }

        private void SoundSourceChanged(object sender, SoundSourceAngleChangedEventArgs e)
        {
            // Console.WriteLine("Sound source angle changed");
        }


        void _spRecEng_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            if (e.Result.Confidence < ConfidenceThreshold)
            {
                //engine is not confident about result => ignore
                Console.WriteLine(Resources.Debug_NotConfident);
                return;
            }

            Debug.WriteLine(e.Result.Text);

            SpeechRecognizer.Recognize(e.Result);  
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.OrdinalIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        public RecognizerInfo RecognizerInfo
        {
            get { return _ri; }
            private set { _ri = value; }
        }

        public SpeechRecognitionEngine SpeechRecognitionEngine
        {
            get { return _speechRecognitionEngine; }
            private set { _speechRecognitionEngine = value; }
        }

        public KinectAudioSource KinectAudioSource
        {
            get { return _audioSource; }
            set { _audioSource = value; }
        }

        public SpeechRecognizer SpeechRecognizer
        {
            get { return _speechRecognizer;  }
            set { _speechRecognizer = value; }
        }
    }
}
