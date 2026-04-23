using System;
using System.Text.Json.Serialization;
using System.Web;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class ChangeEmailInput: IShouldNormalize
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public string EmailAddress { get; set; }
    
    [JsonIgnore]
    public string OldEmailAddress { get; set; }

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

            if (query["emailAddress"] != null)
            {
                EmailAddress = query["emailAddress"];
            }
            
            if (query["old"] != null)
            {
                OldEmailAddress = query["old"];
            }
        }
    }
}