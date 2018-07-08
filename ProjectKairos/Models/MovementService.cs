using System.Collections.Generic;
using System.Linq;

namespace ProjectKairos.Models
{
    public class MovementService
    {
        private KAIROS_SHOPEntities db;

        public MovementService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<Movement> GetMovementList()
        {
            return db.Movements.ToList();
        }
    }
}