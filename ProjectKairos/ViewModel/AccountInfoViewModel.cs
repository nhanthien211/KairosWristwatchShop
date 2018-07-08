using System;

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

        public string maleLabel { get; set; }
        public string femaleLabel { get; set; }
        public string male { get; set; }
        public string female { get; set; }
        private bool gender;
        public bool Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                female = "";
                femaleLabel = "";
                male = "checked";
                maleLabel = "active";

                if (!Gender)
                {
                    female = "checked";
                    femaleLabel = "active";
                    male = "";
                    maleLabel = "";
                }
            }
        }
    }
}