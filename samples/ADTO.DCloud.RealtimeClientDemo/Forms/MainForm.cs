using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ADTO.DCloud.RealtimeClientDemo.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ADTO.DCloud.RealtimeClientDemo.Forms;

public class MainForm : Form
{
    private readonly AuthResult _auth;
    private readonly string _api;
    private readonly ComboBox _cmb = new() { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _btnConnect = new() { Text = "连接", Dock = DockStyle.Top };
    private readonly Button _btnHeartbeat = new() { Text = "发送心跳", Dock = DockStyle.Top };
    private readonly TextBox _txtLog = new() { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
    private HubConnection? _hub;
    private ClientWebSocket? _ws;

    public MainForm(string apiBaseUrl, AuthResult auth)
    {
        _api = apiBaseUrl.TrimEnd('/');
        _auth = auth;
        Text = "主界面";
        Width = 700;
        Height = 500;
        _cmb.Items.AddRange(["SignalR", "WebSocket"]);
        _cmb.SelectedIndex = 0;
        Controls.Add(_txtLog); Controls.Add(_btnHeartbeat); Controls.Add(_btnConnect); Controls.Add(_cmb);
        _btnConnect.Click += async (_, _) => await ConnectAsync();
        _btnHeartbeat.Click += async (_, _) => await SendHeartbeatAsync();
        Log($"AccessToken(调用授权API): {_auth.AccessToken[..Math.Min(20, _auth.AccessToken.Length)]}...");
        Log($"EncryptedAccessToken(SignalR/WebSocket): {_auth.EncryptedAccessToken[..Math.Min(20, _auth.EncryptedAccessToken.Length)]}...");
    }

    private async Task ConnectAsync()
    {
        if ((_cmb.SelectedItem?.ToString()) == "SignalR")
        {
            _hub = new HubConnectionBuilder()
                .WithUrl($"{_api}/signalr-chat", o => o.AccessTokenProvider = () => Task.FromResult(_auth.EncryptedAccessToken)!)
                .WithAutomaticReconnect()
                .Build();

            _hub.On<object>("getChatMessage", payload => Log("[SignalR] getChatMessage: " + JsonSerializer.Serialize(payload)));
            _hub.On<object>("ReceiveBroadcastMessage", payload => Log("[SignalR] ReceiveBroadcastMessage: " + JsonSerializer.Serialize(payload)));
            await _hub.StartAsync();
            Log("SignalR 已连接");
        }
        else
        {
            _ws = new ClientWebSocket();
            _ws.Options.SetRequestHeader("Authorization", $"Bearer {_auth.EncryptedAccessToken}");
            await _ws.ConnectAsync(new Uri($"{_api.Replace("http", "ws")}/ws-chat"), CancellationToken.None);
            _ = Task.Run(ReceiveWsLoopAsync);
            Log("WebSocket 已连接");
        }
    }

    private async Task SendHeartbeatAsync()
    {
        if ((_cmb.SelectedItem?.ToString()) == "SignalR" && _hub is not null)
        {
            await _hub.InvokeAsync("Heartbeat");
            Log("SignalR Heartbeat sent");
            return;
        }

        if (_ws is not null && _ws.State == WebSocketState.Open)
        {
            var request = JsonSerializer.Serialize(new ChatWebSocketRequest { Action = "heartbeat" });
            var bytes = Encoding.UTF8.GetBytes(request);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            Log("WebSocket heartbeat sent");
        }
    }

    private async Task ReceiveWsLoopAsync()
    {
        if (_ws is null) return;
        var buffer = new byte[8192];
        while (_ws.State == WebSocketState.Open)
        {
            var rs = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var msg = Encoding.UTF8.GetString(buffer, 0, rs.Count);
            BeginInvoke(() => Log("[WebSocket] " + msg));
        }
    }

    private void Log(string msg) => _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
}
