﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aozora.Console.Resources {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Aozora.Console.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
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
        ///   Check Newline (only allow CR+LF) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string CheckNewline_Desc {
            get {
                return ResourceManager.GetString("CheckNewline.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Convert Aozora text format to html に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string Command_Desc {
            get {
                return ResourceManager.GetString("Command.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Setting css files に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string CssFiles_Desc {
            get {
                return ResourceManager.GetString("CssFiles.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Setting gaiji directory に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string GaijiDir_Desc {
            get {
                return ResourceManager.GetString("GaijiDir.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Output file に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string HtmlFile_Desc {
            get {
                return ResourceManager.GetString("HtmlFile.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   html file に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string HtmlFile_Title {
            get {
                return ResourceManager.GetString("HtmlFile.Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Reduce memory usage に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string SaveMemory_Desc {
            get {
                return ResourceManager.GetString("SaveMemory.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Input file に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string TextFile_Desc {
            get {
                return ResourceManager.GetString("TextFile.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   text file に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string TextFile_Title {
            get {
                return ResourceManager.GetString("TextFile.Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Use JIS X 0213 character に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string UseJisx0213_Desc {
            get {
                return ResourceManager.GetString("UseJisx0213.Desc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Use Unicode character に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string UseUnicode_Desc {
            get {
                return ResourceManager.GetString("UseUnicode.Desc", resourceCulture);
            }
        }
    }
}
