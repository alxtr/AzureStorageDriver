﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Madd0.AzureStorageDriver.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Madd0.AzureStorageDriver.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AzureTableStorage.
        /// </summary>
        internal static string AssemblyTitle {
            get {
                return ResourceManager.GetString("AssemblyTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0.1.0.0.
        /// </summary>
        internal static string AssemblyVersion {
            get {
                return ResourceManager.GetString("AssemblyVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mauricio Diaz Orlich.
        /// </summary>
        internal static string AuthorName {
            get {
                return ResourceManager.GetString("AuthorName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Azure Table Storage Driver.
        /// </summary>
        internal static string DriverName {
            get {
                return ResourceManager.GetString("DriverName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (Development).
        /// </summary>
        internal static string LocalStorageDisplayName {
            get {
                return ResourceManager.GetString("LocalStorageDisplayName", resourceCulture);
            }
        }
    }
}
