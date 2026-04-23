using System.ComponentModel;

namespace ADTOSharp.Localization
{
    public class ADTOSharpDisplayNameAttribute : DisplayNameAttribute
    {
        public override string DisplayName => LocalizationHelper.GetString(SourceName, Key);

        public string SourceName { get; set; }
        public string Key { get; set; }

        public ADTOSharpDisplayNameAttribute(string sourceName, string key)
        {
            SourceName = sourceName;
            Key = key;
        }
    }
}
