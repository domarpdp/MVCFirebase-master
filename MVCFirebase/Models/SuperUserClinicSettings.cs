using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    public class SuperUserClinicSettings
    {
        [FirestoreProperty]
        public bool smsflag { get; set; }

        [FirestoreProperty]
        public bool syncdataflag { get; set; }

        

    }
}