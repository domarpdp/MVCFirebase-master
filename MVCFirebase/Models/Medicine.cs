using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    public class Medicine
    {
        [Display(Name = "Auto Id")]
        public int serialno { get; set; }

        [Display(Name = "Auto Id")]
        public string id { get; set; }

        [Display(Name = "Medicine Name")]
        public string medicinename { get; set; }

        [Display(Name = "Quantity")]
        public string quantity { get; set; }

        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "Inventory Id")]
        public string inventoryid { get; set; }


    }
}