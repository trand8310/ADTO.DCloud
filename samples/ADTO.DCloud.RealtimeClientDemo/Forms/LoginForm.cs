using System.Net.Http.Json;
using System.Text.Json;
using ADTO.DCloud.RealtimeClientDemo.Models;

namespace ADTO.DCloud.RealtimeClientDemo.Forms;

public class LoginForm : Form
{
    private readonly TextBox _txtUser = new() { Text = "10159", PlaceholderText = "用户名", Dock = DockStyle.Top };
    private readonly TextBox _txtPwd = new() { Text = "123456", PlaceholderText = "密码", UseSystemPasswordChar = true, Dock = DockStyle.Top };
    private readonly Button _btnLogin = new() { Text = "登录", Dock = DockStyle.Top };
    private readonly Button _btnSetting = new() { Text = "设置", Dock = DockStyle.Top };
    private readonly Label _lbl = new() { Dock = DockStyle.Top, Height = 40 };
    private readonly ClientSettings _settings = AppSettingsStore.Load();

    public LoginForm()
    {
        Text = "登录";
        Width = 360;
        Height = 260;
        Controls.Add(_lbl); Controls.Add(_btnLogin); Controls.Add(_btnSetting); Controls.Add(_txtPwd); Controls.Add(_txtUser);
        _btnSetting.Click += (_, _) => new SettingsForm(_settings).ShowDialog(this);
        _btnLogin.Click += async (_, _) => await LoginAsync();
    }

    private async Task LoginAsync()
    {
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(_settings.ApiBaseUrl) };
            var req = new AuthRequest { UserName = _txtUser.Text.Trim(), Password = _txtPwd.Text };
            var response = await client.PostAsJsonAsync("/api/TokenAuth/Authenticate", req);
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            var envelope = JsonSerializer.Deserialize<AuthEnvelope>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (envelope?.Result is null)
            {
                _lbl.Text = "登录失败";
                return;
            }

            var main = new MainForm(_settings.ApiBaseUrl, envelope.Result);
            main.Show();
            Hide();
        }
        catch (Exception ex)
        {
            _lbl.Text = ex.Message;
        }
    }
}
