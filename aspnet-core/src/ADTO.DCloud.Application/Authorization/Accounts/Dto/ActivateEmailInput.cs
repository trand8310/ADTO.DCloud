using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class ActivateEmailInput: IShouldNormalize
{
    public Guid UserId { get; set; }

    public string ConfirmationCode { get; set; }

    /// <summary>
    /// Encrypted values for {TenantId}, {UserId} and {ConfirmationCode}
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
            var parameters = SimpleStringCipher.Instance.Decrypt(c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["userId"] != null)
            {
                UserId = Guid.Parse(query["userId"]);
            }

            if (query["confirmationCode"] != null)
            {
                ConfirmationCode = query["confirmationCode"];
            }
        }
    }
}