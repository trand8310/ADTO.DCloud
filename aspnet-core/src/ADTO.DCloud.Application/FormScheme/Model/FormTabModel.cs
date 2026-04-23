using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Model
{
    /// <summary>
    /// 表单页签模型
    /// </summary>
    public class FormTabModel
    {
        /// <summary>
        /// 表单组件集合
        /// </summary>
        public List<FormComponentModel> Components { get; set; }
    }
}
