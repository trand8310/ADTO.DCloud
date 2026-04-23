using ADTOSharp.Configuration;

namespace ADTOSharp.AutoMapper;

public class MultiLingualMapContext
{
    public ISettingManager SettingManager { get; set; }

    public MultiLingualMapContext(ISettingManager settingManager)
    {
        SettingManager = settingManager;
    }
}