using System;
using System.Collections.Generic;
using ADTOSharp;
using ADTOSharp.Application.Services;
using ADTO.DCloud.DemoUiComponents.Dto;

namespace ADTO.DCloud.DemoUiComponents
{
    public interface IDemoUiComponentsAppService: IApplicationService
    {
        DateFieldOutput SendAndGetDate(DateTime date);

        DateFieldOutput SendAndGetDateTime(DateTime date);

        DateRangeFieldOutput SendAndGetDateRange(DateTime startDate, DateTime endDate);

        DateWithTextFieldOutput SendAndGetDateWithText(SendAndGetDateWithTextInput input);

        List<NameValue<string>> GetCountries(string searchTerm);

        List<NameValue<string>> SendAndGetSelectedCountries(List<NameValue<string>> selectedCountries);

        StringOutput SendAndGetValue(string input);
    }
}