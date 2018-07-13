using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ImageProcessor.Imaging;
using Newtonsoft.Json;
using OfficeOpenXml;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
using WebGrease.Css;

namespace ProjectKairos.Models
{
    public class WatchService
    {
        private KAIROS_SHOPEntities db;

        public WatchService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<WatchTableViewModel> LoadWatchList(string sortColumnDirection, string searchValue, ref int recordsTotal, int skip, int pageSize)
        {
            var watch = db.Watches.Select(w => new WatchTableViewModel
            {
                WatchID = w.WatchID,
                WatchCode = w.WatchCode,
                Quantity = w.Quantity,
                Price = w.Price,
                Status = w.Status
            });
            //sorting
            if (!string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumnDirection)
                {
                    case "asc":
                        watch = watch.OrderBy(w => w.Status);
                        break;
                    case "desc":
                        watch = watch.OrderByDescending(w => w.Status);
                        break;
                }
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                watch = watch.Where(w => w.WatchCode.Contains(searchValue));
            }
            //total number of rows count   
            recordsTotal = watch.Count();
            return watch.Skip(skip).Take(pageSize).ToList();
        }

        public ManageWatchDetailViewModel ViewWatchDetail(int id)
        {
            var watchDetail = db.Watches.Where(w => w.WatchID == id)
                .Select(w => new ManageWatchDetailViewModel
                {
                    WatchId = w.WatchID,
                    WatchCode = w.WatchCode,
                    WatchDescription = w.WatchDescription,
                    Quantity = w.Quantity,
                    Price = w.Price,
                    MovementId = w.MovementID,
                    ModelId = w.ModelID,
                    WaterResistant = w.WaterResistant,
                    BandMaterial = w.BandMaterial,
                    CaseRadius = w.CaseRadius,
                    CaseMaterial = w.CaseMaterial,
                    PublishedTime = w.PublishedTime,
                    PublishedBy = w.PublishedBy,
                    Discount = w.Discount,
                    LedLight = w.LEDLight,
                    Guarantee = w.Guarantee,
                    Alarm = w.Alarm,
                    Thumbnail = w.Thumbnail,
                    Status = w.Status
                }).FirstOrDefault();
            return watchDetail;
        }

