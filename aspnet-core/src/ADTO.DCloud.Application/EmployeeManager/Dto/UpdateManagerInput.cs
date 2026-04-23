using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateManagerInput
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public List<Guid> Ids { get; set; }

        /// <summary>
        /// 直属上级Id
        /// </summary>
        public Guid UserManagerId { get; set; }
    }
}
