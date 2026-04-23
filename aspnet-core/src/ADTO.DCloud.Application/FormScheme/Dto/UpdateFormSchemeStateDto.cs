using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 修改表单状态
    /// </summary>
    public class UpdateFormSchemeStateDto
    {
        /// <summary>
        /// 表单Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 状态 
        /// </summary>
        public bool state { get; set; }
    }
}
