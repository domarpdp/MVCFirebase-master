using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    public class Inventory
    {

        [FirestoreProperty]
        [MaxLength(25)]
        [Display(Name = "Short Name")]
        [Required(ErrorMessage = "Required.")]
        public string shortname { get; set; }

        [FirestoreProperty]
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Required.")]
        [Range(1, 999)]
        public int quantitypurchased { get; set; }

        [FirestoreProperty]
        [Display(Name = "Quantity Given")]
        [Range(1, 999)]
        public int? quantitygiven { get; set; }

        [FirestoreProperty]
        [Display(Name = "Quantity Balance")]
        [Range(1, 999)]
        public int? quantitybalance { get; set; }

        [FirestoreProperty]
        [Display(Name = "Minimum Quantity")]
        [Required(ErrorMessage = "Required.")]
        [Range(1, 999)]
        public int? quantitymin { get; set; }

        [FirestoreProperty]
        [MaxLength(100)]
        [Display(Name = "Medicine Name")]
        [Required(ErrorMessage = "Required.")]
        public string medicinename { get; set; }


        [FirestoreProperty]
        [Display(Name = "Unit MRP")]
        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.Currency)]
        [Range (0.01,9999.99) ]
        public string unitmrp { get; set; }


        [FirestoreProperty]
        [Display(Name = "Date Added")]
        public string dateadded { get; set; }

        [FirestoreProperty]
        [Display(Name = "Expiry Date")]
        public string expirydate { get; set; }

        [FirestoreProperty]
        [Display(Name = "Purchased Unit Price")]
        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 9999.99)]
        public string purchasedunitprice { get; set; }

        [FirestoreProperty]
        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Required.")]
        [MaxLength(200)]
        public string vendorname { get; set; }

        [FirestoreProperty]
        public string id { get; set; }

        [FirestoreProperty]
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string vendormobilenumber { get; set; }
    }
}