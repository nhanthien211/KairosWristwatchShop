using ProjectKairos.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Service")]
    public class ServiceController : Controller
    {
        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();

        [Route("GetAllCities")]
        public JsonResult GetAllCities()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var dataCity = db.Cities
                .Select(c => new
                {
                    c.CityID,
                    c.CityName,
                    c.Type
                }).OrderBy(c => c.CityName)
                .ToList();
            return Json(dataCity, JsonRequestBehavior.AllowGet);
        }

        [Route("GetDistrictByCity/{receive}")]
        public JsonResult GetDistrictByCity(string receive)
        {
            db.Configuration.ProxyCreationEnabled = false;

            int cityID = Convert.ToInt32(receive);

            var dataDistrict = db.Districts
                .Select(d => new
                {
                    d.DistrictID,
                    d.DistrictName,
                    d.CityID,
                    d.Type
                })
                .Where(d => d.CityID == cityID)
                .OrderBy(d => d.DistrictName)
                .ToList();
            return Json(dataDistrict, JsonRequestBehavior.AllowGet);
        }

        [Route("GetWardByDistrict/{receive}")]
        public JsonResult GetWardByDistrict(string receive)
        {
            db.Configuration.ProxyCreationEnabled = false;

            int districtID = Convert.ToInt32(receive);

            var dataWard = db.Wards
                .Select(w => new
                {
                    w.WardID,
                    w.WardName,
                    w.DistrictID,
                    w.Type
                })
                .Where(w => w.DistrictID == districtID)
                .OrderBy(w => w.WardName)
                .ToList();
            return Json(dataWard, JsonRequestBehavior.AllowGet);
        }

    }
}