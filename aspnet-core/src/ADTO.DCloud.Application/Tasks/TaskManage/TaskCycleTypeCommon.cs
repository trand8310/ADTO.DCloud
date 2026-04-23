using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace ADTO.DCloud.Tasks.TaskManage
{
    /// <summary>
    /// 解析器实现
    /// </summary>
    public class EveryDayParser : BaseCycleConfigParser
    {
        public EveryDayParser(ILogger<EveryDayParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "EveryDay".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue) => TimeSpan.FromDays(1);

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<EveryDayConfig>(cycleJsonValue);
            var nextTime = lastExecutionTime.Date
                .AddHours(config.Hour)
                .AddMinutes(config.Minute);

            return nextTime <= lastExecutionTime ? nextTime.AddDays(1) : nextTime;
        }

        private class EveryDayConfig
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
    }

    // NDayParser.cs - 每N天执行
    public class NDayParser : BaseCycleConfigParser
    {
        public NDayParser(ILogger<NDayParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "Nday".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            var config = DeserializeConfig<NDayConfig>(cycleJsonValue);
            return TimeSpan.FromDays(config.Day);
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<NDayConfig>(cycleJsonValue);
            return lastExecutionTime.Date
                .AddDays(config.Day)
                .AddHours(config.Hour)
                .AddMinutes(config.Minute);
        }

        private class NDayConfig
        {
            public int Day { get; set; }
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
    }

    // EveryHourParser.cs - 每小时固定分钟执行
    public class EveryHourParser : BaseCycleConfigParser
    {
        public EveryHourParser(ILogger<EveryHourParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "EveryHour".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue) => TimeSpan.FromHours(1);

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<EveryHourConfig>(cycleJsonValue);
            var nextTime = lastExecutionTime.Date
                .AddHours(lastExecutionTime.Hour)
                .AddMinutes(config.Minute);

            return nextTime <= lastExecutionTime ? nextTime.AddHours(1) : nextTime;
        }

        private class EveryHourConfig
        {
            public int Minute { get; set; }
        }
    }

    // NHourParser.cs - 每N小时执行
    public class NHourParser : BaseCycleConfigParser
    {
        public NHourParser(ILogger<NHourParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "Nhour".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            var config = DeserializeConfig<NHourConfig>(cycleJsonValue);
            return TimeSpan.FromHours(config.Hour);
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<NHourConfig>(cycleJsonValue);
            return lastExecutionTime
                .AddHours(config.Hour)
                .AddMinutes(config.Minute);
        }

        private class NHourConfig
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
    }

    // NMinuteParser.cs - 每N分钟执行
    public class NMinuteParser : BaseCycleConfigParser
    {
        public NMinuteParser(ILogger<NMinuteParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "Nminute".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            var config = DeserializeConfig<NMinuteConfig>(cycleJsonValue);
            return TimeSpan.FromMinutes(config.Minute);
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<NMinuteConfig>(cycleJsonValue);
            return lastExecutionTime.AddMinutes(config.Minute);
        }

        private class NMinuteConfig
        {
            public int Minute { get; set; }
        }
    }

    // EveryWeekParser.cs - 每周N执行
    public class EveryWeekParser : BaseCycleConfigParser
    {
        public EveryWeekParser(ILogger<EveryWeekParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "EveryWeek".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            var config = DeserializeConfig<EveryWeekConfig>(cycleJsonValue);
            return TimeSpan.FromDays(7 * config.Week);
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<EveryWeekConfig>(cycleJsonValue);
            var nextTime = lastExecutionTime.Date;

            while ((nextTime - lastExecutionTime).TotalDays < 1 ||
                   nextTime.DayOfWeek != (DayOfWeek)config.Week ||
                   nextTime <= lastExecutionTime)
            {
                nextTime = nextTime.AddDays(1);
            }

            return nextTime.AddHours(config.Hour).AddMinutes(config.Minute);
        }

        private class EveryWeekConfig
        {
            public int Week { get; set; } // 0-6 对应周日到周六
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
    }

    // EveryMonthParser.cs - 每月N日执行
    public class EveryMonthParser : BaseCycleConfigParser
    {
        public EveryMonthParser(ILogger<EveryMonthParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "EveryMonth".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            return TimeSpan.FromDays(30); // 近似值，实际计算在GetNextExecutionTime中
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<EveryMonthConfig>(cycleJsonValue);
            var nextTime = lastExecutionTime.Date
                .AddDays(-lastExecutionTime.Day + 1) // 当月第一天
                .AddMonths(1) // 下个月
                .AddDays(config.Day - 1) // 指定的日
                .AddHours(config.Hour)
                .AddMinutes(config.Minute);

            return nextTime <= lastExecutionTime ? nextTime.AddMonths(1) : nextTime;
        }

        private class EveryMonthConfig
        {
            public int Day { get; set; }
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
    }

    // NSecondParser.cs - 每N秒执行
    public class NSecondParser : BaseCycleConfigParser
    {
        public NSecondParser(ILogger<NSecondParser> logger) : base(logger) { }

        public override bool CanParse(string cycleType) =>
            "Nsecond".Equals(cycleType, StringComparison.OrdinalIgnoreCase);

        public override TimeSpan GetInterval(string cycleJsonValue)
        {
            var config = DeserializeConfig<NSecondConfig>(cycleJsonValue);
            return TimeSpan.FromSeconds(config.Second);
        }

        public override DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime)
        {
            var config = DeserializeConfig<NSecondConfig>(cycleJsonValue);
            return lastExecutionTime.AddSeconds(config.Second);
        }

        private class NSecondConfig
        {
            public int Second { get; set; }
        }
    }
}
