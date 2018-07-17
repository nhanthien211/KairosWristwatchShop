using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace ProjectKairos.Utilities
{
    public class MyCustomUtility
    {
        public static string RelativeFromAbsolutePath(string path)
        {
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                var applicationPath = request.PhysicalApplicationPath;
                var virtualDir = request.ApplicationPath;
                virtualDir = virtualDir == "/" ? virtualDir : (virtualDir + "/");
                string test = path.Replace(applicationPath, virtualDir);
                test = test.Replace(@"\", "/");
                return test;
            }
            return "";
        }

        public static bool ValidateImportedExcelFileFormat(HttpPostedFileBase excel)
        {
            using (ExcelPackage package = new ExcelPackage(excel.InputStream))
            {
                if (package.Workbook.Worksheets.Count < 1)
                {
                    // Log - Không có sheet nào tồn tại trong file excel của bạn
                    return false;
                }
                // Khởi Lấy Sheet đầu tiện trong file Excel để truy vấn, truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên
                ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "sheet1") ?? package.Workbook.Worksheets.FirstOrDefault();
                // Đọc tất cả các header
                foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                {

                }

                // Đọc tất cả data bắt đầu từ row thứ 2

            }
            return false;
        }
    }
}