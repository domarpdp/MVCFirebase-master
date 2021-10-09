using Google.Cloud.Firestore;
using Microsoft.Ajax.Utilities;
using MVCFirebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCFirebase.Controllers
{
    
    public class AppointmentController : Controller
    {
        // GET: Appointment

        [CheckSessionTimeOut]
        [AccessDeniedAuthorize(Roles = "Receptionist")]
        public async Task<ActionResult> Index(string startdate)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }

                    SearchDate = SearchDate.Date;
                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    int i = 0;
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                            }
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Waiting").GetSnapshotAsync();

                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                appointment.clinic_name = clinic.clinicname;
                                appointment.patient_name = patient.patient_name;
                                appointment.patient_care_of = patient.care_of;
                                appointment.patient_gender = patient.gender;
                                appointment.patient_age = patient.age;
                                appointment.patient_mobile = patient.patient_mobile_number;
                                appointment.id = docsnapAppointments.Id;
                                try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                catch { appointment.tokenIteger = i + 1; }

                                AppointmentList.Add(appointment);
                            }
                        }

                    }
                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    return View(AppointmentList);


                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }

                    SearchDate = SearchDate.Date;
                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    int i = 0;
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                            }
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Waiting").GetSnapshotAsync();

                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                appointment.clinic_name = clinic.clinicname;
                                appointment.patient_name = patient.patient_name;
                                appointment.patient_care_of = patient.care_of;
                                appointment.patient_gender = patient.gender;
                                appointment.patient_age = patient.age;
                                appointment.patient_mobile = patient.patient_mobile_number;
                                appointment.id = docsnapAppointments.Id;
                                try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                catch { appointment.tokenIteger = i + 1; }
                                AppointmentList.Add(appointment);
                            }
                        }

                    }
                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    return View(AppointmentList);

                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        //Kept as a sample to prevent multiple logins of a user
        public async Task<ActionResult> Index1(string startdate)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    return View();
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    return View();
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }
        
        [AccessDeniedAuthorize(Roles = "Chemist,Cashier")]
        public async Task<ActionResult> Waiting(string startdate)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }
                    SearchDate = SearchDate.Date;

                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    string statusCashier = "";
                    string statusChemist = "";
                    int i = 0;
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                                TempData["inventoryon"] = docSnapSettings.GetValue<bool>("inventoryon");
                            }
                        }
                        else
                        {
                            TempData["inventoryon"] = "false";
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Completed").GetSnapshotAsync();




                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                if (User.IsInRole("Cashier") && User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier == null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                    }

                                }
                                else if (User.IsInRole("Cashier"))
                                {
                                    
                                    if(WhoFirst == "Cashier")
                                    {
                                        try
                                        {
                                            statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                        }
                                        catch
                                        {
                                            statusCashier = null;
                                        }
                                        if (statusCashier == null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                        }
                                        catch
                                        {
                                            statusChemist = null;
                                        }
                                        if (statusChemist != null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    
                                }
                                else if (User.IsInRole("Chemist"))
                                {
                                    if (WhoFirst == "Cashier")
                                    {
                                        try
                                        {
                                            statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                        }
                                        catch
                                        {
                                            statusCashier = null;
                                        }
                                        if (statusCashier != null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                        }
                                        catch
                                        {
                                            statusChemist = null;
                                        }
                                        if (statusChemist == null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    
                                }
                            }
                        }

                    }
                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    if (SearchDate.Date < DateTime.Now.Date)
                    {
                        ViewData["DateType"] = "OldDate";
                    }
                    else
                    {
                        ViewData["DateType"] = "CurrentDate";
                    }
                    return View(AppointmentList);
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }
                    SearchDate = SearchDate.Date;
                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    string statusCashier = "";
                    string statusChemist = "";
                    int i = 0;
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                                TempData["inventoryon"] = docSnapSettings.GetValue<bool>("inventoryon");
                            }
                        }
                        else
                        {
                            TempData["inventoryon"] = "false";
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Completed").GetSnapshotAsync();




                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                if (User.IsInRole("Cashier") && User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier == null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                    }

                                }
                                else if (User.IsInRole("Cashier"))
                                {
                                    if (WhoFirst == "Cashier")
                                    {
                                        try
                                        {
                                            statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                        }
                                        catch
                                        {
                                            statusCashier = null;
                                        }
                                        if (statusCashier == null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                        }
                                        catch
                                        {
                                            statusChemist = null;
                                        }
                                        if (statusChemist != null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                }
                                else if (User.IsInRole("Chemist"))
                                {
                                    if (WhoFirst == "Cashier")
                                    {
                                        try
                                        {
                                            statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                        }
                                        catch
                                        {
                                            statusCashier = null;
                                        }
                                        if (statusCashier != null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                        }
                                        catch
                                        {
                                            statusChemist = null;
                                        }
                                        if (statusChemist == null)
                                        {
                                            appointment.clinic_name = clinic.clinicname;
                                            appointment.patient_name = patient.patient_name;
                                            appointment.patient_care_of = patient.care_of;
                                            appointment.patient_gender = patient.gender;
                                            appointment.patient_age = patient.age;
                                            appointment.patient_mobile = patient.patient_mobile_number;
                                            appointment.id = docsnapAppointments.Id;
                                            appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token"));
                                            AppointmentList.Add(appointment);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    if (SearchDate.Date < DateTime.Now.Date)
                    {
                        ViewData["DateType"] = "OldDate";
                    }
                    else
                    {
                        ViewData["DateType"] = "CurrentDate";
                    }
                    return View(AppointmentList);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        
        [AccessDeniedAuthorize(Roles = "Chemist,Cashier")]
        public async Task<ActionResult> Completed(string startdate)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            int totalfee = 0;
            int totalfeecash = 0;
            int totalfeeothers = 0;
            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }
                    SearchDate = SearchDate.Date;
                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    string statusCashier = "";
                    string statusChemist;
                    int i = 0;
                    
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                            }
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Completed").GetSnapshotAsync();




                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                if (User.IsInRole("Cashier") && User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                        totalfee = totalfee + Convert.ToInt32(appointment.fee);
                                        if (appointment.modeofpayment == "Cash")
                                        {
                                            totalfeecash = totalfeecash + Convert.ToInt32(appointment.fee);
                                        }
                                        else
                                        {
                                            totalfeeothers = totalfeeothers + Convert.ToInt32(appointment.fee);
                                        }
                                    }
                                }
                                else if (User.IsInRole("Cashier"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                        totalfee = totalfee + Convert.ToInt32(appointment.fee);
                                        if(appointment.modeofpayment == "Cash")
                                        {
                                            totalfeecash = totalfeecash + Convert.ToInt32(appointment.fee);
                                        }
                                        else
                                        {
                                            totalfeeothers = totalfeeothers + Convert.ToInt32(appointment.fee);
                                        }
                                    }
                                }
                                else if (User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                    }
                                    catch
                                    {
                                        statusChemist = null;
                                    }
                                    if (statusChemist != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                    }
                                }

                            }
                        }

                    }

                    ViewData["totalfee"] = totalfee;
                    ViewData["totalfeecash"] = totalfeecash;
                    ViewData["totalfeeothers"] = totalfeeothers;

                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    return View(AppointmentList);
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    DateTime SearchDate;
                    if (startdate == null)
                    {
                        SearchDate = Convert.ToDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        SearchDate = Convert.ToDateTime(startdate);
                        SearchDate = DateTime.SpecifyKind(SearchDate, DateTimeKind.Utc);
                    }
                    SearchDate = SearchDate.Date;
                    Timestamp SearchDateFrom = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30));
                    Timestamp SearchDateTo = Timestamp.FromDateTime(SearchDate.Date.AddHours(-5).AddMinutes(-30).AddDays(1));

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Appointment> AppointmentList = new List<Appointment>();
                    List<string> statusList = new List<string>();


                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();
                    QuerySnapshot snapAppointments;
                    string WhoFirst = "Cashier";
                    string statusCashier = "";
                    string statusChemist;
                    int i = 0;
                    foreach (DocumentSnapshot docsnapClinics in snapClinics)
                    {
                        Clinic clinic = docsnapClinics.ConvertTo<Clinic>();

                        QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").Limit(1).GetSnapshotAsync();
                        if (snapSettings.Count > 0)
                        {
                            DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                            if (docSnapSettings.Exists)
                            {
                                WhoFirst = docSnapSettings.GetValue<string>("whofirst");
                            }
                        }

                        snapAppointments = await docsnapClinics.Reference.Collection("appointments").WhereGreaterThanOrEqualTo("raisedDate", SearchDateFrom).WhereLessThan("raisedDate", SearchDateTo).WhereEqualTo("status", "Completed").GetSnapshotAsync();




                        foreach (DocumentSnapshot docsnapAppointments in snapAppointments)
                        {


                            Appointment appointment = docsnapAppointments.ConvertTo<Appointment>();

                            QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                            DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                            Patient patient = docsnapPatient.ConvertTo<Patient>();
                            if (docsnapAppointments.Exists)
                            {
                                if (User.IsInRole("Cashier") && User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                        totalfee = totalfee + Convert.ToInt32(appointment.fee);
                                        if (appointment.modeofpayment == "Cash")
                                        {
                                            totalfeecash = totalfeecash + Convert.ToInt32(appointment.fee);
                                        }
                                        else
                                        {
                                            totalfeeothers = totalfeeothers + Convert.ToInt32(appointment.fee);
                                        }
                                    }
                                }
                                else if (User.IsInRole("Cashier"))
                                {
                                    try
                                    {
                                        statusCashier = docsnapAppointments.GetValue<string>("statusCashier");
                                    }
                                    catch
                                    {
                                        statusCashier = null;
                                    }
                                    if (statusCashier != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                        totalfee = totalfee + Convert.ToInt32(appointment.fee);
                                        if (appointment.modeofpayment == "Cash")
                                        {
                                            totalfeecash = totalfeecash + Convert.ToInt32(appointment.fee);
                                        }
                                        else
                                        {
                                            totalfeeothers = totalfeeothers + Convert.ToInt32(appointment.fee);
                                        }
                                    }
                                }
                                else if (User.IsInRole("Chemist"))
                                {
                                    try
                                    {
                                        statusChemist = docsnapAppointments.GetValue<string>("statusChemist");
                                    }
                                    catch
                                    {
                                        statusChemist = null;
                                    }
                                    if (statusChemist != null)
                                    {
                                        appointment.clinic_name = clinic.clinicname;
                                        appointment.patient_name = patient.patient_name;
                                        appointment.patient_care_of = patient.care_of;
                                        appointment.patient_gender = patient.gender;
                                        appointment.patient_age = patient.age;
                                        appointment.patient_mobile = patient.patient_mobile_number;
                                        appointment.id = docsnapAppointments.Id;
                                        try { appointment.tokenIteger = Convert.ToInt32(docsnapAppointments.GetValue<string>("token")); }
                                        catch { appointment.tokenIteger = i + 1; }
                                        AppointmentList.Add(appointment);
                                    }
                                }

                            }
                        }

                    }
                    ViewData["totalfee"] = totalfee;
                    ViewData["totalfeecash"] = totalfeecash;
                    ViewData["totalfeeothers"] = totalfeeothers;
                    AppointmentList = AppointmentList.OrderByDescending(a => a.tokenIteger).ToList();
                    ViewBag.Message = SearchDate.Date;
                    return View(AppointmentList);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }
        

        // GET: Appointment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Appointment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Appointment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Appointment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        //public async Task<string> Prescription(string patientAutoId)
        //{

        //    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
        //    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
        //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
        //    FirestoreDb db = FirestoreDb.Create("greenpaperdev");

        //    string prescriptionString = "";


        //    Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId).Collection("prescriptions").OrderByDescending("timeStamp").Limit(1);
        //    QuerySnapshot snap = await QrefPrescriptions.GetSnapshotAsync();

        //    foreach (DocumentSnapshot docsnap in snap)
        //    {
        //        if (docsnap.Exists)
        //        {

        //            prescriptionString = docsnap.GetValue<string>("file");
        //        }
        //    }

        //    return prescriptionString;
        //}

        //public async Task<ActionResult> PrescriptionList(string patientAutoId)
        //{
        //    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
        //    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
        //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
        //    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


        //    List<string> prescriptionStringList = new List<string>();

        //    Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId).Collection("prescriptions").OrderByDescending("timeStamp");
        //    QuerySnapshot snap = await QrefPrescriptions.GetSnapshotAsync();

        //    foreach (DocumentSnapshot docsnap in snap)
        //    {
        //        if (docsnap.Exists)
        //        {

        //            prescriptionStringList.Add(docsnap.GetValue<string>("file"));
        //        }
        //    }



        //    return View(prescriptionStringList);

        //}

        public async Task<ActionResult> Fee(string id, string patient, string fee)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    TempData["appointmentAutoId"] = id;
                    TempData["patientAutoId"] = patient;
                    TempData["fee"] = fee;
                    List<SelectListItem> paymentmode = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Cash", Value = "Cash"
                },
                new SelectListItem {
                    Text = "Paytm", Value = "Paytm"
                },
                new SelectListItem {
                    Text = "Credit Card", Value = "Credit Card"
                },
                new SelectListItem {
                    Text = "Debit Card", Value = "Debit Card"
                },
            };
                    ViewBag.PAYMENTMODES = paymentmode;
                    return View();
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    TempData["appointmentAutoId"] = id;
                    TempData["patientAutoId"] = patient;
                    TempData["fee"] = fee;
                    List<SelectListItem> paymentmode = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Cash", Value = "Cash"
                },
                new SelectListItem {
                    Text = "Paytm", Value = "Paytm"
                },
                new SelectListItem {
                    Text = "Credit Card", Value = "Credit Card"
                },
                new SelectListItem {
                    Text = "Debit Card", Value = "Debit Card"
                },
            };
                    ViewBag.PAYMENTMODES = paymentmode;
                    return View();
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Fee1(string id,string patient,string fee)
        {
            TempData["appointmentAutoId"] = id;
            TempData["patientAutoId"] = patient;
            TempData["fee"] = fee;
            List<SelectListItem> paymentmode = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Cash", Value = "Cash"
                },
                new SelectListItem {
                    Text = "Paytm", Value = "Paytm"
                },
                new SelectListItem {
                    Text = "Credit Card", Value = "Credit Card"
                },
                new SelectListItem {
                    Text = "Debit Card", Value = "Debit Card"
                },
            };
            ViewBag.PAYMENTMODES = paymentmode;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Fee()
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {
                        string patientAutoId = TempData["patientAutoId"].ToString();
                        string appointmentAutoId = TempData["appointmentAutoId"].ToString();

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"}

                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        return View();
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {
                        string patientAutoId = TempData["patientAutoId"].ToString();
                        string appointmentAutoId = TempData["appointmentAutoId"].ToString();

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"}

                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        return View();
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Appointment/Create
        [HttpPost]
        public async Task<ActionResult> Fee1()
        {
            try
            {
                string patientAutoId = TempData["patientAutoId"].ToString();
                string appointmentAutoId = TempData["appointmentAutoId"].ToString();

                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                if (docSnap.Exists)
                {
                    Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"}
                        
                        };

                    
                    await docRef.UpdateAsync(data1);

                }
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> SubmitCashier(FormCollection collection)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoIdFee"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"modeofpayment",collection["modeofpaymentFee"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoIdFee"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"modeofpayment",collection["modeofpaymentFee"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public async Task<ActionResult> SubmitChemist(FormCollection collection)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {

                        string appointmentAutoId = collection["appid"]; ;

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string appointmentAutoId = collection["appid"]; ;

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public async Task<ActionResult> SubmitChemistInventoryOn(FormCollection collection)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoIdMedicine"]; 

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpaymentChemist",collection["modeofpaymentMedicine"]}

                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return RedirectToAction("Waiting");
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoId"]; ;

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpaymentChemist",collection["modeofpaymentMedicine"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return RedirectToAction("Waiting");
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CashierChemistUpdate(FormCollection collection)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoId"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpayment",collection["modeofpayment"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoId"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpayment",collection["modeofpayment"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CashierChemistUpdateInvOn(FormCollection collection)
        {

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    try
                    {

                        string appointmentAutoId = collection["appId"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpayment",collection["modeofpayment"]},
                            {"modeofpaymentChemist",collection["modeofpayment"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string appointmentAutoId = collection["appointmentAutoId"];

                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appointmentAutoId);
                        DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                        if (docSnap.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"completiondateCashier" ,DateTime.UtcNow},
                            {"statusCashier" ,"Completed"},
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"},
                            {"modeofpayment",collection["modeofpayment"]},
                            {"modeofpaymentChemist",collection["modeofpayment"]}
                        };


                            await docRef.UpdateAsync(data1);

                        }
                        // TODO: Add delete logic here

                        return RedirectToAction("Waiting");
                    }
                    catch
                    {
                        return View();
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        public async static Task<bool> IsYourLoginStillTrue(string userId, string sid)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            Query QrefPatientLastId = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("logins").WhereEqualTo("userid", userId).WhereEqualTo("sessionid", sid).WhereEqualTo("loggedin", true).Limit(1);
            QuerySnapshot snapPatientLastId = await QrefPatientLastId.GetSnapshotAsync();
            if (snapPatientLastId.Count > 0)
            { 
                return true;
            }

            return false;
        }

        public async static Task<bool> IsUserLoggedOnElsewhere(string userId, string sid)
        {
            int i = 0;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            Query QrefUserLoggedInElseWhere = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("logins").WhereEqualTo("userid", userId).WhereEqualTo("loggedin", true);
            QuerySnapshot snapUserLoggedInElseWhere = await QrefUserLoggedInElseWhere.GetSnapshotAsync();
            if (snapUserLoggedInElseWhere.Count > 0)
            {

                foreach (DocumentSnapshot docsnapLoggedInUsers in snapUserLoggedInElseWhere)
                {
                    if(docsnapLoggedInUsers.GetValue<string>("sessionid") != sid)
                    {
                        i = i + 1;
                    }
                }
            }
            if (i > 0)
            { return true; }
            else
            { return false; }
            
        }

        public async static void LogEveryoneElseOut(string userId, string sid)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            Query QrefPatientLastId = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("logins").WhereEqualTo("userid", userId).WhereEqualTo("loggedin", true);
            QuerySnapshot snapPatientLastId = await QrefPatientLastId.GetSnapshotAsync();
            if (snapPatientLastId.Count > 0)
            {
                foreach (DocumentSnapshot docsnapLoggedInUsers in snapPatientLastId)
                {
                    if (docsnapLoggedInUsers.GetValue<string>("sessionid") != sid)
                    {
                        DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("logins").Document(docsnapLoggedInUsers.Id);
                        DocumentSnapshot docSnapupdate = await docRef.GetSnapshotAsync();

                        if (docSnapupdate.Exists)
                        {
                            Dictionary<string, object> data1 = new Dictionary<string, object>
                            {
                                {"loggedin" ,false}
                            };

                            await docRef.UpdateAsync(data1);
                        }
                    }
                }
            }
        }
    }
}
