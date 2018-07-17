using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace ProjectKairos.Utilities
{
    public class ProcessImportFileHelper
    {

        private static bool CheckExcelFileHeader(HttpPostedFileBase file)
        {
            string[] expectedHeader = new[] {
                "WatchCode", "WatchDescription", "Quantity",
                "Price", "MovementID", "ModelID", "WaterResistant",
                "BandMaterial", "CaseRadius", "CaseMaterial", "Discount",
                "LEDLight", "Guarantee", "Alarm"
            };
            // Khởi tạo data table            
            // Load file excel và các setting ban đầu
            try
            {
                using (ExcelPackage package = new ExcelPackage(file.InputStream))
                {
                    if (package.Workbook.Worksheets.Count < 1)
                    {
                        // Log - Không có sheet nào tồn tại trong file excel của bạn
                        return false;
                    }
                    // Khởi Lấy Sheet đầu tiện trong file Excel để truy vấn, truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    if (workSheet.Dimension == null)
                    {
                        return false;
                    }

                    int startRow = workSheet.Dimension.Start.Row;
                    int startCol = workSheet.Dimension.Start.Column;
                    int endCol = workSheet.Dimension.End.Column;
                    // Đọc tất cả các header
                    using (var headers = workSheet.Cells[startRow, startRow, startCol, endCol])
                    {
                        if (!expectedHeader.All(e => headers.Any(h => h.Value.Equals(e))))
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        private static int GetNumberOfRecord(HttpPostedFileBase file)
        {
            using (ExcelPackage package = new ExcelPackage(file.InputStream))
            {
                if (package.Workbook.Worksheets.Count < 1)
                {
                    // Log - Không có sheet nào tồn tại trong file excel của bạn
                    return 0;
                }
                // Khởi Lấy Sheet đầu tiện trong file Excel để truy vấn, truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                // Đọc tất cả các header
                if (workSheet.Dimension == null)
                {
                    return 0;
                }
                int startRow = workSheet.Dimension.Start.Row;
                int endRow = workSheet.Dimension.End.Row;
                return endRow - startRow;
            }
        }

        public static string CheckValidExcelFile(HttpPostedFileBase file)
        {
            if (!FileTypeDetector.IsExcelFile(file))
            {
                return "Input file is not vaild excel file";
            }

            file.InputStream.Position = 0;
            if (!CheckExcelFileHeader(file))
            {
                return "Excel file is missing some column";
            }
            file.InputStream.Position = 0;
            if (GetNumberOfRecord(file) == 0)
            {
                return "Excel does not contains any record";
            }
            file.InputStream.Position = 0;
            return null;
        }

        private static bool CheckImageZipFile(HttpPostedFileBase file)
        {
            //true: leaveOpen after read, so that file stream will not be disposed
            using (ZipArchive archive = new ZipArchive(file.InputStream, ZipArchiveMode.Read, true))
            {
                if (archive.Entries.Count == 0)
                {
                    return false;
                }
                foreach (var entry in archive.Entries)
                {
                    if (!FileTypeDetector.IsImageFile(entry.Open()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string CheckValidZipFile(HttpPostedFileBase file)
        {
            if (!FileTypeDetector.IsZipFile(file))
            {
                return "Input file is not .zip file";
            }
            file.InputStream.Position = 0;
            if (!CheckImageZipFile(file))
            {
                return "Zip file is empty or contains invalid image file";
            }
            file.InputStream.Position = 0;
            return null;
        }
    }
}