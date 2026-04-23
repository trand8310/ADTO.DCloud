using ADTO.DCloud.FormScheme.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace ADTO.DCloud.FormScheme.Model
{
    public class FormInfoModel
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Ver { get; set; }

        /// <summary>
        /// 组件配置
        /// </summary>
        public List<FormComponent> Components { get; set; }

        /// <summary>
        /// 表单配置信息
        /// </summary>
        public FormConfig Form { get; set; }
    }

    public class FormConfig
    {
        /// <summary>
        /// 历史记录存储位置 0 无 1 公共 2 私有
        /// </summary>
        public string HistoryType { get; set; }
        /// <summary>
        /// 重复字段校验设置
        /// </summary>
        public List<FormRepeatModel> VRepeatList { get; set; }
    }
}
