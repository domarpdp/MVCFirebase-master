using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCFirebase.Models
{
    public class GlobalSessionVariables
    {

        public static string ClinicMobileNumber
        {
            get { return HttpContext.Current.Session["ClinicMobileNumber"].ToString(); }
            set { HttpContext.Current.Session["ClinicMobileNumber"] = value; }
        }
        public static string ClinicDocumentAutoId
        {
            get { return HttpContext.Current.Session["ClinicDocumentAutoId"].ToString(); }
            set { HttpContext.Current.Session["ClinicDocumentAutoId"] = value; }
        }

        public static string UserRoles
        {
            get { return HttpContext.Current.Session["UserRoles"].ToString(); }
            set { HttpContext.Current.Session["UserRoles"] = value; }
        }

    }
}