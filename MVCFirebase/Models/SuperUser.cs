using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    [FirestoreData]
    public class SuperUser
    {
        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string UserId { get; set; }
        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string UserName { get; set; }
        [FirestoreProperty]
        [Required(ErrorMessage = "Required.")]
        public string Password { get; set; }
        [FirestoreProperty]
        public bool RememberMe { get; set; }


    }
}