﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KH3AssetHelper.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KH3AssetHelper.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to import os
        ///import bpy
        ///import sys
        ///import math
        ///from bpy.props import *
        ///if &apos;--&apos; in sys.argv:
        ///    argv = sys.argv[sys.argv.index(&apos;--&apos;) + 1:]
        ///
        ///print(argv)
        ///
        ///srcPath=argv[0]
        ///dstPath=argv[1]
        ///
        ///print(&quot;srcPath: &quot; + srcPath)
        ///print(&quot;dstPath: &quot; + dstPath)
        ///
        ///context = bpy.context
        ///scene = context.scene
        ///
        ///#clear the scene
        ///for c in scene.collection.children:
        ///    scene.collection.children.unlink(c)
        ///
        ///# put the location to the folder where the plys are located here in this fashion
        ///#error_file = parent_path  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string BlenderScript_ASCII2FBX {
            get {
                return ResourceManager.GetString("BlenderScript_ASCII2FBX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to import os
        ///import bpy
        ///import sys
        ///import math
        ///
        ///from bpy.props import *
        ///
        ///if &apos;--&apos; in sys.argv:
        ///    argv = sys.argv[sys.argv.index(&apos;--&apos;) + 1:]
        ///
        ///print(argv)
        ///
        ///srcPath=argv[0]
        ///dstPath=argv[1]
        ///pluginPath=argv[2]
        ///
        ///print(&quot;srcPath: &quot; + srcPath)
        ///print(&quot;dstPath: &quot; + dstPath)
        ///print(&quot;pluginPath: &quot; + pluginPath)
        ///
        ///context = bpy.context
        ///scene = context.scene
        ///
        ///sys.path.append(pluginPath)
        ///from pskpsab280 import *
        ///
        ///#clear the scene
        ///for c in scene.collection.children:
        ///    scene.collection.children.unli [rest of string was truncated]&quot;;.
        /// </summary>
        public static string BlenderScript_PSK2FBX {
            get {
                return ResourceManager.GetString("BlenderScript_PSK2FBX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        public static byte[] pskpsab280 {
            get {
                object obj = ResourceManager.GetObject("pskpsab280", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
