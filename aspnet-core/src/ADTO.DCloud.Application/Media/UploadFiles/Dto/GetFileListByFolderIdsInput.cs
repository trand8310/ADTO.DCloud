using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Media.UploadFiles.Dto
{
    public class GetFileListByFolderIdsInput
    {
        /// <summary>
        /// FolderId数组
        /// </summary>
        public List<Guid> FolderIds { get; set; }


        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? ProjectId { get; set; }
    }
}
