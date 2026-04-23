using ADTO.DCloud.Dto.Excel;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace ADTO.DCloud.Infrastructur
{
    public class ExcelHelper
    {
        #region 创建ExcelPackage
        /// <summary>
        /// 创建ExcelPackage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">数据实体</param>
        /// <param name="columnNames">列名{"Count","数量"}</param>
        /// <param name="title">标题</param>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="totalColumsValues">合计行，数据展示：{"Count",100}</param>
        /// <param name="isProtected">是否加密</param>
        /// <returns></returns>
        private static ExcelPackage CreateExcelPackage<T>(List<T> datas, Dictionary<string, string> columnNames, string title = "", string sheetName = "Sheet1", Dictionary<string, object> totalColumsValues = null, bool isProtected = true)
        {
             

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            if (isProtected)
            {
                worksheet.Protection.IsProtected = false;//设置是否进行锁定
                #region 工作表保护中的其它选项控制
                //worksheet.Protection.SetPassword("****");//设置密码
                //worksheet.Protection.AllowAutoFilter = false;//下面是一些锁定时权限的设置
                //worksheet.Protection.AllowDeleteColumns = false;
                //worksheet.Protection.AllowDeleteRows = false;
                //worksheet.Protection.AllowEditScenarios = false;
                //worksheet.Protection.AllowEditObject = false;
                //worksheet.Protection.AllowFormatCells = false;
                //worksheet.Protection.AllowFormatColumns = false;
                //worksheet.Protection.AllowFormatRows = false;
                //worksheet.Protection.AllowInsertColumns = false;
                //worksheet.Protection.AllowInsertHyperlinks = false;
                //worksheet.Protection.AllowInsertRows = false;
                //worksheet.Protection.AllowPivotTables = false;
                //worksheet.Protection.AllowSelectLockedCells = false;
                //worksheet.Protection.AllowSelectUnlockedCells = false;
                //worksheet.Protection.AllowSort = false;
                #endregion
            }
            worksheet.Cells.Style.WrapText = true;//自动换行
            var titleRow = 0;
            if (!string.IsNullOrWhiteSpace(title))
            {
                titleRow = 1;
                worksheet.Cells[1, 1].Value = title;
                worksheet.Cells.Style.ShrinkToFit = true;//单元格自动适应大小
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                worksheet.Cells[1, 1].Style.Font.Size = 20;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Row(1).Height = 30;//设置行高
                if (columnNames != null && columnNames.Count > 0)
                {
                    worksheet.Cells[1, 1, 1, columnNames.Count].Merge = true;//合并单元格
                }
                // worksheet.Cells.Style.WrapText = true;//自动换行
                // worksheet.Row(1).CustomHeight = true;//自动调整行高
            }

            //获取要反射的属性,加载首行
            Type myType = typeof(T);
            List<PropertyInfo> myPro = new List<PropertyInfo>();
            int i = 1;
            foreach (string key in columnNames.Keys)
            {
                PropertyInfo p = myType.GetProperty(key);
                myPro.Add(p);
                worksheet.Cells[1 + titleRow, i].Value = columnNames[key];
                worksheet.Column(i).Width = 15;
                i++;
            }
            int row = 2 + titleRow;
            if (datas != null && datas.Count > 0)
            {
                foreach (T data in datas)
                {
                    int column = 1;
                    foreach (PropertyInfo p in myPro)
                    {
                        if (p == null)
                            worksheet.Cells[row, column].Value = "";
                        else
                        {
                            if (p.PropertyType == typeof(System.Decimal))
                            {
                                System.Decimal.TryParse(p.GetValue(data, null).ToString(), out decimal dvalue);
                                worksheet.Cells[row, column].Value = dvalue;
                            }
                            else
                            {
                                worksheet.Cells[row, column].Value = Convert.ToString(p.GetValue(data, null));
                            }
                            if (p.PropertyType == typeof(System.Decimal) || p.PropertyType == typeof(Int32) || p.PropertyType == typeof(Int64) || p.PropertyType == typeof(double))
                            {
                                worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;//默认居右
                            }

                        }
                        column++;
                    }
                    row++;
                }
                datas.Clear();
            }
            #region 合计行处理
            if (totalColumsValues != null && totalColumsValues.Count > 0)
            {
                int col = 1;
                foreach (PropertyInfo p in myPro)
                {
                    if (p == null)
                        worksheet.Cells[row, col].Value = "";
                    else
                    {
                        if (totalColumsValues.ContainsKey(p.Name))
                        {
                            if (p.PropertyType == typeof(System.Decimal))
                            {
                                System.Decimal.TryParse(totalColumsValues[p.Name].ToString(), out decimal dvalue);
                                worksheet.Cells[row, col].Value = decimal.Round(dvalue, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value = Convert.ToString(totalColumsValues[p.Name]);
                            }
                            if (p.PropertyType == typeof(System.Decimal) || p.PropertyType == typeof(Int32) || p.PropertyType == typeof(Int64) || p.PropertyType == typeof(double))
                            {
                                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;//默认居右
                            }
                        }
                        else
                            worksheet.Cells[row, col].Value = "";
                    }
                    col++;
                }
                totalColumsValues.Clear();
            }
            #endregion
            columnNames.Clear();
            return package;
        }
        #endregion

        #region 通用导出EXCEL
        /// <summary>
        ///通用导出EXCEL，将ExcelPackage转换成Byte类型，以流的方式进行导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">数据实体</param>
        /// <param name="columnNames">列名：{"Count","数量"}</param>
        /// <param name="title">标题</param>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="totalColumsValues">合计行，数据展示：{"Count",100}</param>
        /// <param name="isProtected">是否加密</param>
        /// <returns></returns>
        public static Byte[] GetByteToExportExcel<T>(List<T> datas, Dictionary<string, string> columnNames, string title = "", string sheetName = "Sheet1", Dictionary<string, object> totalColumsValues = null, bool isProtected = true)
        {
            if (columnNames == null || columnNames.Count <= 0)
            {
                return new Byte[] { };
            }
            using (var fs = new MemoryStream())
            {
                using (var package = CreateExcelPackage(datas, columnNames, title, sheetName, totalColumsValues, isProtected))
                {
                    package.SaveAs(fs);
                    return fs.ToArray();
                }
            }
        }
        #endregion

        #region Excel导入
        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="filePath">Excel地址</param>
        /// <returns></returns>
        public static DataTable GetExcelDataTable(string filePath)
        {
            IWorkbook Workbook;
            DataTable table = new DataTable();
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                    string fileExt = Path.GetExtension(filePath).ToLower();
                    if (fileExt == ".xls")
                    {
                        Workbook = new HSSFWorkbook(fileStream);
                    }
                    else if (fileExt == ".xlsx")
                    {
                        Workbook = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        Workbook = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //定位在第一个sheet
            ISheet sheet = Workbook.GetSheetAt(0);
            //第一行为标题行
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            int rowCount = sheet.LastRowNum;

            //循环添加标题列
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            //数据
            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                bool result = false;
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();
                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = GetCellValue(row.GetCell(j));
                            //全为空则不取
                            if (dataRow[j].ToString() != "")
                            {
                                result = true;
                            }
                        }
                    }
                    if (result == true)
                    {
                        table.Rows.Add(dataRow);
                    }
                }
            }

            return table;
        }

        #endregion

        #region DataTable导出到Excel的MemoryStream
        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static Byte[] ExportDataTableMemoryStream(DataTable dtSource, ExcelConfig excelConfig)
        {
            for (int i = 0; i < dtSource.Columns.Count;)
            {
                DataColumn column = dtSource.Columns[i];
                var isColum = false;
                if (excelConfig.ColumnEntity != null)
                {
                    foreach (var item in excelConfig.ColumnEntity)
                    {
                        if (item.Column.Equals(column.ToString()))
                        {
                            isColum = true;
                        }
                    }
                }else
                    isColum=true;   
                if (!isColum)
                {
                    dtSource.Columns.Remove(column.ToString());
                }
                else
                {
                    i++;
                }
            }
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "adto"; //填加xls文件作者信息
                si.ApplicationName = "adto"; //填加xls文件创建程序信息
                si.LastAuthor = "adto"; //填加xls文件最后保存者信息
                si.Comments = "adto"; //填加xls文件作者信息
                si.Title = "adto"; //填加xls文件标题信息
                si.Subject = "adto";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            #region 设置标题样式
            ICellStyle headStyle = workbook.CreateCellStyle();
            int[] arrColWidth = new int[dtSource.Columns.Count];
            string[] arrColName = new string[dtSource.Columns.Count];//列名
            ICellStyle[] arryColumStyle = new ICellStyle[dtSource.Columns.Count];//样式表
            headStyle.Alignment = HorizontalAlignment.Center; // ------------------
            if (excelConfig.Background != new Color())
            {
                if (excelConfig.Background != new Color())
                {
                    headStyle.FillPattern = FillPattern.SolidForeground;
                    headStyle.FillForegroundColor = GetXLColour(workbook, excelConfig.Background);
                }
            }
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = excelConfig.TitlePoint;
            if (excelConfig.ForeColor != new Color())
            {
                font.Color = GetXLColour(workbook, excelConfig.ForeColor);
            }
            //font.Boldweight = 700;
            headStyle.SetFont(font);
            #endregion

            #region 列头及样式
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center; // ------------------
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;
            cHeadStyle.SetFont(cfont);
            #endregion

            #region 设置内容单元格样式
            foreach (DataColumn item in dtSource.Columns)
            {
                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                arrColName[item.Ordinal] = item.ColumnName.ToString();
                if (excelConfig.ColumnEntity != null)
                {
                    ColumnModel columnentity = excelConfig.ColumnEntity.Find(t => t.Column == item.ColumnName);
                    if (columnentity != null)
                    {
                        arrColName[item.Ordinal] = columnentity.ExcelColumn;
                        if (columnentity.Width != 0)
                        {
                            arrColWidth[item.Ordinal] = columnentity.Width;
                        }
                        if (columnentity.Background != new Color())
                        {
                            if (columnentity.Background != new Color())
                            {
                                columnStyle.FillPattern = FillPattern.SolidForeground;
                                columnStyle.FillForegroundColor = GetXLColour(workbook, columnentity.Background);
                            }
                        }
                        if (columnentity.Font != null || columnentity.Point != 0 || columnentity.ForeColor != new Color())
                        {
                            IFont columnFont = workbook.CreateFont();
                            columnFont.FontHeightInPoints = 10;
                            if (columnentity.Font != null)
                            {
                                columnFont.FontName = columnentity.Font;
                            }
                            if (columnentity.Point != 0)
                            {
                                columnFont.FontHeightInPoints = columnentity.Point;
                            }
                            if (columnentity.ForeColor != new Color())
                            {
                                columnFont.Color = GetXLColour(workbook, columnentity.ForeColor);
                            }
                            columnStyle.SetFont(font);
                        }
                        columnStyle.Alignment = getAlignment(columnentity.Alignment);
                    }
                }
                arryColumStyle[item.Ordinal] = columnStyle;
            }
            if (excelConfig.IsAllSizeColumn)
            {
                #region 根据列中最长列的长度取得列宽
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        if (arrColWidth[j] != 0)
                        {
                            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }

                    }
                }
                #endregion
            }
            #endregion

            int rowIndex = 0;

            #region 表头及样式
            if (excelConfig.Title != null)
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                if (excelConfig.TitleHeight != 0)
                {
                    headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                }
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
            }
            #endregion

            #region 列头及样式
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                foreach (DataColumn column in dtSource.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(arrColName[column.Ordinal]);
                    headerRow.GetCell(column.Ordinal).CellStyle = cHeadStyle;
                    //设置列宽
                    //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                    int colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                    if (colWidth < 255 * 256)
                    {
                        sheet.SetColumnWidth(column.Ordinal, colWidth < 3000 ? 3000 : colWidth);
                    }
                    else
                    {
                        sheet.SetColumnWidth(column.Ordinal, 6000);
                    }
                }
                #endregion
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535)
                {
                    sheet = workbook.CreateSheet();
                    rowIndex = 0;
                    #region 表头及样式
                    {
                        if (excelConfig.Title != null)
                        {
                            IRow headerRow = sheet.CreateRow(rowIndex);
                            rowIndex++;
                            if (excelConfig.TitleHeight != 0)
                            {
                                headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                            }
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
                        }

                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(rowIndex);
                        rowIndex++;
                        #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(arrColName[column.Ordinal]);
                            headerRow.GetCell(column.Ordinal).CellStyle = cHeadStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    NPOI.SS.UserModel.ICell newCell = dataRow.CreateCell(column.Ordinal);
                    newCell.CellStyle = arryColumStyle[column.Ordinal];
                    string drValue = row[column].ToString();
                    SetCell(newCell, dateStyle, column.DataType, drValue);
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                //return ms;
                return ms.ToArray();
            }
        }
        #endregion


        #region DataTable导出到Excel的MemoryStream
        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static MemoryStream ExportMemoryStream(DataTable dtSource, ExcelConfig excelConfig)
        {
            int colint = 0;
            for (int i = 0; i < dtSource.Columns.Count;)
            {
                DataColumn column = dtSource.Columns[i];
                if (excelConfig.ColumnEntity[colint].Column != column.ColumnName)
                {
                    dtSource.Columns.Remove(column.ColumnName);
                }
                else
                {
                    i++;
                    if (colint < excelConfig.ColumnEntity.Count - 1)
                    {
                        colint++;
                    }
                }
            }
            //for (int i = 0; i < dtSource.Columns.Count;)
            //{
            //    DataColumn column = dtSource.Columns[i];
            //    var res = true;
            //    foreach (var item in excelConfig.ColumnEntity)
            //    {
            //        if (item.Column == column.ColumnName)
            //        {
            //            res = false;
            //        }
            //    }
            //    if (res)
            //    {
            //        dtSource.Columns.Remove(column);
            //    }
            //}

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "adto"; //填加xls文件作者信息
                si.ApplicationName = "adto"; //填加xls文件创建程序信息
                si.LastAuthor = "adto"; //填加xls文件最后保存者信息
                si.Comments = "adto"; //填加xls文件作者信息
                si.Title = "adto"; //填加xls文件标题信息
                si.Subject = "adto";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            #region 设置标题样式
            ICellStyle headStyle = workbook.CreateCellStyle();
            int[] arrColWidth = new int[dtSource.Columns.Count];
            string[] arrColName = new string[dtSource.Columns.Count];//列名
            ICellStyle[] arryColumStyle = new ICellStyle[dtSource.Columns.Count];//样式表
            headStyle.Alignment = HorizontalAlignment.Center; // ------------------
            if (excelConfig.Background != new Color())
            {
                if (excelConfig.Background != new Color())
                {
                    headStyle.FillPattern = FillPattern.SolidForeground;
                    headStyle.FillForegroundColor = GetXLColour(workbook, excelConfig.Background);
                }
            }
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = excelConfig.TitlePoint;
            if (excelConfig.ForeColor != new Color())
            {
                font.Color = GetXLColour(workbook, excelConfig.ForeColor);
            }
            ///font.Boldweight = 700;
            headStyle.SetFont(font);
            #endregion

            #region 列头及样式
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center; // ------------------
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;
            cHeadStyle.SetFont(cfont);
            #endregion

            #region 设置内容单元格样式
            foreach (DataColumn item in dtSource.Columns)
            {
                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                arrColName[item.Ordinal] = item.ColumnName.ToString();
                if (excelConfig.ColumnEntity != null)
                {
                    ColumnModel columnentity = excelConfig.ColumnEntity.Find(t => t.Column == item.ColumnName);
                    if (columnentity != null)
                    {
                        arrColName[item.Ordinal] = columnentity.ExcelColumn;
                        if (columnentity.Width != 0)
                        {
                            arrColWidth[item.Ordinal] = columnentity.Width;
                        }
                        if (columnentity.Background != new Color())
                        {
                            if (columnentity.Background != new Color())
                            {
                                columnStyle.FillPattern = FillPattern.SolidForeground;
                                columnStyle.FillForegroundColor = GetXLColour(workbook, columnentity.Background);
                            }
                        }
                        if (columnentity.Font != null || columnentity.Point != 0 || columnentity.ForeColor != new Color())
                        {
                            IFont columnFont = workbook.CreateFont();
                            columnFont.FontHeightInPoints = 10;
                            if (columnentity.Font != null)
                            {
                                columnFont.FontName = columnentity.Font;
                            }
                            if (columnentity.Point != 0)
                            {
                                columnFont.FontHeightInPoints = columnentity.Point;
                            }
                            if (columnentity.ForeColor != new Color())
                            {
                                columnFont.Color = GetXLColour(workbook, columnentity.ForeColor);
                            }
                            columnStyle.SetFont(font);
                        }
                        columnStyle.Alignment = getAlignment(columnentity.Alignment);
                    }
                }
                arryColumStyle[item.Ordinal] = columnStyle;
            }
            if (excelConfig.IsAllSizeColumn)
            {
                #region 根据列中最长列的长度取得列宽
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        if (arrColWidth[j] != 0)
                        {
                            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }

                    }
                }
                #endregion
            }
            #endregion

            int rowIndex = 0;

            #region 表头及样式
            if (excelConfig.Title != null)
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                if (excelConfig.TitleHeight != 0)
                {
                    headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                }
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
            }
            #endregion

            #region 列头及样式
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                foreach (DataColumn column in dtSource.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(arrColName[column.Ordinal]);
                    headerRow.GetCell(column.Ordinal).CellStyle = cHeadStyle;
                    //设置列宽
                    //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                    int colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                    if (colWidth < 255 * 256)
                    {
                        sheet.SetColumnWidth(column.Ordinal, colWidth < 3000 ? 3000 : colWidth);
                    }
                    else
                    {
                        sheet.SetColumnWidth(column.Ordinal, 6000);
                    }
                }
                #endregion
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535)
                {
                    sheet = workbook.CreateSheet();
                    rowIndex = 0;
                    #region 表头及样式
                    {
                        if (excelConfig.Title != null)
                        {
                            IRow headerRow = sheet.CreateRow(rowIndex);
                            rowIndex++;
                            if (excelConfig.TitleHeight != 0)
                            {
                                headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                            }
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
                        }

                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(rowIndex);
                        rowIndex++;
                        #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(arrColName[column.Ordinal]);
                            headerRow.GetCell(column.Ordinal).CellStyle = cHeadStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    NPOI.SS.UserModel.ICell newCell = dataRow.CreateCell(column.Ordinal);
                    newCell.CellStyle = arryColumStyle[column.Ordinal];
                    string drValue = row[column].ToString();
                    SetCell(newCell, dateStyle, column.DataType, drValue);
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        #endregion

        #region 设置表格内容
        private static void SetCell(NPOI.SS.UserModel.ICell newCell, ICellStyle dateStyle, Type dataType, string drValue)
        {
            switch (dataType.ToString())
            {
                case "System.TimeSpan":
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }

                    newCell.CellStyle = dateStyle;//格式化显示
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }
        #endregion

        #region 设置列的对齐方式
        /// <summary>
        /// 设置对齐方式
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private static HorizontalAlignment getAlignment(string style)
        {
            switch (style)
            {
                case "center":
                    return HorizontalAlignment.Center;
                case "left":
                    return HorizontalAlignment.Left;
                case "right":
                    return HorizontalAlignment.Right;
                case "fill":
                    return HorizontalAlignment.Fill;
                case "justify":
                    return HorizontalAlignment.Justify;
                case "centerselection":
                    return HorizontalAlignment.CenterSelection;
                case "distributed":
                    return HorizontalAlignment.Distributed;
            }
            return NPOI.SS.UserModel.HorizontalAlignment.General;
        }
        #endregion

        #region RGB颜色转NPOI颜色
        private static short GetXLColour(HSSFWorkbook workbook, Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }

            }
            else
                s = XlColour.Indexed;
            return s;
        }
        #endregion

        #region 获取单元格数据
        /// <summary>
        /// 获取单元格数据
        /// </summary>
        /// <param name="cell">excel单元格</param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType._None:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        #endregion

    }
}
