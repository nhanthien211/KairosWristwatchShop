﻿using System;
using System.Collections.Generic;
using ProjectKairos.Models;

namespace ProjectKairos.ViewModel
{
    public class ManageWatchDetailViewModel
    {
        public int WatchId { get; set; }
        public string WatchCode { get; set; }
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
            get => waterResistant;
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
            get => ledLight;
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
            get => alarm;
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

        private double caseRadius;
        public double? CaseRadius
        {
            get => caseRadius;
            set => caseRadius = value.GetValueOrDefault();

        }

        public int Discount { get; set; }
        public int Guarantee { get; set; }

        public List<Movement> Movement { get; set; }
        public List<WatchModel> WatchModel { get; set; }

        public string DuplicateErrorMessage { get; set; }

        public string PublishedBy { get; set; }

        public DateTime PublishedTime { get; set; }


        public string StatusEnableLabel { get; set; }
        public string StatusDisableLabel { get; set; }
        public string StatusEnable { get; set; }
        public string StatusDisable { get; set; }

        private bool status;
        public bool Status
        {
            get => status;
            set
            {
                status = value;
                StatusEnable = "";
                StatusEnableLabel = "";
                StatusDisable = "checked";
                StatusDisableLabel = "active";

                if (status)
                {
                    StatusEnable = "checked";
                    StatusEnableLabel = "active";
                    StatusDisable = "";
                    StatusDisableLabel = "";
                }
            }
        }

        public string Thumbnail { get; set; }
        public string InvalidImageFileMessage { get; set; }

    }
}