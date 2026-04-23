using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    /// <summary>
    /// AttributeTargets.Method 表示此特性只能应用于方法。
    /// Inherited = false 表示该特性不会被子类继承。如果一个类有这个特性，子类的方法不会自动继承这个特性。
    /// AllowMultiple = false 表示一个方法不能应用此特性多次。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DataAuthPermissionAttribute : Attribute
    {
        public string Permission { get; }

        public DataAuthPermissionAttribute(string permission)
        {
            Permission = permission;
        }
      
    }
}
