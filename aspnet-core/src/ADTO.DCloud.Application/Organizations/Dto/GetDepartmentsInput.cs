using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{
    /// <summary>
    /// 部门查询条件
    /// </summary>
    public class GetDepartmentsInput
    {
        public Guid? CompanyId { get; set; }
        public string Keyword { get; set; }

    }
}
