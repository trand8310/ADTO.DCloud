using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Abs.enums
{
    /// <summary>
    /// 考勤相关枚举
    /// </summary>
    public class EnumAttStatusType
    {
        public enum AttStatusType
        {
            正常,
            迟到,
            早退,
            未打卡,
            旷工半天,
            上午上班忘记打卡,
            上午上班忘记打卡审核中,
            上午上班迟到忘记打卡,
            上午上班迟到忘记打卡审核中,
            上午下班忘记打卡,
            上午下班忘记打卡审核中,
            下午上班忘记打卡,
            下午上班忘记打卡审核中,
            下午上班迟到忘记打卡,
            下午上班迟到忘记打卡审核中,
            下午下班忘记打卡,
            下午下班忘记打卡审核中,
            出差,
            出差审核中,
            外出,
            外出审核中,
            病假,
            病假审核中,
            事假,
            事假审核中,
            调休,
            调休审核中,
            产假,
            产假审核中,
            丧假,
            丧假审核中,
            婚假,
            婚假审核中,
            年假,
            年假审核中,
            新冠确诊,
            新冠确诊审核中,
            _2022年假补充,
            _2022年度年假补充审核中,
            钉钉外出,
            钉钉签到
        }
        public enum AbsType
        {
            事假 = 1,
            病假,
            年假,
            婚假,
            产假,
            丧假,
            调休,
            新冠确诊 = 8,
            _2022年假补充 = 9,
        }
        public enum AttType
        {
            上午上班忘记打卡 = 1,
            上午下班忘记打卡 = 2,
            下午上班忘记打卡 = 3,
            下午下班忘记打卡 = 4
        }
    }
}
