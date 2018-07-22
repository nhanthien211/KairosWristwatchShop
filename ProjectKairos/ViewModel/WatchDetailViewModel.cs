using System.Collections.Generic;

namespace ProjectKairos.ViewModel
{
    public class WatchDetailViewModel
    {
        public int WatchId { get; set; }
        public string WatchCode { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string MovementName { get; set; }
        public string ModelName { get; set; }
        public string BandMaterial { get; set; }
        public double? CaseRadius { get; set; }
        public string CaseMaterial { get; set; }
        public int Guarantee { get; set; }

        public string WaterResistantInfo { get; set; }
        private bool waterResistant;
        public bool WaterResistant
        {
            get => waterResistant;
            set
            {
                waterResistant = value;
                WaterResistantInfo = "No";
                if (value)
                {
                    WaterResistantInfo = "Yes";
                }
            }
        }


        public string LedLightInfo { get; set; }
        private bool ledLight;
        public bool LedLight
        {
            get => ledLight;
            set
            {
                ledLight = value;
                LedLightInfo = "No";
                if (value)
                {
                    LedLightInfo = "Yes";
                }
            }
        }

        public string AlarmInfo { get; set; }
        private bool alarm;

        public bool Alarm
        {
            get => alarm;
            set
            {
                alarm = value;
                AlarmInfo = "No";
                if (value)
                {
                    AlarmInfo = "Yes";
                }
            }
        }
        //list of preview

        public List<ReviewInProductViewModel> ReviewList { get; set; }
    }
}