using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.Dto;
using ADTOSharp;
using System.Collections.Generic;

namespace ADTO.DCloud.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}
