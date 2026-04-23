using System.Threading.Tasks;

namespace ADTO.DCloud.Security.Recaptcha;

public interface IRecaptchaValidator
{
    Task ValidateAsync(string captchaResponse);
}