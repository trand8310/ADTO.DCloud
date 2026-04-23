using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Editions.Dto
{
    public class MoveTenantsToAnotherEditionDto
    {
        //[Range(1, Int32.MaxValue)]
        public Guid SourceEditionId { get; set; }

        //[Range(1, Int32.MaxValue)]
        public Guid TargetEditionId { get; set; }
    }
}