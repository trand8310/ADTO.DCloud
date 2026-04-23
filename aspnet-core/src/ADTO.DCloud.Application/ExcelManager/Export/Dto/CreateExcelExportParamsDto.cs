using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// EXCEL 导出请求参数配置表
    /// </summary>
    [AutoMapTo(typeof(ExcelExportParam))]
    public class CreateExcelExportParamsDto
    {
        /// <summary>
        ///  配置主表Id
        /// </summary>
        public Guid ExportId { get; set; }

        /// <summary>
        /// 参数名：DeptId / Status / KeyWord
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 显示名称：部门 / 状态 / 关键词
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 控件类型
        /// 1=输入框 2=下拉框 3=单个日期 4=日期区间 99=自定义组件
        /// </summary>
        public int ControlType { get; set; }

        /// <summary>
        /// 数据源类型(下拉框类型使用)
        /// 1=静态数据 2=数据字典 3=数据视图
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// 数据源配置
        /// 静态：1=启用,0=禁用
        /// 字典：Sex
        /// 远程：/api/department/getselect
        /// 枚举：ADTO.DCloud.StatusEnum
        /// </summary>
        public string SourceConfig { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
