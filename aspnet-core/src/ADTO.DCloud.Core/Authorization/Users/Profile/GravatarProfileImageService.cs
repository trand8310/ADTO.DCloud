using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Session;

namespace ADTO.DCloud.Authorization.Users.Profile
{
    public class GravatarProfileImageService : IProfileImageService, ITransientDependency
    {
        private readonly UserManager _userManager;
        private readonly IADTOSharpSession _adtosharpSession;

        public GravatarProfileImageService(
            UserManager userManager,
            IADTOSharpSession adtosharpSession)
        {
            _userManager = userManager;
            _adtosharpSession = adtosharpSession;
        }

        public async Task<string> GetProfilePictureContentForUser(UserIdentifier userIdentifier)
        {
            var user = await _userManager.GetUserAsync(userIdentifier);
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"http://DCloud-api.adtogroup.com:20980/img/avatar.jpg"))
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }
        
        private static string GetMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
