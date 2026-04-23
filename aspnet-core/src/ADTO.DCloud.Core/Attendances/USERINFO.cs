using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances
{

    /// <summary>
    /// 考勤机用户
    /// </summary>
    [Table("temp_USERINFO")]
    public class USERINFO : Entity<int>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Column("USERID")]
        public override int Id { get; set; }

        public int USERID => Id;
        /// <summary>
        /// 工号
        /// </summary>
        public string BADGENUMBER { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
    }
}
