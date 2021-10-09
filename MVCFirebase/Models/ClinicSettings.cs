using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    public class ClinicSettings
    {
        [FirestoreProperty]
        public bool bill_sms { get; set; }
        [FirestoreProperty]
        public bool reminder_sms { get; set; }

        [FirestoreProperty]
        public bool inventoryon { get; set; }

        [FirestoreProperty]
        [Range(1, 10000)]
        public int fee1 { get; set; }
        [FirestoreProperty]
        [Range(1, 10000)]
        public int fee2 { get; set; }
        [FirestoreProperty]
        [Range(1, 10000)]
        public int fee3 { get; set; }

        [FirestoreProperty]
        [Range(1, 30)]
        public int days1 { get; set; }
        [FirestoreProperty]
        [Range(1, 30)]
        public int days2 { get; set; }
        [FirestoreProperty]
        [Range(1, 30)]
        public int days3 { get; set; }

        [FirestoreProperty]
        public string whofirst { get; set; }

        [FirestoreProperty]
        public bool consultationfee { get; set; }

        [FirestoreProperty]
        public string id { get; set; }

    }
}