        public bool IsDuplicatedWatchCode(string watchCode, int watchId)
        {
            var result = db.Watches.Where(w => w.WatchCode == watchCode && w.WatchID != watchId)
                .Select(w => w.WatchCode)
                .FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool IsDuplicatedWatchCode(string watchCode)
        {
            var result = db.Watches.Where(w => w.WatchCode == watchCode)
                .Select(w => w.WatchCode)
                .FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public String SerializeOldValue(int watchId)
        {
            var archieveValue = db.Watches.Where(w => w.WatchID == watchId)
                .Select(w => new
                {
                    w.WatchCode,
                    w.WatchDescription,
                    w.Quantity,
                    w.Price,
                    w.MovementID,
                    w.ModelID,
                    w.WaterResistant,
                    w.BandMaterial,
                    w.CaseRadius,
                    w.CaseMaterial,
                    w.Discount,
                    w.LEDLight,
                    w.Alarm,
                    w.Thumbnail,
                    w.Status,
                    w.Guarantee
                }).FirstOrDefault();
            return JsonConvert.SerializeObject(archieveValue);
        }

        public bool UpdateWatchInfo(Watch watch, HttpPostedFileBase thumbnail)
        {
            Watch trackedWatch = db.Watches.Find(watch.WatchID);
            db.Watches.Attach(trackedWatch);
            trackedWatch.WatchCode = watch.WatchCode;
            trackedWatch.WatchDescription = watch.WatchDescription;
            trackedWatch.Quantity = watch.Quantity;
            trackedWatch.Price = watch.Price;
            trackedWatch.MovementID = watch.MovementID;
            trackedWatch.ModelID = watch.ModelID;
            trackedWatch.WaterResistant = watch.WaterResistant;
            trackedWatch.BandMaterial = watch.BandMaterial;
            trackedWatch.CaseRadius = watch.CaseRadius;
            trackedWatch.CaseMaterial = watch.CaseMaterial;
            trackedWatch.Discount = watch.Discount;
            trackedWatch.LEDLight = watch.LEDLight;
            trackedWatch.Alarm = watch.Alarm;
            trackedWatch.Status = watch.Status;
            trackedWatch.Guarantee = watch.Guarantee;

            if (thumbnail != null)
            {
                //đổi ảnh thumbnail
                //lưu hình ảnh xuống máy  
                string path = HostingEnvironment.MapPath("~/Content/img/ProductThumbnail/") + watch.WatchCode + DateTime.Now.ToBinary();
                ImageProcessHelper.ResizedImage(thumbnail.InputStream, 360, 500, ResizeMode.Pad, ref path);
                trackedWatch.Thumbnail = path;
            }

            int result = db.SaveChanges();
            if (db.Entry(trackedWatch).State == EntityState.Unchanged || result > 0)
            {
                return true;
            }
            return false;
        }

        public ManageWatchDetailViewModel PrepopulateEditValue(Watch watch, List<Movement> movement, List<WatchModel> watchModel)
        {
            var oldValue = db.Watches.Where(w => w.WatchID == watch.WatchID)
                .Select(w => new { w.WatchID, w.Thumbnail }).FirstOrDefault();

            var viewModel = new ManageWatchDetailViewModel
            {
                Movement = movement,
                WatchModel = watchModel,
                WatchId = oldValue.WatchID, //giá trị cũ do giá trị mới bị trùng
                WatchCode = watch.WatchCode,
                WatchDescription = watch.WatchDescription,
                Quantity = watch.Quantity,
                Price = watch.Price,
                MovementId = watch.MovementID,
                ModelId = watch.ModelID,
                WaterResistant = watch.WaterResistant,
                BandMaterial = watch.BandMaterial,
                CaseRadius = watch.CaseRadius,
                CaseMaterial = watch.CaseMaterial,
                PublishedTime = watch.PublishedTime,
                PublishedBy = watch.PublishedBy,
                Discount = watch.Discount,
                LedLight = watch.LEDLight,
                Guarantee = watch.Guarantee,
                Alarm = watch.Alarm,
                Thumbnail = oldValue.Thumbnail, //load lại thumbnail cũ
                Status = watch.Status,
                DuplicateErrorMessage = "Watch with code '" + watch.WatchCode + "' already existed. Please choose another one"
            };
            return viewModel;
        }

        public bool AddNewWatch(Watch watch)
        {
            db.Watches.Add(watch);
            int result = db.SaveChanges();
            return result > 0;
        }

        public List<WatchInIndexPageModel> LoadWatchListIndex()
        {
            var watch = db.Watches.Where(w => w.Quantity > 0 && w.Status == true)
                .Select(w => new WatchInIndexPageModel
                {
                    WatchID = w.WatchID,
                    WatchCode = w.WatchCode,
                    Thumbnail = w.Thumbnail,
                    Price = w.Price * (1 - w.Discount * 0.01)
                })
                .OrderByDescending(w => w.WatchID)
                .Take(8).ToList();
            return watch;
        }

        public int ImportWatchFromFile(HttpPostedFileBase excel, HttpPostedFileBase zip, string username)
        {

            int totalImported = 0;

            using (ExcelPackage package = new ExcelPackage(excel.InputStream))
            using (ZipArchive archive = new ZipArchive(zip.InputStream, ZipArchiveMode.Read))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                int startRow = workSheet.Dimension.Start.Row;
                int endRow = workSheet.Dimension.End.Row;
                int startCol = workSheet.Dimension.Start.Column;
                int endCol = workSheet.Dimension.End.Column;
                string watchCodeColumn,
                    watchDescriptionColumn,
                    quantityColumn,
                    priceColumn,
                    movementIdColumn,
                    modelIdColumn,
                    waterResistantColumn,
                    bandMaterialColumn,
                    caseRadiusColumn,
                    caseMaterialColumn,
                    discountColumn,
                    ledLightColumn,
                    guaranteeColumn,
                    alarmColumn;
                using (var headers = workSheet.Cells[startRow, startRow, startCol, endCol])
                {
                    watchCodeColumn = headers.First(h => h.Value.Equals("WatchCode")).Address[0].ToString();
                    watchDescriptionColumn = headers.First(h => h.Value.Equals("WatchDescription")).Address[0].ToString();
                    priceColumn = headers.First(h => h.Value.Equals("Price")).Address[0].ToString();
                    quantityColumn = headers.First(h => h.Value.Equals("Quantity")).Address[0].ToString();
                    movementIdColumn = headers.First(h => h.Value.Equals("MovementID")).Address[0].ToString();
                    modelIdColumn = headers.First(h => h.Value.Equals("ModelID")).Address[0].ToString();
                    waterResistantColumn = headers.First(h => h.Value.Equals("WaterResistant")).Address[0].ToString();
                    bandMaterialColumn = headers.First(h => h.Value.Equals("BandMaterial")).Address[0].ToString();
                    caseRadiusColumn = headers.First(h => h.Value.Equals("CaseRadius")).Address[0].ToString();
                    caseMaterialColumn = headers.First(h => h.Value.Equals("CaseMaterial")).Address[0].ToString();
                    discountColumn = headers.First(h => h.Value.Equals("Discount")).Address[0].ToString();
                    ledLightColumn = headers.First(h => h.Value.Equals("LEDLight")).Address[0].ToString();
                    guaranteeColumn = headers.First(h => h.Value.Equals("Guarantee")).Address[0].ToString();
                    alarmColumn = headers.First(h => h.Value.Equals("Alarm")).Address[0].ToString();
                }

                for (int row = startRow + 1; row <= endRow; row++)
                {
                    string watchCode = workSheet.Cells[watchCodeColumn + row].Value.ToString();

                    //Duplicate code found. Skip add. Otherwise will continue to check
                    if (!IsDuplicatedWatchCode(watchCode))
                    {

                        ZipArchiveEntry entry = archive.Entries.FirstOrDefault(e => e.Name.Contains(watchCode));
                        //No Thumbnail match watchcode. Skip add. Otherwise will continue
                        if (entry != null)
                        {
                            try
                            {
                                Watch watch = new Watch
                                {
                                    WatchCode = watchCode,
                                    WatchDescription = workSheet.Cells[watchDescriptionColumn + row].Value.ToString(),
                                    BandMaterial = workSheet.Cells[bandMaterialColumn + row].Value.ToString(),
                                    CaseMaterial = workSheet.Cells[caseMaterialColumn + row].Value.ToString(),

                                    Quantity = Int32.Parse(workSheet.Cells[quantityColumn + row].Value.ToString()),
                                    Price = Double.Parse(workSheet.Cells[priceColumn + row].Value.ToString()),
                                    MovementID = Int32.Parse(workSheet.Cells[movementIdColumn + row].Value.ToString()),
                                    ModelID = Int32.Parse(workSheet.Cells[modelIdColumn + row].Value.ToString()),
                                    CaseRadius = Double.Parse(workSheet.Cells[caseRadiusColumn + row].Value.ToString()),
                                    Discount = Int32.Parse(workSheet.Cells[discountColumn + row].Value.ToString()),
                                    Guarantee = Int32.Parse(workSheet.Cells[guaranteeColumn + row].Value.ToString()),
                                    WaterResistant = workSheet.Cells[waterResistantColumn + row].Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase),
                                    LEDLight = workSheet.Cells[ledLightColumn + row].Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase),
                                    Alarm = workSheet.Cells[alarmColumn + row].Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase),
                                };

                                string path = HostingEnvironment.MapPath("~/Content/img/ProductThumbnail/") + watchCode + DateTime.Now.ToBinary();
                                ImageProcessHelper.ResizedImage(entry.Open(), 360, 500, ResizeMode.Pad, ref path);

                                watch.Thumbnail = path;
                                watch.PublishedTime = DateTime.Now;
                                watch.PublishedBy = username;
                                watch.Status = true;
                                db.Watches.Add(watch);
                                db.SaveChanges();
                                totalImported++;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }

            return totalImported;
        }
    }
}
