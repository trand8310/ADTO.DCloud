using System;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace ADTOSharp.AspNetCore.Dependency;

public static class ADTOSharpMvcBuilderExtensions
{
    public static IMvcBuilder AddADTOSharpNewtonsoftJson(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddNewtonsoftJson().Services.AddOptions<MvcNewtonsoftJsonOptions>()
            .Configure<IServiceProvider>((options, rootServiceProvider) =>
            {
                var aspNetCoreConfiguration = rootServiceProvider.GetRequiredService<IADTOSharpAspNetCoreConfiguration>();

                if (aspNetCoreConfiguration.UseMvcDateTimeFormatForAppServices)
                {
                    var mvcNewtonsoftJsonOptions = rootServiceProvider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value;
                    aspNetCoreConfiguration.InputDateTimeFormats.Add(mvcNewtonsoftJsonOptions.SerializerSettings.DateFormatString);
                }

                options.SerializerSettings.ContractResolver = new ADTOSharpMvcContractResolver(aspNetCoreConfiguration.InputDateTimeFormats, aspNetCoreConfiguration.OutputDateTimeFormat)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

        return mvcBuilder;
    }
}
