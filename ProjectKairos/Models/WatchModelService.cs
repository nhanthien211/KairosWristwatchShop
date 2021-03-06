﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectKairos.Models
{
    public class WatchModelService
    {
        private KAIROS_SHOPEntities db;

        public WatchModelService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<WatchModel> GetModelsList()
        {
            return db.WatchModels.ToList();
        }

        public bool IsModelExisted(string category)
        {
            return db.WatchModels.Any(m => m.ModelName.Equals(category, StringComparison.OrdinalIgnoreCase));
        }
    }
}