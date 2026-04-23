using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace ADTO.DCloud.Web.Host.Startup;

/// <summary>
/// 分组排序
/// </summary>
public class OrderTagsDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="swaggerDoc"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = swaggerDoc.Tags.OrderBy(x => x.Name).ToList();
    }
}
