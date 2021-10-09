using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    [FirestoreData]
    public class Clinic
    {

        [FirestoreProperty]
        public string clinicname { get; set; }
        [FirestoreProperty]
        public string cliniccity { get; set; }
        [FirestoreProperty]
        public string clinicmobilenumber { get; set; }

        [FirestoreProperty]
        public string selected_plan { get; set; }

        [FirestoreProperty]
        public string id { get; set; }
    }
}