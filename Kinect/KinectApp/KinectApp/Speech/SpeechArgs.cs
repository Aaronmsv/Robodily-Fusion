using System;
using System.Collections.Generic;

namespace KinectApp.Speech
{
    public class SpeechArgs : EventArgs
    {
        public List<NaoSpeechRecognizer.WhatSaid> Said { get; set; }

        public SpeechArgs(List<NaoSpeechRecognizer.WhatSaid> said)
        {
            Said = said;
        }
    }
}
