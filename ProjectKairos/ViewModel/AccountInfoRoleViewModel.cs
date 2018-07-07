using System;
using System.Collections.Generic;
using ProjectKairos.Models;

namespace ProjectKairos.ViewModel
{
    public class AccountInfoRoleViewModel
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime StartDate { get; set; }
        public bool Gender { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public List<Role> Role { get; set; }
    }
}