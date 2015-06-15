﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KinectApp.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KinectApp.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SPEECH: Couldn&apos;t understand input..
        /// </summary>
        public static string Debug_DidntUnderstand {
            get {
                return ResourceManager.GetString("Debug_DidntUnderstand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SPEECH: An error occurred while loading grammar..
        /// </summary>
        public static string Debug_GrammarError {
            get {
                return ResourceManager.GetString("Debug_GrammarError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kinect connected successfully with id {0}..
        /// </summary>
        public static string Debug_KinectConnected {
            get {
                return ResourceManager.GetString("Debug_KinectConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SPEECH: No audio source was given..
        /// </summary>
        public static string Debug_NoAudioSource {
            get {
                return ResourceManager.GetString("Debug_NoAudioSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SPEECH: Not confident enough, try again..
        /// </summary>
        public static string Debug_NotConfident {
            get {
                return ResourceManager.GetString("Debug_NotConfident", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SPEECH: RI WAS NULL..
        /// </summary>
        public static string Debug_RiNull {
            get {
                return ResourceManager.GetString("Debug_RiNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MAIN: Stopping Kinect sensors..
        /// </summary>
        public static string Debug_StoppingKinect {
            get {
                return ResourceManager.GetString("Debug_StoppingKinect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disconnected, trying to reconnect....
        /// </summary>
        public static string Disconnected {
            get {
                return ResourceManager.GetString("Disconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred!.
        /// </summary>
        public static string ErrorOccurred {
            get {
                return ResourceManager.GetString("ErrorOccurred", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;grammar version=&quot;1.0&quot; xml:lang=&quot;en-US&quot; root=&quot;rootRule&quot; tag-format=&quot;semantics/1.0-literals&quot; xmlns=&quot;http://www.w3.org/2001/06/grammar&quot;&gt;
        ///  &lt;rule id=&quot;rootRule&quot; scope=&quot;public&quot;&gt;
        ///    &lt;one-of&gt;
        ///      &lt;item&gt;
        ///        &lt;tag&gt;START&lt;/tag&gt;
        ///        &lt;one-of&gt;
        ///          &lt;item&gt;start&lt;/item&gt;
        ///          &lt;item&gt;start programming&lt;/item&gt;
        ///        &lt;/one-of&gt;
        ///      &lt;/item&gt;
        ///      
        ///      &lt;item&gt;
        ///        &lt;tag&gt;STOP&lt;/tag&gt;
        ///        &lt;one-of&gt;
        ///          &lt;item&gt;stop programming&lt;/item&gt;
        ///        &lt;/one-of&gt;
        ///      &lt;/item&gt;
        ///      
        ///      &lt;!--  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Grammars {
            get {
                return ResourceManager.GetString("Grammars", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing....
        /// </summary>
        public static string Initializing {
            get {
                return ResourceManager.GetString("Initializing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trying to communicate with Kinect....
        /// </summary>
        public static string Reconnecting {
            get {
                return ResourceManager.GetString("Reconnecting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please connect the Kinect.
        /// </summary>
        public static string WaitingForConnection {
            get {
                return ResourceManager.GetString("WaitingForConnection", resourceCulture);
            }
        }
    }
}