using ADTO.DCloud.RealtimeClientDemo.Models;

namespace ADTO.DCloud.RealtimeClientDemo.Forms;

public class SettingsForm : Form
{
    private readonly TextBox _txtApi = new() { Dock = DockStyle.Top };
    private readonly Button _btnSave = new() { Text = "保存", Dock = DockStyle.Bottom };

    public SettingsForm(ClientSettings settings)
    {
        Text = "设置";
        Width = 420;
        Height = 140;
        _txtApi.Text = settings.ApiBaseUrl;
        Controls.Add(_txtApi);
        Controls.Add(_btnSave);
        _btnSave.Click += (_, _) => { settings.ApiBaseUrl = _txtApi.Text.Trim(); AppSettingsStore.Save(settings); DialogResult = DialogResult.OK; Close(); };
    }
}
