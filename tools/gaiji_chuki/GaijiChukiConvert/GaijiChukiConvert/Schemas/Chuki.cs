﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// このソース コードは xsd によって自動生成されました。Version=4.8.3928.0 です。
// 
namespace GaijiChukiConvert.Schemas {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd", IsNullable=false)]
    public partial class dictionary : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public dictionaryKanji kanji;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class dictionaryKanji : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("page")]
        public page[] page;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd", IsNullable=false)]
    public partial class page : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public pageRadical radical;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("entry", IsNullable=false)]
        public entry[] entries;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class pageRadical : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public pageRadicalReadings readings;
        
        /// <remarks/>
        public pageRadicalCharacters characters;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class pageRadicalReadings : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("reading")]
        public string[] reading;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd", IsNullable=false)]
    public partial class entry : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public entryCharacters characters;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Compatible78Inclusion", typeof(entryCompatible78Inclusion))]
        [System.Xml.Serialization.XmlElementAttribute("designVariant", typeof(entryDesignVariant))]
        [System.Xml.Serialization.XmlElementAttribute("inclusionApplication", typeof(entryInclusionApplication))]
        [System.Xml.Serialization.XmlElementAttribute("inputable", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("integrationApplication", typeof(entryIntegrationApplication))]
        public object Item;
        
        /// <remarks/>
        public note note;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UCV")]
        public entryUCV[] UCV;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        [System.ComponentModel.DefaultValueAttribute("0")]
        public string refPage;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        [System.ComponentModel.DefaultValueAttribute("0")]
        public string docPage;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string strokes;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool duplicate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(entrySupplement.@default)]
        public entrySupplement supplement;
        
        public entry() {
            this.refPage = "0";
            this.docPage = "0";
            this.duplicate = false;
            this.supplement = entrySupplement.@default;
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryCharacters : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("character")]
        public string[] character;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryCompatible78Inclusion : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @ref;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryDesignVariant : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @ref;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryInclusionApplication : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("character", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("note", typeof(note))]
        public object Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("reference")]
        public entryInclusionApplicationReference[] reference;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd", IsNullable=false)]
    public partial class note : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public string full;
        
        /// <remarks/>
        public string description;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jisx0213", typeof(noteJisx0213))]
        [System.Xml.Serialization.XmlElementAttribute("unicode", typeof(noteUnicode))]
        public object Item;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class noteJisx0213 : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int level;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool levelSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int men;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool menSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ku;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool kuSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ten;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tenSpecified;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class noteUnicode : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string code;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryInclusionApplicationReference : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string page;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryIntegrationApplication : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("character", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("note", typeof(note))]
        public object Item;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class entryUCV : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string number;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public enum entrySupplement {
        
        /// <remarks/>
        supplementOnly,
        
        /// <remarks/>
        supplementCommon,
        
        /// <remarks/>
        @default,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/aozora2htmlSharp/blob/master/tools/gaiji_chuki/GaijiChu" +
        "kiConvert/GaijiChukiConvert/Schemas/Chuki.xsd")]
    public partial class pageRadicalCharacters : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("character")]
        public string[] character;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
