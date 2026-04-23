using System.Collections.Generic;
using System.Linq;
using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.DataExporting.Excel.MiniExcel;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing.Timezone;


namespace ADTO.DCloud.Chat.Exporting
{
    public class ChatMessageListExcelExporter : MiniExcelExcelExporterBase, IChatMessageListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IADTOSharpSession _adtosharpSession;

        public ChatMessageListExcelExporter(
            ITempFileCacheManager tempFileCacheManager,
            ITimeZoneConverter timeZoneConverter,
            IADTOSharpSession adtosharpSession
            ) : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _adtosharpSession = adtosharpSession;
        }

        public FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages)
        {
            var tenancyName = messages.Count > 0 ? messages.First().TargetTenantName : L("Anonymous");
            var userName = messages.Count > 0 ? messages.First().TargetUserName : L("Anonymous");

            var items = new List<Dictionary<string, object>>();

            foreach (var message in messages)
            {
                items.Add(new Dictionary<string, object>()
                {
                    {L("ChatMessage_From"), message.Side == ChatSide.Receiver ? (message.TargetTenantName + "/" + message.TargetUserName) : L("You")},
                    {L("ChatMessage_To"), message.Side == ChatSide.Receiver ? L("You") : (message.TargetTenantName + "/" + message.TargetUserName)},
                    {L("Message"), message.Message},
                    {L("ReadState"), message.Side == ChatSide.Receiver ? message.ReadState : message.ReceiverReadState},
                    {L("CreationTime"), _timeZoneConverter.Convert(message.CreationTime, user.TenantId, user.UserId)},
                });
            }

            return CreateExcelPackage($"Chat_{tenancyName}_{userName}.xlsx", items);
        }
    }
}
