using System.Collections.Generic;
using ProjectKairos.Models;

namespace ProjectKairos.ViewModel
{
    public class AddWatchViewModel
    {


        public string WatchName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string WatchDescription { get; set; }
        public int MovementId { get; set; }
        public int ModelId { get; set; }

        public string WaterEnableLabel { get; set; }
        public string WaterDisableLabel { get; set; }
        public string WaterEnable { get; set; }
        public string WaterDisable { get; set; }
        private bool waterResistant;
        public bool WaterResistant
        {
            get { return waterResistant; }
            set
            {
                waterResistant = value;
                WaterEnable = "";
                WaterEnableLabel = "";
                WaterDisable = "checked";
                WaterDisableLabel = "active";

                if (waterResistant)
                {
                    WaterEnable = "checked";
                    WaterEnableLabel = "active";
                    WaterDisable = "";
                    WaterDisableLabel = "";
                }
            }
        }

        public string AlarmEnableLabel { get; set; }
        public string AlarmDisableLabel { get; set; }
        public string AlarmEnable { get; set; }
        public string AlarmDisable { get; set; }
        private bool ledLight;
        public bool LedLight
        {
            get { return ledLight; }
            set
            {
                ledLight = value;
                LedEnable = "";
                LedEnableLabel = "";
                LedDisable = "checked";
                LedDisableLabel = "active";

                if (ledLight)
                {
                    LedEnable = "checked";
                    LedEnableLabel = "active";
                    LedDisable = "";
                    LedDisableLabel = "";
                }
            }
        }

        public string LedEnableLabel { get; set; }
        public string LedDisableLabel { get; set; }
        public string LedEnable { get; set; }
        public string LedDisable { get; set; }

        private bool alarm;
        public bool Alarm
        {
            get { return alarm; }
            set
            {
                alarm = value;
                AlarmEnable = "";
                AlarmEnableLabel = "";
                AlarmDisable = "checked";
                AlarmDisableLabel = "active";

                if (alarm)
                {
                    AlarmEnable = "checked";
                    AlarmEnableLabel = "active";
                    AlarmDisable = "";
                    AlarmDisableLabel = "";
                }
            }
        }

        public string BandMaterial { get; set; }
        public string CaseMaterial { get; set; }

        private double? caseRadius;
        public double CaseRadius
        {
            get { return caseRadius.GetValueOrDefault(); }
            set { caseRadius = value; }

        }

        public int Discount { get; set; }
        public int Guarantee { get; set; }

        public List<Movement> Movement { get; set; }
        public List<WatchModel> WatchModel { get; set; }

        public string DuplicateErrorMessage { get; set; }
        public string InvalidExcelFileMessage { get; set; }
        public string InvalidZipFileMessage { get; set; }
        public string InvalidImageFileMessage { get; set; }
        public string ImportMessage { get; set; }

        public AddWatchViewModel(List<Movement> movement, List<WatchModel> watchModel)
        {
            Movement = movement;
            WatchModel = watchModel;
            Alarm = true;
            LedLight = true;
            WaterResistant = true;
            CaseRadius = 0;
            Discount = 0;
            Quantity = 1;
            Price = 1;
            Guarantee = 12;
        }

        public AddWatchViewModel()
        {

        }
    }
}