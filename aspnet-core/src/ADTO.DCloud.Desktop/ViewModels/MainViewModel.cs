using ADTO.DCloud.Desktop.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace ADTO.DCloud.Desktop.ViewModels;

public sealed class MainViewModel : BindableBase
{
    private readonly ISessionService _sessionService;
    private readonly IApplicationNavigator _navigator;
    private readonly IClockService _clockService;
    private string _headline;
    private string _currentTime;

    public MainViewModel(ISessionService sessionService, IApplicationNavigator navigator, IClockService clockService)
    {
        _sessionService = sessionService;
        _navigator = navigator;
        _clockService = clockService;
        _headline = $"欢迎回来，{_sessionService.Current?.UserName ?? "DCloud User"}";
        _currentTime = _clockService.Now.ToString("yyyy-MM-dd HH:mm:ss");
        LogoutCommand = new DelegateCommand(Logout);
        RefreshCommand = new DelegateCommand(() => CurrentTime = _clockService.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    public string Headline
    {
        get => _headline;
        set => SetProperty(ref _headline, value);
    }

    public string CurrentTime
    {
        get => _currentTime;
        set => SetProperty(ref _currentTime, value);
    }

    public string TokenPreview
    {
        get
        {
            var token = _sessionService.Current?.AccessToken;
            return string.IsNullOrWhiteSpace(token) ? "未获取令牌" : $"{token[..Math.Min(token.Length, 18)]}••••••";
        }
    }

    public string UserId => _sessionService.Current?.UserId?.ToString() ?? "--";

    public DelegateCommand LogoutCommand { get; }

    public DelegateCommand RefreshCommand { get; }

    private void Logout()
    {
        _sessionService.SignOut();
        _navigator.ShowLoginWindow();
    }
}
