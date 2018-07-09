using System.Collections.Generic;
using System.Linq;

namespace ProjectKairos.Models
{
    public class RoleService
    {
        private KAIROS_SHOPEntities db;

        public RoleService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<Role> GetListRole()
        {
            return db.Roles.ToList();
        }
    }
}