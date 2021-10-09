using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        [Display(Name = "Clinic Mobile Number:")]
        public string clinicmobilenumber { get; set; }

        [FirestoreProperty]
        [Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string mobile_number { get; set; }

        [FirestoreProperty]
        [Display(Name = "User Name:")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(50)]
        public string name { get; set; }


        [FirestoreProperty]
        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(15)]
        public string password { get; set; }


        [FirestoreProperty]
        public bool RememberMe { get; set; }

        [FirestoreProperty]
        [Display(Name = "Qualification:")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(200)]
        public string user_qualification { get; set; }

        [FirestoreProperty]
        public DateTime creation_date { get; set; }

        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string idproof { get; set; }
        [FirestoreProperty]
        public string signature { get; set; }
        [FirestoreProperty]
        public bool? status_enable { get; set; }

        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        [Display(Name = "Roles:")]
        public string[] user_roles { get; set; }
    }
}