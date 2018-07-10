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


        public string MaleLabel { get; set; }
        public string FemaleLabel { get; set; }
        public string Male { get; set; }
        public string Female { get; set; }
        private bool gender;
        public bool Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                Female = "";
                FemaleLabel = "";
                Male = "checked";
                MaleLabel = "active";

                if (!Gender)
                {
                    Female = "checked";
                    FemaleLabel = "active";
                    Male = "";
                    MaleLabel = "";
                }
            }
        }

    }
}