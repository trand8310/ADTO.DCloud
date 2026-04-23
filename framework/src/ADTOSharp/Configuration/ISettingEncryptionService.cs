using JetBrains.Annotations;

namespace ADTOSharp.Configuration
{
    public interface ISettingEncryptionService
    {
        [CanBeNull]
        string Encrypt([NotNull]SettingDefinition settingDefinition, [CanBeNull] string plainValue);

        [CanBeNull]
        string Decrypt([NotNull]SettingDefinition settingDefinition, [CanBeNull] string encryptedValue);
    }
}