using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.TrainingRoomRequireForm
{
    public interface ITrainingRoomDto
    {
        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime SDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EDate { get; set; }

        /// <summary>
        /// 会议室
        /// 字典
        /// </summary>
        [StringLength(128)]
        public string TrainingRoomName { get; set; }

    }
}
