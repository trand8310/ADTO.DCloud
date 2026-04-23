using ADTO.DCloud.EnumManager.EnumUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    /// <summary>
    /// 允许用户打卡规则,钉钉签到和钉钉打卡只能存在一种，不能相互存在
    /// </summary>
    public enum EnumUserCheckInRules
    {
        //允许用户打卡规则，默认所有类型，钉钉打卡限制时间(按照考勤区域设置时间来算)，钉钉签到不要求时间(上午下午各一条记录即可)
        /// <summary>
        /// 所有类型,，默认就是钉钉签到和考勤机记录，是指考勤打卡和钉钉签到或者是考勤机打卡和钉钉打卡2种类型
        /// </summary>
        [EnumDesc("考勤机")]
        [Description("考勤机")]
        All = 0,
        /// <summary>
        /// 钉钉签到
        /// </summary>
        [EnumDesc("钉钉签到或考勤机")]
        [Description("钉钉签到或考勤机")]
        SignIn = 1,
        /// <summary>
        /// 钉钉打卡，指钉钉里面的打卡规则
        /// </summary>
        [EnumDesc("钉钉打卡或考勤机")]
        [Description("钉钉打卡或考勤机")]
        CheckIn = 2,
        /// <summary>
        /// 钉钉打卡，指钉钉里面的打卡规则
        /// </summary>
        [EnumDesc("微信打卡或考勤机")]
        [Description("微信打卡或考勤机")]
        WeChat = 3,
    }

    public enum EnumUserCheckInType
    {
        //0：正常打卡，1：固定打卡（根据打卡记录生成对应的考勤数据）,2：排班打卡
        /// <summary>
        /// 正常打卡
        /// </summary>
        [EnumDesc("正常打卡")]
        [Description("正常打卡")]
        NormalPunch = 0,
        /// <summary>
        /// 固定打卡
        /// </summary>
        [EnumDesc("固定打卡")]
        [Description("固定打卡")]
        FixedPunch = 1,
        /// <summary>
        /// 排班打卡
        /// </summary>
        [EnumDesc("排班打卡")]
        [Description("排班打卡")]
        SchedulePunch = 2,
        /// <summary>
        /// 单休打卡
        /// </summary>
        [EnumDesc("单休打卡")]
        [Description("单休打卡")]
        SingleBreakPunch = 3,
        /// <summary>
        /// 大小周
        /// </summary>
        [EnumDesc("大小周")]
        [Description("大小周")]
        SizeWeek = 4,
        /// <summary>
        /// 不打卡
        /// </summary>
        [EnumDesc("不打卡")]
        [Description("不打卡")]
        NoCheckin = 5,
    }
}
