using System;

namespace ProjectKairos.Models
{
    public class ModificationService
    {
        private KAIROS_SHOPEntities db;

        public ModificationService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public bool CreateNewModificationHistory(int watchId, string oldValue, string userId)
        {
            Modification modification = new Modification
            {
                WatchID = watchId,
                ModifiedBy = userId,
                ModifiedTime = DateTime.Now,
                OldValue = oldValue
            };
            db.Modifications.Add(modification);
            int result = db.SaveChanges();
            return result > 0;
        }
    }
}