using System;

namespace ProjectKairos.ViewModel
{
    public class AccountRegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime? Dob { get; set; }
        public string DuplicateUsernameErrorMessage { get; set; }
        public string DuplicateEmailErrorMessage { get; set; }
        public string InvalidLogin { get; set; }

        public string GenderEnable { get; set; }
        public string GenderDisable { get; set; }
        public string GenderEnableLabel { get; set; }
        public string GenderDisableLabel { get; set; }

        private bool gender;
        public bool Gender
        {
            get => gender;
            set
            {
                gender = value;
                GenderEnable = "";
                GenderEnableLabel = "";
                GenderDisable = "checked";
                GenderDisableLabel = "active";

                if (gender)
                {
                    GenderEnable = "checked";
                    GenderEnableLabel = "active";
                    GenderDisable = "";
                    GenderDisableLabel = "";
                }
            }
        }

        public AccountRegisterViewModel()
        {
            Username = "";
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            DuplicateEmailErrorMessage = "";
            DuplicateUsernameErrorMessage = "";
            InvalidLogin = "";
            Gender = true;
        }
    }
}