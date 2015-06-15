using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using KinectApp.Extra;
using KinectApp.Robots;
using KinectApp.Speech;
using KinectApp.Tracking;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.WpfViewers;
using NLog;

namespace KinectApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static readonly DependencyProperty KinectSensorProperty =
            DependencyProperty.Register(
                "KinectSensor",
                typeof(KinectSensor),
                typeof(MainWindow),
                new PropertyMetadata(null));

        public KinectSensor KinectSensor
        {
            get { return (KinectSensor)GetValue(KinectSensorProperty); }
            set { SetValue(KinectSensorProperty, value); }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public SpeechRecognition SpeechRecognition { get; set; }
        private readonly KinectWindowViewModel _viewModel;
        private readonly ChangeTracker _changeTracker;
        private FaceTrackingViewer _faceTrackingViewer;
        private Skeleton[] _skeletonData;
        private readonly Storyboard _recordingAnimation;
        private readonly Storyboard _savedAnimation;
        public Boolean Programming { get; private set; }

        /// <summary>
        /// Initializes a new instance of the KinectWindow class, which provides access to many KinectSensor settings
        /// and output visualization.
        /// </summary>
        public MainWindow()
        {
            _viewModel = new KinectWindowViewModel();
            var robotMapper = new NaoInstructionMapper();
            _changeTracker = new ChangeTracker(robotMapper);

            // The KinectSensorManager class is a wrapper for a KinectSensor that adds
            // state logic and property change/binding/etc support, and is the data model
            // for KinectDiagnosticViewer.
            _viewModel.KinectSensorManager = new KinectSensorManager();
            // Handle kinect sensor changes 
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            //start skeleton tracking
            _viewModel.KinectSensorManager.SkeletonStreamEnabled = true;
            //set binding
            var sensorBinding = new Binding("KinectSensor") { Source = this };
            BindingOperations.SetBinding(_viewModel.KinectSensorManager, KinectSensorManager.KinectSensorProperty,
                sensorBinding);

            DataContext = _viewModel;
            Loaded += (sender, args) => InitializeKinect();
            InitializeComponent();

            // Register animations
            AnimationCanvas.Visibility = Visibility.Hidden;
            _recordingAnimation = FindResource("FlyIn") as Storyboard;
            _savedAnimation = FindResource("RecordingSaved") as Storyboard;
            _savedAnimation.Completed += (sender, args) =>
            {
                _savedAnimation.Stop();
                AnimationCanvas.Visibility = Visibility.Hidden;
            };
        }

        private void InitializeKinect()
        {
            // Get current running sensor
            KinectSensor sensor =
                KinectSensor.KinectSensors.FirstOrDefault(sens => sens.Status == KinectStatus.Connected);
            if (sensor != null)
            {
                // Save instance
                KinectSensor = sensor;
                // Start sensor
                try
                {
                    Logger.Info("Starting kinect");
                    KinectSensor.Start();
                }
                //todo display an error message on screen
                catch (InvalidOperationException)
                {
                    Logger.Error("Something went wrong while starting the kinect!");
                    //e.g. unplugged in the middle
                }
                catch (IOException)
                {
                    Logger.Error("Kinect already in use by another program!");
                }
                catch (Exception)
                {
                    Logger.Error("Something went wrong while starting the kinect!");
                }
                // Start speech
                SpeechRecognition = new SpeechRecognition(KinectSensor.AudioSource);
                SpeechRecognition.SpeechRecognizer.Recognized += OnSpeechRecognized;
                SpeechRecognition.Start();

                //Face tracking
                _faceTrackingViewer = new FaceTrackingViewer(KinectSensor, _changeTracker);

                // Identify the sensor based on the USB port it is plugged into.
                Logger.Info(Properties.Resources.Debug_KinectConnected, sensor.DeviceConnectionId);
                _viewModel.MessageViewVisibility = Visibility.Hidden;
            }
        }

        private void OnSpeechRecognized(object sender, SpeechArgs e)
        {
            //start with the first keyword
            if (e.Said.Count <= 0)
                return;

            switch (e.Said[0].Action)
            {
                case NaoSpeechRecognizer.Action.Stop:

                    if (Programming)
                    {
                        Logger.Debug("Stopped recording");
                        //Start animation
                        _savedAnimation.Begin();
                        _viewModel.KinectSensorManager.KinectSensor.SkeletonFrameReady -= OnSkeletonFrameReady;
                        KinectSensor.AllFramesReady -= _faceTrackingViewer.OnAllFramesReady;
                        try
                        {
                            FileRepository.SaveProgram(JsonHelper.Serialize(_changeTracker.Instructions));
                        }
                        catch (Exception)
                        {
                            Logger.Error("Could not save program to disk!");
                            //todo show message on screen
                        }
                        _changeTracker.Reset();
                        Programming = false;
                    }
                    return;
                case NaoSpeechRecognizer.Action.Start:
                    if (!Programming)
                    {
                        Logger.Debug("Started recording");
                        //Start animation
                        AnimationCanvas.Visibility = Visibility.Visible;
                        _recordingAnimation.Begin();
                        _changeTracker.StartRecording();
                        _viewModel.KinectSensorManager.KinectSensor.SkeletonFrameReady += OnSkeletonFrameReady;
                        KinectSensor.AllFramesReady += _faceTrackingViewer.OnAllFramesReady;
                        Programming = true;
                    }
                    return;
                case NaoSpeechRecognizer.Action.Show:
                    _viewModel.SkeletonViewEnabled = _viewModel.KinectSensorManager.ColorStreamEnabled;
                    return;
                case NaoSpeechRecognizer.Action.Hide:
                    _viewModel.SkeletonViewEnabled = false;
                    return;
            }

            //if programming, continue with programmable commands
            if (!Programming)
                return;

            switch (e.Said[0].Action)
            {
                case NaoSpeechRecognizer.Action.Move:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.VerbalMove, Direction = e.Said[1].Direction });
                    return;
                case NaoSpeechRecognizer.Action.MoveArm:
                    //Up: 10 degrees, Down: 85 degrees
                    _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(
                            e.Said[1].LimbSide == NaoSpeechRecognizer.LimbSide.Left ? BodyPart.LeftShoulderPitch : BodyPart.RightShoulderPitch,
                            e.Said[1].Direction == NaoSpeechRecognizer.Direction.Up ? 0.174532925f : 1.48352986f
                        ));
                    return;
                case NaoSpeechRecognizer.Action.MoveLeg:
                    //Up: 7.4degrees, Down: -40 degrees
                    _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(
                            e.Said[1].LimbSide == NaoSpeechRecognizer.LimbSide.Left ? BodyPart.LeftHipPitch : BodyPart.RightHipPitch,
                            e.Said[1].Direction == NaoSpeechRecognizer.Direction.Up ? -0.698131701f : 0.129154365f
                        ));
                    return;
                case NaoSpeechRecognizer.Action.MoveHead:
                    switch (e.Said[1].Direction)
                    {
                        case NaoSpeechRecognizer.Direction.Forward:
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadPitch, 0.514872129f)); //29.5 degrees
                            break;
                        case NaoSpeechRecognizer.Direction.Back:
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadPitch, -0.671951762f)); //-38.5 degrees
                            break;
                        case NaoSpeechRecognizer.Direction.Right:
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadYaw, 1.57079633f)); //90 degrees   
                            break;
                        case NaoSpeechRecognizer.Direction.Left:
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadYaw, -1.57079633f)); //-90 degrees   
                            break;
                        case NaoSpeechRecognizer.Direction.Init:
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadPitch, -0.0575958653f));
                            _changeTracker.Instructions.Add(_changeTracker.RobotMapper.Move(BodyPart.HeadYaw, 0.0f));
                            break;
                    }
                    return;
                case NaoSpeechRecognizer.Action.Turn:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Turn, Direction = e.Said[1].Direction });
                    return;
                case NaoSpeechRecognizer.Action.Sit:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Sit });
                    return;
                case NaoSpeechRecognizer.Action.Stand:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Stand });
                    return;
                case NaoSpeechRecognizer.Action.Wave:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Wave });
                    return;
                case NaoSpeechRecognizer.Action.Sing:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Sing });
                    return;
                case NaoSpeechRecognizer.Action.Dance:
                    _changeTracker.Instructions.Add(new Instruction { Command = Command.Dance });
                    return;
            }
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e == null)
                return;

            switch (e.Status)
            {
                case KinectStatus.Initializing:
                    _viewModel.KinectStatusText = Properties.Resources.Initializing;
                    break;
                case KinectStatus.Disconnected:
                    _viewModel.KinectStatusText = Properties.Resources.Disconnected;
                    _viewModel.MessageViewVisibility = Visibility.Visible;
                    break;
                case KinectStatus.Error:
                    _viewModel.KinectStatusText = Properties.Resources.ErrorOccurred;
                    break;
                case KinectStatus.Connected:
                    InitializeKinect();
                    break;
                default:
                    _viewModel.KinectStatusText = Properties.Resources.Reconnecting;
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (KinectSensor != null && KinectSensor.Status == KinectStatus.Connected)
            {
                KinectSensor.Stop();
                Console.WriteLine(Properties.Resources.Debug_StoppingKinect);
            }
            base.OnClosing(e);
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {
            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((_skeletonData == null) || (_skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        _skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(_skeletonData);

                    foreach (Skeleton skeleton in _skeletonData)
                    {
                        if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                        {
                            //Todo: fix interference when multiple, active skeletons
                            _changeTracker.CheckChanges(skeleton);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// A ViewModel for a KinectWindow.
    /// </summary>
    public class KinectWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(null));

        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        /// <summary>
        /// Shows or hides the skeleton view
        /// </summary>
        public static readonly DependencyProperty SkeletonViewEnabledProperty =
            DependencyProperty.Register(
                "SkeletonViewEnabled",
                typeof(bool),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(null));

        public bool SkeletonViewEnabled
        {
            get { return (bool)GetValue(SkeletonViewEnabledProperty); }
            set { SetValue(SkeletonViewEnabledProperty, value); }
        }

        /// <summary>
        /// Controls the visibility of  the status message
        /// </summary>
        public static readonly DependencyProperty MessageViewVisibilityProperty =
            DependencyProperty.Register(
                "MessageViewVisibility",
                typeof(Visibility),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(null));

        public Visibility MessageViewVisibility
        {
            get { return (Visibility)GetValue(MessageViewVisibilityProperty); }
            set { SetValue(MessageViewVisibilityProperty, value); }
        }

        /// <summary>
        /// Represents the Kinect connection status 
        /// </summary>
        public static readonly DependencyProperty KinectStatusTextProperty =
            DependencyProperty.Register(
                "KinectStatusText",
                typeof(string),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(null));

        public string KinectStatusText
        {
            get { return (string)GetValue(KinectStatusTextProperty); }
            set { SetValue(KinectStatusTextProperty, value); }
        }
    }
}
