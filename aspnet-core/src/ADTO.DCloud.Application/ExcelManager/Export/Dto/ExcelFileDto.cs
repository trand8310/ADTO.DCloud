using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    public class ExcelFileDto
    {
        public ExcelFileDto() { }

        public ExcelFileDto(byte[] fileBytes, string fileName)
        {
            FileBytes = fileBytes;
            FileName = fileName;
        }

        public byte[] FileBytes { get; set; }

        public string FileName { get; set; }
    }
}
