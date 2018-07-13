namespace ProjectKairos.ViewModel
{
    public class WatchDetailViewModel
    {
        public int WatchId { get; set; }
        public string WatchCode { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string WatchMovement { get; set; }
        public string WatchModel { get; set; }
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

        public string BandMaterial { get; set; }
        public double CaseRadius { get; set; }
        public string CaseMaterial { get; set; }
        public int Discount { get; set; }

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

        public int Guarantee { get; set; }
        public string Thumbnail { get; set; }


    }
}