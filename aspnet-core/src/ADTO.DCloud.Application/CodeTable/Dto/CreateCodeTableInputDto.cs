using ADTO.DCloud.DataBase.Model;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.CodeTable.Dto
{
    /// <summary>
    /// 添加数据库表信息
    /// </summary>
    public class CreateCodeTableInputDto
    {
        public List<CreateCodeTableDto> TableList { get; set; }
    }

    public class CreateCodeTableDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
