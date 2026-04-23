using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static List<EnumDto> GetEnumList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new EnumDto
                {
                    Name = e.ToString(),
                    Value = Convert.ToInt32(e),
                    Description = e.GetDescription()
                })
                .ToList();
        }

        public class EnumDto
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public string Description { get; set; }
        }
    }
}
