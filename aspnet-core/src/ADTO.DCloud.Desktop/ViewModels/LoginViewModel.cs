using ADTO.DCloud.Desktop.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace ADTO.DCloud.Desktop.ViewModels;

public sealed class LoginViewModel : BindableBase
{
    private readonly IAuthApiClient _authApiClient;
    private readonly ISessionService _sessionService;
    private readonly IApplicationNavigator _navigator;
    private string _userName = "10159";
    private string _password = "123456";
    private string _statusMessage = "连接 http://localhost:44380/ · 等待身份校验";
    private bool _isBusy;

    public LoginViewModel(IAuthApiClient authApiClient, ISessionService sessionService, IApplicationNavigator navigator)
    {
        _authApiClient = authApiClient;
        _sessionService = sessionService;
        _navigator = navigator;
        LoginCommand = new DelegateCommand(async () => await LoginAsync(), CanLogin).ObservesProperty(() => UserName).ObservesProperty(() => Password).ObservesProperty(() => IsBusy);
    }

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public DelegateCommand LoginCommand { get; }

    private bool CanLogin() => !IsBusy && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "正在请求 /api/TokenAuth/Authenticate ...";
            var session = await _authApiClient.AuthenticateAsync(UserName, Password);
            _sessionService.SignIn(session);
            StatusMessage = "认证成功，正在进入办公中枢。";
            _navigator.ShowMainWindow();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
