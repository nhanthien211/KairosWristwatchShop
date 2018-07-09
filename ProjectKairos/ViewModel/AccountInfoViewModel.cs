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

<<<<<<< HEAD
                if (!Gender)
                {
                    female = "checked";
                    femaleLabel = "active";
                    male = "";
                    maleLabel = "";
                }
            }
        }
=======
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

>>>>>>> origin/backend_admin_feature_nhan
    }
}