using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    ///  新增、修改项目时，查重参数
    /// </summary>
    public class ProjectExistInfoDto
    {
        /// <summary>
        /// Id(编辑必传)
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        
    }
}
