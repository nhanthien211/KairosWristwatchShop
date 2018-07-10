using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class ShoppingCartService
    {
        private KAIROS_SHOPEntities db;
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public ShoppingCartService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }
    }
}