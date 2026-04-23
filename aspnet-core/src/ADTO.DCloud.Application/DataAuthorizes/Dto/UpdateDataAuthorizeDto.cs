using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Dto
{
    [AutoMap(typeof(DataAuthorize))]
    public class UpdateDataAuthorizeDto:EntityDto<Guid>
    {

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(120)]
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [StringLength(120)]
        public string Code { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// 1.普通权限 2.自定义表单权限
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        public Guid ObjectId { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        [StringLength(120)]
        public string ObjectName { get; set; }
        /// <summary>
        /// 对象类型1.角色2.用户
        /// </summary>
        public int? ObjectType { get; set; }
        /// <summary>
        /// 条件公式
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 权限备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
