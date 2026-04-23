using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.OA4PTest;
using ADTOSharp.AutoMapper;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 创建4p测试结果
    /// </summary>

    [AutoMap(typeof(OA4PStatistic))]
    public class CreateOA4PStatisticDto
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required(ErrorMessage = "真实姓名不能为空")]
        [StringLength(128)]
        public string TrueName { get; set; }
        /// <summary>
        /// 应聘职位
        /// </summary>
        [Required(ErrorMessage = "应聘职位不能为空")]
        [StringLength(128)]
        public string PostName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "手机号码不能为空")]
        [StringLength(128)]
        public string Mobile { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(128)]
        public string Type { get; set; }
        /// <summary>
        /// 邀请人
        /// </summary>
        public string Inviter { get; set; }



        ///// <summary>
        ///// 选项一选择个数
        ///// </summary>
        //public int? Option_1_Total { get; set; }
        ///// <summary>
        ///// 选项二选择个数
        ///// </summary>
        //public int? Option_2_Total { get; set; }
        ///// <summary>
        ///// 选项三选择个数
        ///// </summary>
        //public int? Option_3_Total { get; set; }
        ///// <summary>
        ///// 选项四选择个数
        ///// </summary>
        //public int? Option_4_Total { get; set; }
        ///// <summary>
        ///// 总答题个数
        ///// </summary>
        //public int? Option_Sum_Total { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateTimeConverter))]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 答题数据
        /// </summary>
        public List<OA4PDataDto> Data { get; set; }

    }
}
