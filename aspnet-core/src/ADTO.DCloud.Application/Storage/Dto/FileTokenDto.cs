using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.OA.Storage.Dto
{
    /// <summary>
    /// 文件标识
    /// </summary>
    public class FileTokenDto
    {
        /// <summary>
        /// FileToken
        /// </summary>
        public string FileToken { get; set; }

        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// FileSize
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// MimeType
        /// </summary>
        public string MimeType { get; set; }
    }
}
