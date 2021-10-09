using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    [FirestoreData]
    public class Patient
    {
        [FirestoreProperty]
        public string id { get; set; }

        [FirestoreProperty]
        [Display(Name = "Patient Name")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(50)]
        public string patient_name { get; set; }

        [FirestoreProperty]
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string patient_mobile_number { get; set; }

        [FirestoreProperty]
        [Display(Name = "Clinic Name")]
        [MaxLength(200)]
        public string clinic_name { get; set; }

        [FirestoreProperty]
        [Display(Name = "Patient UID")]
        public string patient_id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "City")]
        [MaxLength(24)]
        public string city { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Age")]
        [Range(1, 99)]
        public string age { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Care Of")]
        [MaxLength(50)]
        public string care_of { get; set; }

        [FirestoreProperty]
        public DateTime creation_date { get; set; }

        [FirestoreProperty]
        [Display(Name = "Appointment Date")]
        public DateTime appointment_date { get; set; }

        [FirestoreProperty]
        [Display(Name = "Token Number")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [MaxLength(3)]
        public string tokenNumber { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Disease")]
        [MaxLength(50)]
        public string disease { get; set; }

        [FirestoreProperty]
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(6)]
        public string gender { get; set; }

        [FirestoreProperty]
        [Display(Name = "Categarization")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(6)]
        public string severity { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Refered By")]
        [MaxLength(50)]
        public string refer_by { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Refer To")]
        [MaxLength(50)]
        public string refer_to_doctor { get; set; }

        [FirestoreProperty]
        [MaxLength(200)]
        public string search_text { get; set; }

        [FirestoreProperty]
        [Display(Name = "Want to create Appointment?")]
        public string createAppointment { get; set; }


    }
}