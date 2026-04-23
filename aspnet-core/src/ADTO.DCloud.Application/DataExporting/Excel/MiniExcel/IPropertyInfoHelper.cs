#nullable enable

using System;
using System.Reflection;
using ADTOSharp.Dependency;


namespace ADTO.DCloud.DataExporting.Excel.MiniExcel;

public interface IPropertyInfoHelper : ITransientDependency
{
    object? GetConvertedPropertyValue(PropertyInfo property, object item,
        Func<PropertyInfo, object, object?>? handleComplexTypes = null);
}