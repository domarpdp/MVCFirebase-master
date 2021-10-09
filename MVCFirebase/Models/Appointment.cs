using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    [FirestoreData]
    public class Appointment
    {
        [FirestoreProperty]
        [Display(Name = "Bill SMS:")]
        [Required(ErrorMessage = "Required.")]
        public bool bill_sms { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string clinic_id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public DateTime complitionDate { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string date { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string days { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string fee { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string patient { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string patient_id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public DateTime raisedDate { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public bool reminder_sms { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string severity { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string status { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public DateTime timeStamp { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string token { get; set; }

        [FirestoreProperty]
        public int tokenIteger { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string patient_name { get; set; }

        [FirestoreProperty]
        public string patient_care_of { get; set; }
        [FirestoreProperty]
        public string patient_gender { get; set; }

        [FirestoreProperty]
        public string patient_age { get; set; }

        [FirestoreProperty]
        public string patient_mobile { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string clinic_name { get; set; }
        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string modeofpayment { get; set; }

        //public ImageViewModel SelectedImage { get; set; }


        //public List<ImageViewModel> GetList()
        //{
        //    List<ImageViewModel> list = new List<ImageViewModel>();
        //    list.Add(new ImageViewModel
        //    {
        //        Id = 1,
        //        Path = "~/Content/Images/IMG_7785.jpg"
        //    });
        //    list.Add(new ImageViewModel
        //    {
        //        Id = 2,
        //        Path = "~/Content/Images/IMG_7788.jpg"
        //    });
        //    list.Add(new ImageViewModel
        //    {
        //        Id = 3,
        //        Path = "~/Content/Images/IMG_7790.jpg"
        //    });
        //    list.Add(new ImageViewModel
        //    {
        //        Id = 4,
        //        Path = "~/Content/Images/IMG_7799.jpg"
        //    });
        //    list.Add(new ImageViewModel
        //    {
        //        Id = 5,
        //        Path = "~/Content/Images/IMG_7847.jpg"
        //    });

        //    return list;
        //}






    }
}