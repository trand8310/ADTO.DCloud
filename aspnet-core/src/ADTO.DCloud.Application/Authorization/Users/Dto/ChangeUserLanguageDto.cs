using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
