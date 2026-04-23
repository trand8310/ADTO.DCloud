using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users;
using AutoMapper;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Organizations;
using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.Chat;

namespace ADTO.DCloud.Infrastructure;
/// <summary>
/// DTO自定义映射
/// 有些DTO不适合使用 AutoMap,AutoMapTo,AutoMapFrom,需要字段级映射,或不同名映射则需要使用这个方法来处理 
/// </summary>
internal static class CustomDtoMapper
{
    public static void CreateMappings(IMapperConfigurationExpression configuration)
    {
        //ReverseMap, .ForMember(dto => dto.TotalAmount, options => options.MapFrom(e => e.GetTotalAmount()));
        //.ForMember(user => user.Password, options => options.Ignore());

        //User
        /*
        configuration.CreateMap<User, UserEditDto>()
            .ForMember(dto => dto.Password, options => options.Ignore())
            .ReverseMap()
            .ForMember(user => user.Password, options => options.Ignore());
        */

        configuration.CreateMap<User, UserEditDto>()
        .ForMember(dto => dto.Password, options => options.Ignore())
        .ReverseMap()
        .ForMember(user => user.Password, options => options.Ignore());


        configuration.CreateMap<OrganizationUnit, DepartmentDto>();


        //Chat
        configuration.CreateMap<ChatMessage, ChatMessageDto>();
        configuration.CreateMap<ChatMessage, ChatMessageExportDto>();
    }
}