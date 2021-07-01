using System;

namespace Cortside.Bowdlerizer.Test.Models {
    public class Person {
        public Person() {
            BirthDate = new DateTime();
        }

        public int BorrowerID { get; set; }
        public Address MailingAddress { get; set; }
        public string BorrowerFName { get; set; }
        public string EmailAddress { get; set; }
        public string BorrowerLName { get; set; }
        public string SocialSecurityNum { get; set; }
        public string BorrowerSuffix { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
