using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Editions.Dto
{
    public class UpdateEditionDto
    {
        [Required]
        public EditionEditDto Edition { get; set; }

        [Required]
        public List<NameValueDto> FeatureValues { get; set; }
    }
}