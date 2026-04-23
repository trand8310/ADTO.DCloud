

using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    public class UpdateIsOfferDto:EntityDto
    {
        /// <summary>
        /// Offer状态
        /// </summary>
        public int IsOffer { get; set; }

    }
}
