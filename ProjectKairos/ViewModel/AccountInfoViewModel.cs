using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.ViewModel
{
    public class AccountInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime StartedDate { get; set; }
        public bool Gender { get; set; }


    }
}