using System.Collections.Generic;
using Microsoft.Speech.Recognition;

namespace KinectApp.Speech
{
    public class NaoSpeechRecognizer : SpeechRecognizer
    {
        public NaoSpeechRecognizer(SpeechRecognitionEngine speechRecognitionEngine)
            : base(speechRecognitionEngine)
        {
        }

        private readonly Dictionary<string, WhatSaid> _singlePhrases = new Dictionary<string, WhatSaid>
        {
            {"Stop", new WhatSaid {Action = Action.Stop}},
            {"Start", new WhatSaid {Action = Action.Start}},
            {"Wave", new WhatSaid {Action = Action.Wave}},
            {"Stand", new WhatSaid {Action = Action.Stand}},
            {"Sing", new WhatSaid {Action = Action.Sing}},
            {"Sit", new WhatSaid {Action = Action.Sit}},
            {"Show", new WhatSaid {Action = Action.Show}},
            {"Hide", new WhatSaid {Action = Action.Hide}}
        };

        private readonly Dictionary<string, WhatSaid> _actionPhrases = new Dictionary<string, WhatSaid>
        {
            {"Move", new WhatSaid { Action = Action.Move} },
            {"Turn", new WhatSaid { Action = Action.Turn} },
        };

        private readonly Dictionary<string, WhatSaid> _limbPhrases = new Dictionary<string, WhatSaid>
        {
            {"Right Arm", new WhatSaid {Action = Action.MoveArm, LimbSide = LimbSide.Right}},
            {"Left Arm", new WhatSaid {Action = Action.MoveArm, LimbSide = LimbSide.Left}},
            {"Right Leg", new WhatSaid {Action = Action.MoveLeg, LimbSide = LimbSide.Right}},
            {"Left Leg", new WhatSaid {Action = Action.MoveLeg, LimbSide = LimbSide.Left}},
        };

        private readonly Dictionary<string, WhatSaid> _directionPhrases = new Dictionary<string, WhatSaid>
        {
            {"Down", new WhatSaid { Action = Action.Direction, Direction = Direction.Down} },
            {"Up", new WhatSaid { Action = Action.Direction, Direction = Direction.Up} },
            {"Forward", new WhatSaid { Action = Action.Direction, Direction = Direction.Up} },
            {"Backward", new WhatSaid { Action = Action.Direction, Direction = Direction.Down} },
            {"Left", new WhatSaid { Action = Action.Direction, Direction = Direction.Left} },
            {"Right", new WhatSaid { Action = Action.Direction, Direction = Direction.Right} },
            {"Around", new WhatSaid { Action = Action.Direction, Direction = Direction.Around} },
            {"Initial", new WhatSaid { Action = Action.Direction, Direction = Direction.Init} },
        };

        public override void Recognize(RecognitionResult result)
        {            
            //var allDicts = new List<Dictionary<string, WhatSaid>> { _actionPhrases, _limbPhrases, _directionPhrases, _singlePhrases };
            var arguments = new List<WhatSaid>();

            WhatSaid value;
            //single phrases
            bool found = _singlePhrases.TryGetValue(result.Text, out value);
            if (found)
            {
                foreach (var action in _singlePhrases)
                {
                    if (action.Value.Action.Equals(value.Action))
                    {
                        arguments.Add(value);
                        OnSpeechRecognized(this, new SpeechArgs(arguments));
                        return;
                    }
                }
            }

            //movement phrases
            var movementDicts = new List<Dictionary<string, WhatSaid>>
            {
                _actionPhrases,
                _limbPhrases, //limbs are optional so it will skip when not found
                _directionPhrases
            };

            for (int i = 0; i < movementDicts.Count; ++i)
            {
                foreach (var phrase in movementDicts[i])
                {
                    if (result.Text.Contains(phrase.Key))
                    {
                        arguments.Add(phrase.Value);
                        break;
                    }
                }
            }

            OnSpeechRecognized(this, new SpeechArgs(arguments));
        }

        public override void LoadGrammar()
        {
            var single = new Choices();
            foreach (var phrase in _singlePhrases)
            {
                single.Add(phrase.Key);
            }

            var actions = new Choices();
            foreach (var phrase in _actionPhrases)
            {
                actions.Add(phrase.Key);
            }

            var limbs = new Choices();
            foreach (var limb in _limbPhrases)
            {
                limbs.Add(limb.Key);
            }

            var directions = new Choices();
            foreach (var phrase in _directionPhrases)
            {
                directions.Add(phrase.Key);
            }

            //create action with directions
            var actionLimbsDirections = new GrammarBuilder();
            actionLimbsDirections.Append(actions);
            actionLimbsDirections.Append(limbs);
            actionLimbsDirections.Append(directions);

            var actionDirections = new GrammarBuilder();
            actionDirections.Append(actions);
            actionDirections.Append(directions);

            // add all
            var allChoices = new Choices();
            allChoices.Add(actionLimbsDirections);
            allChoices.Add(actionDirections);
            allChoices.Add(single);

            var gb = new GrammarBuilder { Culture = SpeechRecognitionEngine.RecognizerInfo.Culture };
            gb.Append(allChoices);

            var g = new Grammar(gb);
            SpeechRecognitionEngine.LoadGrammar(g);
        }

        public enum Action
        {
            Move,
            Turn,
            MoveArm,
            MoveLeg,
            MoveHead,
            Start,
            Stop,
            Wave,
            Stand,
            Sing,
            Sit,
            Direction,
            Show,
            Hide,
            Dance
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Around,
            Forward,
            Back,
            Init // To move back to initial position
        }

        public enum LimbSide
        {
            Left,
            Right
        }

        public struct WhatSaid
        {
            public Action Action;
            public Direction Direction;
            public LimbSide LimbSide;
        }
    }
}

