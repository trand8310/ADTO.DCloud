using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ADTOSharp.Auditing;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class ResetPasswordInput: IShouldNormalize
{
    public Guid UserId { get; set; }

    public string ResetCode { get; set; }
    public DateTime ExpireDate { get; set; }

    [DisableAuditing]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    public string SingleSignIn { get; set; }

    /// <summary>
    /// Encrypted values for {TenantId}, {UserId}, {ResetCode} and {ExpireDate}
    /// </summary>
    public string c { get; set; }

    public void Normalize()
    {
        ResolveParameters();
    }

    protected virtual void ResolveParameters()
    {
        if (!string.IsNullOrEmpty(c))
        {
            try
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null)
                {
                    UserId = Guid.Parse(query["userId"]);
                }

                if (query["resetCode"] != null)
                {
                    ResetCode = query["resetCode"];
                }

                if (query["expireDate"] == null)
                {
                    throw new ADTOSharpValidationException();
                }
                
                ExpireDate = Convert.ToDateTime(query["expireDate"]);

            }
            catch (Exception e)
            {
                throw new ADTOSharpValidationException("Invalid reset password link!");
            }
        }
    }
}