using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WzComparerR2.Config
{
    public class CharaSimMiscConfig : ConfigurationElement
    {
        [ConfigurationProperty("enableWorldArchive", DefaultValue = true)]
        public bool EnableWorldArchive
        {
            get { return (bool)this["enableWorldArchive"]; }
            set { this["enableWorldArchive"] = value; }
        }

        [ConfigurationProperty("locatePetEquip", DefaultValue = false)]
        public bool LocatePetEquip
        {
            get { return (bool)this["locatePetEquip"]; }
            set { this["locatePetEquip"] = value; }
        }

        [ConfigurationProperty("enable22AniStyle", DefaultValue = false)]
        public bool Enable22AniStyle
        {
            get { return (bool)this["enable22AniStyle"]; }
            set { this["enable22AniStyle"] = value; }
        }

        [ConfigurationProperty("PreferredStringCopyMethod", DefaultValue = false)]
        public bool PreferredStringCopyMethod
        {
            get { return (bool)this["PreferredStringCopyMethod"]; }
            set { this["PreferredStringCopyMethod"] = value; }
        }
    }
}