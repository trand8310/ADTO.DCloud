using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{
    public class GetOrganizationUnitTreeInput
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 组织Id
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
