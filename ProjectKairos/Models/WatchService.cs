using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ImageProcessor.Imaging;
using Newtonsoft.Json;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;

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

        public WatchDetailViewModel ViewWatchDetail(int id)
        {
            var watchDetail = db.Watches.Where(w => w.WatchID == id)
                .Select(w => new WatchDetailViewModel
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

        public WatchDetailViewModel PrepopulateEditValue(Watch watch, List<Movement> movement, List<WatchModel> watchModel)
        {
            var oldValue = db.Watches.Where(w => w.WatchID == watch.WatchID)
                .Select(w => new { w.WatchID, w.Thumbnail }).FirstOrDefault();

            var viewModel = new WatchDetailViewModel
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
    }
}