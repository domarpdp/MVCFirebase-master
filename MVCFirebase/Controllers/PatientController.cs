using Google.Cloud.Firestore;
using MVCFirebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using Newtonsoft.Json;

namespace MVCFirebase.Controllers
{
    
    
    [AccessDeniedAuthorize(Roles = "Receptionist")]
    public class PatientController : Controller
    {
        // GET: Patient
        
        public async Task<ActionResult> Index1(string search)
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            List<Patient> PatientList = new List<Patient>();
            
            //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
            Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
            QuerySnapshot snap = await Qref.GetSnapshotAsync();
            
           foreach (DocumentSnapshot docsnap in snap)
            {
                Clinic clinic = docsnap.ConvertTo<Clinic>();



                if (search != null && search != "")
                {
                    string searchInputToLower = search.ToLower();
                    string searchInputToUpper = search.ToUpper();
                    QuerySnapshot snapPatientId = await docsnap.Reference.Collection("patientList").OrderBy("patient_id").StartAt(searchInputToUpper).EndAt(searchInputToUpper + "\uf8ff").GetSnapshotAsync();

                    if(snapPatientId.Count == 0)
                    {
                        QuerySnapshot snapPatientMobileNumber = await docsnap.Reference.Collection("patientList").OrderBy("patient_mobile_number").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                        if (snapPatientMobileNumber.Count == 0)
                        {
                            QuerySnapshot snapPatientName = await docsnap.Reference.Collection("patientList").OrderBy("patient_name").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                            if (snapPatientName.Count > 0)
                            {
                                foreach (DocumentSnapshot docsnap2 in snapPatientName)
                                {
                                    Patient patient = docsnap2.ConvertTo<Patient>();
                                    patient.id = docsnap2.Id;

                                    if (docsnap2.Exists)
                                    {
                                        patient.clinic_name = clinic.clinicname;
                                        PatientList.Add(patient);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DocumentSnapshot docsnap2 in snapPatientMobileNumber)
                            {
                                Patient patient = docsnap2.ConvertTo<Patient>();
                                patient.id = docsnap2.Id;

                                if (docsnap2.Exists)
                                {
                                    patient.clinic_name = clinic.clinicname;
                                    PatientList.Add(patient);
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (DocumentSnapshot docsnap2 in snapPatientId)
                        {
                            Patient patient = docsnap2.ConvertTo<Patient>();
                            patient.id = docsnap2.Id;

                            if (docsnap2.Exists)
                            {
                                patient.clinic_name = clinic.clinicname;
                                PatientList.Add(patient);
                            }
                        }
                    }
                }
                else
                {

                    QuerySnapshot snap2 = await docsnap.Reference.Collection("patientList").OrderByDescending("patient_id").GetSnapshotAsync();
                    if (snap2.Count > 0)
                    {
                        foreach (DocumentSnapshot docsnap2 in snap2)
                        {
                            Patient patient = docsnap2.ConvertTo<Patient>();
                            patient.id = docsnap2.Id;

                            if (docsnap2.Exists)
                            {
                                patient.clinic_name = clinic.clinicname;
                                PatientList.Add(patient);
                            }
                        }
                    }
                }

                
                    
                    
            }

            

            return View(PatientList);
            
        }

        public async Task<ActionResult> Index(string search)
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
                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");




                    List<Patient> PatientList = new List<Patient>();
                    List<SelectListItem> UserList = new List<SelectListItem>();

                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snap = await Qref.GetSnapshotAsync();




                    foreach (DocumentSnapshot docsnap in snap)
                    {

                        Clinic clinic = docsnap.ConvertTo<Clinic>();

                        #region Code to get doctors list for refer to
                        QuerySnapshot snapUsersDoctors = await docsnap.Reference.Collection("user").WhereArrayContainsAny("user_roles",new string[]{ "Doctor", "Chemist" }).GetSnapshotAsync();
                        foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                        {
                            SelectListItem user = new SelectListItem();
                            user.Text = docsnapUsers.GetValue<string>("name");
                            user.Value = docsnapUsers.Id;
                            if (docsnapUsers.Exists)
                            {
                                UserList.Add(user);
                            }
                        }
                        ViewBag.USERS = UserList;
                        #endregion Code to Code to get doctors list for refer to

                        if (search != null && search != "")
                        {
                            string searchInputToLower = search.ToLower();
                            string searchInputToUpper = search.ToUpper();
                            QuerySnapshot snapPatientId = await docsnap.Reference.Collection("patientList").OrderBy("patient_id").StartAt(searchInputToUpper).EndAt(searchInputToUpper + "\uf8ff").GetSnapshotAsync();

                            if (snapPatientId.Count == 0)
                            {
                                QuerySnapshot snapPatientMobileNumber = await docsnap.Reference.Collection("patientList").OrderBy("patient_mobile_number").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                                if (snapPatientMobileNumber.Count == 0)
                                {
                                    QuerySnapshot snapPatientName = await docsnap.Reference.Collection("patientList").OrderBy("patient_name").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                                    if (snapPatientName.Count > 0)
                                    {
                                        foreach (DocumentSnapshot docsnap2 in snapPatientName)
                                        {
                                            Patient patient = docsnap2.ConvertTo<Patient>();
                                            patient.id = docsnap2.Id;

                                            if (docsnap2.Exists)
                                            {
                                                patient.clinic_name = clinic.clinicname;
                                                PatientList.Add(patient);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (DocumentSnapshot docsnap2 in snapPatientMobileNumber)
                                    {
                                        Patient patient = docsnap2.ConvertTo<Patient>();
                                        patient.id = docsnap2.Id;

                                        if (docsnap2.Exists)
                                        {
                                            patient.clinic_name = clinic.clinicname;
                                            PatientList.Add(patient);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                foreach (DocumentSnapshot docsnap2 in snapPatientId)
                                {
                                    Patient patient = docsnap2.ConvertTo<Patient>();
                                    patient.id = docsnap2.Id;

                                    if (docsnap2.Exists)
                                    {
                                        patient.clinic_name = clinic.clinicname;
                                        PatientList.Add(patient);
                                    }
                                }
                            }
                        }
                        else
                        {

                            QuerySnapshot snap2 = await docsnap.Reference.Collection("patientList").OrderByDescending("patient_id").Limit(5).GetSnapshotAsync();
                            if (snap2.Count > 0)
                            {
                                foreach (DocumentSnapshot docsnap2 in snap2)
                                {
                                    Patient patient = docsnap2.ConvertTo<Patient>();
                                    patient.id = docsnap2.Id;

                                    if (docsnap2.Exists)
                                    {
                                        patient.clinic_name = clinic.clinicname;
                                        PatientList.Add(patient);
                                    }
                                }
                            }
                        }




                    }



                    return View(PatientList);
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    List<Patient> PatientList = new List<Patient>();
                    List<SelectListItem> UserList = new List<SelectListItem>();

                    //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
                    Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                    QuerySnapshot snap = await Qref.GetSnapshotAsync();

                    foreach (DocumentSnapshot docsnap in snap)
                    {
                        Clinic clinic = docsnap.ConvertTo<Clinic>();

                        #region Code to get doctors list for refer to
                        QuerySnapshot snapUsersDoctors = await docsnap.Reference.Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
                        foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                        {
                            SelectListItem user = new SelectListItem();
                            user.Text = docsnapUsers.GetValue<string>("name");
                            user.Value = docsnapUsers.Id;
                            if (docsnapUsers.Exists)
                            {
                                UserList.Add(user);
                            }
                        }
                        ViewBag.USERS = UserList;
                        #endregion Code to Code to get doctors list for refer to


                        if (search != null && search != "")
                        {
                            string searchInputToLower = search.ToLower();
                            string searchInputToUpper = search.ToUpper();
                            QuerySnapshot snapPatientId = await docsnap.Reference.Collection("patientList").OrderBy("patient_id").StartAt(searchInputToUpper).EndAt(searchInputToUpper + "\uf8ff").GetSnapshotAsync();

                            if (snapPatientId.Count == 0)
                            {
                                QuerySnapshot snapPatientMobileNumber = await docsnap.Reference.Collection("patientList").OrderBy("patient_mobile_number").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                                if (snapPatientMobileNumber.Count == 0)
                                {
                                    QuerySnapshot snapPatientName = await docsnap.Reference.Collection("patientList").OrderBy("patient_name").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                                    if (snapPatientName.Count > 0)
                                    {
                                        foreach (DocumentSnapshot docsnap2 in snapPatientName)
                                        {
                                            Patient patient = docsnap2.ConvertTo<Patient>();
                                            patient.id = docsnap2.Id;

                                            if (docsnap2.Exists)
                                            {
                                                patient.clinic_name = clinic.clinicname;
                                                PatientList.Add(patient);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (DocumentSnapshot docsnap2 in snapPatientMobileNumber)
                                    {
                                        Patient patient = docsnap2.ConvertTo<Patient>();
                                        patient.id = docsnap2.Id;

                                        if (docsnap2.Exists)
                                        {
                                            patient.clinic_name = clinic.clinicname;
                                            PatientList.Add(patient);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                foreach (DocumentSnapshot docsnap2 in snapPatientId)
                                {
                                    Patient patient = docsnap2.ConvertTo<Patient>();
                                    patient.id = docsnap2.Id;

                                    if (docsnap2.Exists)
                                    {
                                        patient.clinic_name = clinic.clinicname;
                                        PatientList.Add(patient);
                                    }
                                }
                            }
                        }
                        else
                        {

                            QuerySnapshot snap2 = await docsnap.Reference.Collection("patientList").OrderByDescending("patient_id").Limit(5).GetSnapshotAsync();
                            if (snap2.Count > 0)
                            {
                                foreach (DocumentSnapshot docsnap2 in snap2)
                                {
                                    Patient patient = docsnap2.ConvertTo<Patient>();
                                    patient.id = docsnap2.Id;

                                    if (docsnap2.Exists)
                                    {
                                        patient.clinic_name = clinic.clinicname;
                                        PatientList.Add(patient);
                                    }
                                }
                            }
                        }




                    }



                    return View(PatientList);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Patient/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            List<SelectListItem> diseases = new List<SelectListItem>() {
                        new SelectListItem {
                            Text = "General -(fever/general acute complaints)", Value = "General"
                        },
                        new SelectListItem {
                            Text = "ENT (ear nose throat)", Value = "ENT(ear nose throat)"
                        },
                        new SelectListItem {
                            Text = "CNS (central nervous system", Value = "CNS(central nervous system)"
                        },
                        new SelectListItem {
                            Text = "DERMATO (skin/hair)", Value = "DERMATO./SKIN/hair"
                        },
                        new SelectListItem {
                            Text = "GIT (gastro intestinal track)", Value = "GIT(gastro intestinal track)"
                        },
                        new SelectListItem {
                            Text = "SURGERY", Value = "SURGERY"
                        },
                        new SelectListItem {
                            Text = "TUMORS/ONCOLOGY", Value = "TUMOURS/ONCOLOGY"
                        },
                        new SelectListItem {
                            Text = "HAEMATOLOGY (blood)", Value = "HAEMATOLOGY(blood)"
                        },
                        new SelectListItem {
                            Text = "ORTHOPADICS (bones & muscles)", Value = "ORTHOPADICS(bones & muscles)"
                        },
                        new SelectListItem {
                            Text = "PAEDIATRIC (child)", Value = "paediatric"
                        },
                        new SelectListItem {
                            Text = "OPTHALMOLOGISTS (EYE)", Value = "opthalmologists(eye)"
                        },
                        new SelectListItem {
                            Text = "CVS (cardio vascular system/heart)", Value = "CVS(cardio vascular system/heart)"
                        },
                        new SelectListItem {
                            Text = "ENDOCRINOLOGY", Value = "ENDOCRINOLOGY"
                        },
                        new SelectListItem {
                            Text = "GYNAECOLOGY & OBS", Value = "GYNAECOLOGY & OBS"
                        },
                        new SelectListItem {
                            Text = "GENETICS", Value = "GENETICS"
                        },
                    };
            ViewBag.DISEASES = diseases;

            List<SelectListItem> referby = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "PATIENT", Value = "PATIENT"
                },
                new SelectListItem {
                    Text = "TV ADV", Value = "TV ADV"
                },
                new SelectListItem {
                    Text = "SOCIAL MEDIA", Value = "SOCIAL MEDIA"
                },
                new SelectListItem {
                    Text = "WALL PAINTING", Value = "WALL PAINTING"
                },
                new SelectListItem {
                    Text = "POSTERS", Value = "POSTERS"
                },
                new SelectListItem {
                    Text = "INTERNET SEARCH", Value = "INTERNET SEARCH"
                },
                new SelectListItem {
                    Text = "CALLS", Value = "CALLS"
                },
                new SelectListItem {
                    Text = "DOCTOR", Value = "DOCTOR"
                },
                new SelectListItem {
                    Text = "HOSPITAL", Value = "HOSPITAL"
                },
                new SelectListItem {
                    Text = "MAIL/MSG", Value = "MAILMSG"
                },
                new SelectListItem {
                    Text = "RANDOM", Value = "RANDOM"
                },

            };
            ViewBag.REFERBYS = referby;

            List<SelectListItem> UserList = new List<SelectListItem>();
            

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }



            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    

                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");
                    DocumentReference docRefClinicCity = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId);
                    DocumentSnapshot docSnapClinicCity = await docRefClinicCity.GetSnapshotAsync();
                    Clinic clinic = docSnapClinicCity.ConvertTo<Clinic>();
                    Patient patient = new Patient();
                    patient.city = clinic.cliniccity;
                    //patient.appointment_date = DateTime.Now;
                    //patient.tokenNumber = await getLatestToken(DateTime.Now.ToString("MM/dd/yyyy"));

                    QuerySnapshot snapUsersDoctors = await docSnapClinicCity.Reference.Collection("user").WhereArrayContains("user_roles","Doctor").GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                    {
                        SelectListItem user = new SelectListItem();
                        user.Text = docsnapUsers.GetValue<string>("name");
                        user.Value = docsnapUsers.Id;
                        if (docsnapUsers.Exists)
                        {
                            UserList.Add(user); 
                        }
                    }
                    ViewBag.USERS = UserList;
                    return View(patient);
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());

                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");
                    DocumentReference docRefClinicCity = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId);
                    DocumentSnapshot docSnapClinicCity = await docRefClinicCity.GetSnapshotAsync();
                    Clinic clinic = docSnapClinicCity.ConvertTo<Clinic>();
                    Patient patient = new Patient();
                    patient.city = clinic.cliniccity;
                    //patient.appointment_date = DateTime.Now;
                    //patient.tokenNumber = await getLatestToken(DateTime.Now.ToString("MM/dd/yyyy"));

                    QuerySnapshot snapUsersDoctors = await docSnapClinicCity.Reference.Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                    {
                        SelectListItem user = new SelectListItem();
                        user.Text = docsnapUsers.GetValue<string>("name");
                        user.Value = docsnapUsers.Id;
                        if (docsnapUsers.Exists)
                        {
                            UserList.Add(user);
                        }
                    }
                    ViewBag.USERS = UserList;

                    return View(patient);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(Patient patient)
        {
            List<SelectListItem> diseases = new List<SelectListItem>() {
                        new SelectListItem {
                            Text = "General -(fever/general acute complaints)", Value = "General"
                        },
                        new SelectListItem {
                            Text = "ENT (ear nose throat)", Value = "ENT(ear nose throat)"
                        },
                        new SelectListItem {
                            Text = "CNS (central nervous system", Value = "CNS(central nervous system)"
                        },
                        new SelectListItem {
                            Text = "DERMATO (skin/hair)", Value = "DERMATO./SKIN/hair"
                        },
                        new SelectListItem {
                            Text = "GIT (gastro intestinal track)", Value = "GIT(gastro intestinal track)"
                        },
                        new SelectListItem {
                            Text = "SURGERY", Value = "SURGERY"
                        },
                        new SelectListItem {
                            Text = "TUMORS/ONCOLOGY", Value = "TUMOURS/ONCOLOGY"
                        },
                        new SelectListItem {
                            Text = "HAEMATOLOGY (blood)", Value = "HAEMATOLOGY(blood)"
                        },
                        new SelectListItem {
                            Text = "ORTHOPADICS (bones & muscles)", Value = "ORTHOPADICS(bones & muscles)"
                        },
                        new SelectListItem {
                            Text = "PAEDIATRIC (child)", Value = "paediatric"
                        },
                        new SelectListItem {
                            Text = "OPTHALMOLOGISTS (EYE)", Value = "opthalmologists(eye)"
                        },
                        new SelectListItem {
                            Text = "CVS (cardio vascular system/heart)", Value = "CVS(cardio vascular system/heart)"
                        },
                        new SelectListItem {
                            Text = "ENDOCRINOLOGY", Value = "ENDOCRINOLOGY"
                        },
                        new SelectListItem {
                            Text = "GYNAECOLOGY & OBS", Value = "GYNAECOLOGY & OBS"
                        },
                        new SelectListItem {
                            Text = "GENETICS", Value = "GENETICS"
                        },
                    };
            ViewBag.DISEASES = diseases;

            List<SelectListItem> referby = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "PATIENT", Value = "PATIENT"
                },
                new SelectListItem {
                    Text = "TV ADV", Value = "TV ADV"
                },
                new SelectListItem {
                    Text = "SOCIAL MEDIA", Value = "SOCIAL MEDIA"
                },
                new SelectListItem {
                    Text = "WALL PAINTING", Value = "WALL PAINTING"
                },
                new SelectListItem {
                    Text = "POSTERS", Value = "POSTERS"
                },
                new SelectListItem {
                    Text = "INTERNET SEARCH", Value = "INTERNET SEARCH"
                },
                new SelectListItem {
                    Text = "CALLS", Value = "CALLS"
                },
                new SelectListItem {
                    Text = "DOCTOR", Value = "DOCTOR"
                },
                new SelectListItem {
                    Text = "HOSPITAL", Value = "HOSPITAL"
                },
                new SelectListItem {
                    Text = "MAIL/MSG", Value = "MAILMSG"
                },
                new SelectListItem {
                    Text = "RANDOM", Value = "RANDOM"
                },

            };
            ViewBag.REFERBYS = referby;

            List<SelectListItem> UserList = new List<SelectListItem>();
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            #region Code to get doctors list for refer to
            QuerySnapshot snapUsersDoctors = await db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
            foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
            {
                SelectListItem user = new SelectListItem();
                user.Text = docsnapUsers.GetValue<string>("name");
                user.Value = docsnapUsers.Id;
                if (docsnapUsers.Exists)
                {
                    UserList.Add(user);
                }
            }
            ViewBag.USERS = UserList;
            #endregion Code to get doctors list for refer to


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

                        string patientLastId = "";
                        string patientLastIdDocId = "";

                        

                        if (ModelState.IsValid)
                        {


                            //string lastTokenNumber = "0";
                            
                            DocumentReference docRefClinicCity = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId);
                            DocumentSnapshot docSnapClinicCity = await docRefClinicCity.GetSnapshotAsync();

                            #region Code to checkduplicacy of patient on the basis of name and mobile number
                            Query Qref = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").WhereEqualTo("patient_name", patient.patient_name).WhereEqualTo("patient_mobile_number", patient.patient_mobile_number);
                            QuerySnapshot snap = await Qref.GetSnapshotAsync();
                            if (snap.Count > 0)
                            {
                                ViewBag.Message = "Patient " + patient.patient_name + " having Mobile number " + patient.patient_mobile_number + " already exists. ";
                            }
                            #endregion Code to checkduplicacy of patient on the basis of name and mobile number
                            else
                            {
                                #region Code to generate new patient UID and update in paltientLastId collection
                                Query QrefPatientLastId = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientLastId").Limit(1);
                                QuerySnapshot snapPatientLastId = await QrefPatientLastId.GetSnapshotAsync();
                                if (snapPatientLastId.Count > 0)
                                {
                                    DocumentSnapshot docsnap2 = snapPatientLastId.Documents[0];
                                    patientLastIdDocId = docsnap2.Id;

                                    patientLastId = PatientLastId(docsnap2.GetValue<string>("id"));//Code to get plus one patientLastId

                                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientLastId").Document(patientLastIdDocId);
                                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                                    Dictionary<string, object> dataPatientLastId = new Dictionary<string, object>
                            {
                                {"id" ,patientLastId}
                            };

                                    if (docSnap.Exists)
                                    {
                                        await docRef.UpdateAsync(dataPatientLastId);
                                    }

                                }
                                #endregion Code to generate new patient UID and update in paltientLastId collection
                                #region Code to create new Patient
                                CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList");

                                Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"patient_name" ,patient.patient_name.ToLower()},
                            {"age" ,patient.age},
                            {"care_of" ,patient.care_of},
                            {"city" ,patient.city},
                            {"creation_date" ,DateTime.UtcNow},
                            {"disease" ,patient.disease},
                            {"gender" ,patient.gender},
                            {"severity" ,patient.severity},
                            {"patient_id" ,patientLastId},
                            {"patient_mobile_number",patient.patient_mobile_number},
                            {"refer_by" ,patient.refer_by},
                            {"refer_to_doctor" ,patient.refer_to_doctor},
                            {"search_text" ,patient.patient_name+patient.patient_mobile_number+patientLastId}
                        };
                                await col1.Document().SetAsync(data1);
                                #endregion Code to create new Patient
                                #region Code to get newly created patient's auto id
                                Query QrefLatestPatient = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").WhereEqualTo("patient_id", patientLastId).Limit(1);
                                QuerySnapshot snapLatestPatient = await QrefLatestPatient.GetSnapshotAsync();

                                DocumentSnapshot docSnapLatestPatient = snapLatestPatient.Documents[0];
                                #endregion Code to get newly created patient's auto id
                                #region Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection (Commented)
                                //DocumentReference docRefTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(DateTime.Now.ToString("dd_MM_yyyy"));
                                //DocumentSnapshot docsnapTokenNumber = await docRefTokenNumber.GetSnapshotAsync();

                                //if (docsnapTokenNumber.Exists)
                                //{
                                //    lastTokenNumber = docsnapTokenNumber.GetValue<string>("last_token");
                                //}
                                //else
                                //{
                                //    lastTokenNumber = "0";
                                //}

                                //lastTokenNumber = (Convert.ToInt32(lastTokenNumber) + 1).ToString();

                                //CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                                //Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                                //{
                                //    {"last_token" ,lastTokenNumber},
                                //    {"assigned_tokens" ,null}

                                //};
                                //await colTokenNumber.Document(DateTime.Now.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);

                                #endregion Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection
                                if (patient.createAppointment == "Yes")
                                {
                                    #region Code to check duplicate appointment for selected date having status waiting

                                    DateTime ConvertedAppDate = DateTime.SpecifyKind(Convert.ToDateTime(patient.appointment_date), DateTimeKind.Utc);

                                    Query QrefduplicateApp = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", docSnapLatestPatient.Id).WhereGreaterThanOrEqualTo("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.Date)).WhereLessThan("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.AddDays(1))).WhereEqualTo("status", "Waiting");
                                    QuerySnapshot duplicateApp = await QrefduplicateApp.GetSnapshotAsync();
                                    if (duplicateApp.Count > 0)
                                    {
                                        ViewBag.Message = "Appointment of " + patient.patient_name + "(" + patient.patient_id + ") for " + patient.appointment_date + " already exists. ";
                                        return View(patient);
                                    }
                                    else
                                    {

                                        #endregion Code to check duplicate appointment for selected date having status waiting

                                        string message = await UpdateTokenNumber(patient.appointment_date.ToString(), patient.tokenNumber);
                                        if (message != null)
                                        {
                                            ViewBag.Message = message;
                                            return View(patient);
                                        }
                                        #region Code to create new appointment id for today


                                        CollectionReference colAppountments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments");

                                        Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,null},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,""},
                                    {"fee" ,""},
                                    {"patient" ,docSnapLatestPatient.Id},
                                    {"patient_id" ,patientLastId},
                                    {"raisedDate",DateTime.SpecifyKind(patient.appointment_date.AddHours(-5).AddMinutes(-30), DateTimeKind.Utc)},
                                    {"reminder_sms" ,false},
                                    {"severity" ,"Low"},
                                    {"status" ,"Waiting"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,patient.tokenNumber},
                                    {"refertodoctor" ,patient.refer_to_doctor}
                                };
                                        await colAppountments.Document().SetAsync(dataAppointment);
                                        #endregion Code to create new appointment id for today

                                        return RedirectToAction("Index", "Appointment");
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Patient");
                                }



                            }



                            return View(patient);


                        }
                        else
                        {

                            return View(patient);
                        }


                    }
                    catch (Exception ex)
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

                        string patientLastId = "";
                        string patientLastIdDocId = "";

                        if (ModelState.IsValid)
                        {


                            
                            DocumentReference docRefClinicCity = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId);
                            DocumentSnapshot docSnapClinicCity = await docRefClinicCity.GetSnapshotAsync();

                            

                            #region Code to checkduplicacy of patient on the basis of name and mobile number
                            Query Qref = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").WhereEqualTo("patient_name", patient.patient_name).WhereEqualTo("patient_mobile_number", patient.patient_mobile_number);
                            QuerySnapshot snap = await Qref.GetSnapshotAsync();
                            if (snap.Count > 0)
                            {
                                ViewBag.Message = "Patient " + patient.patient_name + " having Mobile number " + patient.patient_mobile_number + " already exists. ";
                            }
                            #endregion Code to checkduplicacy of patient on the basis of name and mobile number
                            Query QrefUsers = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("users").WhereEqualTo("patient_name", patient.patient_name).WhereEqualTo("patient_mobile_number", patient.patient_mobile_number);
                            QuerySnapshot snapUsers = await QrefUsers.GetSnapshotAsync();
                            if (snapUsers.Count > 0)
                            {
                                ViewBag.Message = "Patient " + patient.patient_name + " having Mobile number " + patient.patient_mobile_number + " already exists. ";
                            }

                            #region Code to generate new patient UID and update in paltientLastId collection
                            else
                            {
                                Query QrefPatientLastId = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientLastId").Limit(1);
                                QuerySnapshot snapPatientLastId = await QrefPatientLastId.GetSnapshotAsync();
                                if (snapPatientLastId.Count > 0)
                                {
                                    DocumentSnapshot docsnap2 = snapPatientLastId.Documents[0];
                                    patientLastIdDocId = docsnap2.Id;

                                    patientLastId = PatientLastId(docsnap2.GetValue<string>("id"));//Code to get plus one patientLastId

                                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientLastId").Document(patientLastIdDocId);
                                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                                    Dictionary<string, object> dataPatientLastId = new Dictionary<string, object>
                            {
                                {"id" ,patientLastId}
                            };

                                    if (docSnap.Exists)
                                    {
                                        await docRef.UpdateAsync(dataPatientLastId);
                                    }

                                }
                                #endregion Code to generate new patient UID and update in paltientLastId collection
                                #region Code to create new Patient
                                CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList");

                                Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"patient_name" ,patient.patient_name.ToLower()},
                            {"age" ,patient.age},
                            {"care_of" ,patient.care_of},
                            {"city" ,patient.city},
                            {"creation_date" ,DateTime.UtcNow},
                            {"disease" ,patient.disease},
                            {"gender" ,patient.gender},
                            {"severity" ,patient.severity},
                            {"patient_id" ,patientLastId},
                            {"patient_mobile_number",patient.patient_mobile_number},
                            {"refer_by" ,patient.refer_by},
                            {"refer_to_doctor" ,patient.refer_to_doctor},
                            {"search_text" ,patient.patient_name+patient.patient_mobile_number+patientLastId}
                        };
                                await col1.Document().SetAsync(data1);
                                #endregion Code to create new Patient
                                #region Code to get newly created patient's auto id
                                Query QrefLatestPatient = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").WhereEqualTo("patient_id", patientLastId).Limit(1);
                                QuerySnapshot snapLatestPatient = await QrefLatestPatient.GetSnapshotAsync();

                                DocumentSnapshot docSnapLatestPatient = snapLatestPatient.Documents[0];
                                #endregion Code to get newly created patient's auto id
                                #region Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection (Commented)
                                //DocumentReference docRefTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(DateTime.Now.ToString("dd_MM_yyyy"));
                                //DocumentSnapshot docsnapTokenNumber = await docRefTokenNumber.GetSnapshotAsync();

                                //if (docsnapTokenNumber.Exists)
                                //{
                                //    lastTokenNumber = docsnapTokenNumber.GetValue<string>("last_token");
                                //}
                                //else
                                //{
                                //    lastTokenNumber = "0";
                                //}

                                //lastTokenNumber = (Convert.ToInt32(lastTokenNumber) + 1).ToString();

                                //CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                                //Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                                //{
                                //    {"last_token" ,lastTokenNumber},
                                //    {"assigned_tokens" ,null}

                                //};
                                //await colTokenNumber.Document(DateTime.Now.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);

                                #endregion Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection
                                if (patient.createAppointment == "Yes")
                                {
                                    #region Code to check duplicate appointment for selected date having status waiting

                                    DateTime ConvertedAppDate = DateTime.SpecifyKind(Convert.ToDateTime(patient.appointment_date), DateTimeKind.Utc);

                                    Query QrefduplicateApp = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", docSnapLatestPatient.Id).WhereGreaterThanOrEqualTo("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.Date)).WhereLessThan("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.AddDays(1))).WhereEqualTo("status", "Waiting");
                                    QuerySnapshot duplicateApp = await QrefduplicateApp.GetSnapshotAsync();
                                    if (duplicateApp.Count > 0)
                                    {
                                        ViewBag.Message = "Appointment of " + patient.patient_name + "(" + patient.patient_id + ") for " + patient.appointment_date + " already exists. ";
                                        return View(patient);
                                    }
                                    else
                                    {

                                        #endregion Code to check duplicate appointment for selected date having status waiting

                                        string message = await UpdateTokenNumber(patient.appointment_date.ToString(), patient.tokenNumber);
                                        if (message != null)
                                        {
                                            ViewBag.Message = message;
                                            return View(patient);
                                        }
                                        #region Code to create new appointment id for today


                                        CollectionReference colAppountments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments");

                                        Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,null},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,""},
                                    {"fee" ,""},
                                    {"patient" ,docSnapLatestPatient.Id},
                                    {"patient_id" ,patientLastId},
                                    {"raisedDate",DateTime.SpecifyKind(patient.appointment_date.AddHours(-5).AddMinutes(-30), DateTimeKind.Utc)},
                                    {"reminder_sms" ,false},
                                    {"severity" ,"Low"},
                                    {"status" ,"Waiting"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,patient.tokenNumber},
                                    {"refertodoctor" ,patient.refer_to_doctor}
                                };
                                        await colAppountments.Document().SetAsync(dataAppointment);
                                        #endregion Code to create new appointment id for today

                                        return RedirectToAction("Index", "Appointment");
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Patient");
                                }



                            }



                            return View(patient);


                        }
                        else
                        {

                            return View(patient);
                        }


                    }
                    catch (Exception ex)
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

        public async Task<ActionResult> Edit(string id)
        {
            List<SelectListItem> diseases = new List<SelectListItem>() {
                        new SelectListItem {
                            Text = "General -(fever/general acute complaints)", Value = "General"
                        },
                        new SelectListItem {
                            Text = "ENT (ear nose throat)", Value = "ENT(ear nose throat)"
                        },
                        new SelectListItem {
                            Text = "CNS (central nervous system", Value = "CNS(central nervous system)"
                        },
                        new SelectListItem {
                            Text = "DERMATO (skin/hair)", Value = "DERMATO./SKIN/hair"
                        },
                        new SelectListItem {
                            Text = "GIT (gastro intestinal track)", Value = "GIT(gastro intestinal track)"
                        },
                        new SelectListItem {
                            Text = "SURGERY", Value = "SURGERY"
                        },
                        new SelectListItem {
                            Text = "TUMORS/ONCOLOGY", Value = "TUMOURS/ONCOLOGY"
                        },
                        new SelectListItem {
                            Text = "HAEMATOLOGY (blood)", Value = "HAEMATOLOGY(blood)"
                        },
                        new SelectListItem {
                            Text = "ORTHOPADICS (bones & muscles)", Value = "ORTHOPADICS(bones & muscles)"
                        },
                        new SelectListItem {
                            Text = "PAEDIATRIC (child)", Value = "paediatric"
                        },
                        new SelectListItem {
                            Text = "OPTHALMOLOGISTS (EYE)", Value = "opthalmologists(eye)"
                        },
                        new SelectListItem {
                            Text = "CVS (cardio vascular system/heart)", Value = "CVS(cardio vascular system/heart)"
                        },
                        new SelectListItem {
                            Text = "ENDOCRINOLOGY", Value = "ENDOCRINOLOGY"
                        },
                        new SelectListItem {
                            Text = "GYNAECOLOGY & OBS", Value = "GYNAECOLOGY & OBS"
                        },
                        new SelectListItem {
                            Text = "GENETICS", Value = "GENETICS"
                        },
                    };
            ViewBag.DISEASES = diseases;


            List<SelectListItem> referby = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "PATIENT", Value = "PATIENT"
                },
                new SelectListItem {
                    Text = "TV ADV", Value = "TV ADV"
                },
                new SelectListItem {
                    Text = "SOCIAL MEDIA", Value = "SOCIAL MEDIA"
                },
                new SelectListItem {
                    Text = "WALL PAINTING", Value = "WALL PAINTING"
                },
                new SelectListItem {
                    Text = "POSTERS", Value = "POSTERS"
                },
                new SelectListItem {
                    Text = "INTERNET SEARCH", Value = "INTERNET SEARCH"
                },
                new SelectListItem {
                    Text = "CALLS", Value = "CALLS"
                },
                new SelectListItem {
                    Text = "DOCTOR", Value = "DOCTOR"
                },
                new SelectListItem {
                    Text = "HOSPITAL", Value = "HOSPITAL"
                },
                new SelectListItem {
                    Text = "MAIL/MSG", Value = "MAILMSG"
                },
                new SelectListItem {
                    Text = "RANDOM", Value = "RANDOM"
                },

            };
            ViewBag.REFERBYS = referby;

            List<SelectListItem> UserList = new List<SelectListItem>();

            if (Session["sessionid"] == null)
            { Session["sessionid"] = "empty"; }

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (await IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (!await IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString()))
                {
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");
                    

                    //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(id);
                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                    Patient patient = docSnap.ConvertTo<Patient>();
                    patient.appointment_date = DateTime.UtcNow;
                    patient.tokenNumber = "0";
                    //patient.id = id;

                    #region Code to get doctors list for refer to
                    QuerySnapshot snapUsersDoctors = await db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                    {
                        SelectListItem user = new SelectListItem();
                        user.Text = docsnapUsers.GetValue<string>("name");
                        user.Value = docsnapUsers.Id;
                        if (docsnapUsers.Exists)
                        {
                            UserList.Add(user);
                        }
                    }
                    ViewBag.USERS = UserList;
                    #endregion Code to Code to get doctors list for refer to

                    return View(patient);
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");
                    
                    //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(id);
                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                    Patient patient = docSnap.ConvertTo<Patient>();
                    patient.appointment_date = DateTime.UtcNow;
                    patient.tokenNumber = "0";
                    //patient.id = id;
                    #region Code to get doctors list for refer to
                    QuerySnapshot snapUsersDoctors = await db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
                    {
                        SelectListItem user = new SelectListItem();
                        user.Text = docsnapUsers.GetValue<string>("name");
                        user.Value = docsnapUsers.Id;
                        if (docsnapUsers.Exists)
                        {
                            UserList.Add(user);
                        }
                    }
                    ViewBag.USERS = UserList;
                    #endregion Code to get doctors list for refer to
                    return View(patient);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Edit(string id, Patient patient)
        {
            List<SelectListItem> diseases = new List<SelectListItem>() {
                        new SelectListItem {
                            Text = "General -(fever/general acute complaints)", Value = "General"
                        },
                        new SelectListItem {
                            Text = "ENT (ear nose throat)", Value = "ENT(ear nose throat)"
                        },
                        new SelectListItem {
                            Text = "CNS (central nervous system", Value = "CNS(central nervous system)"
                        },
                        new SelectListItem {
                            Text = "DERMATO (skin/hair)", Value = "DERMATO./SKIN/hair"
                        },
                        new SelectListItem {
                            Text = "GIT (gastro intestinal track)", Value = "GIT(gastro intestinal track)"
                        },
                        new SelectListItem {
                            Text = "SURGERY", Value = "SURGERY"
                        },
                        new SelectListItem {
                            Text = "TUMORS/ONCOLOGY", Value = "TUMOURS/ONCOLOGY"
                        },
                        new SelectListItem {
                            Text = "HAEMATOLOGY (blood)", Value = "HAEMATOLOGY(blood)"
                        },
                        new SelectListItem {
                            Text = "ORTHOPADICS (bones & muscles)", Value = "ORTHOPADICS(bones & muscles)"
                        },
                        new SelectListItem {
                            Text = "PAEDIATRIC (child)", Value = "paediatric"
                        },
                        new SelectListItem {
                            Text = "OPTHALMOLOGISTS (EYE)", Value = "opthalmologists(eye)"
                        },
                        new SelectListItem {
                            Text = "CVS (cardio vascular system/heart)", Value = "CVS(cardio vascular system/heart)"
                        },
                        new SelectListItem {
                            Text = "ENDOCRINOLOGY", Value = "ENDOCRINOLOGY"
                        },
                        new SelectListItem {
                            Text = "GYNAECOLOGY & OBS", Value = "GYNAECOLOGY & OBS"
                        },
                        new SelectListItem {
                            Text = "GENETICS", Value = "GENETICS"
                        },
                    };
            ViewBag.DISEASES = diseases;


            List<SelectListItem> referby = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "PATIENT", Value = "PATIENT"
                },
                new SelectListItem {
                    Text = "TV ADV", Value = "TV ADV"
                },
                new SelectListItem {
                    Text = "SOCIAL MEDIA", Value = "SOCIAL MEDIA"
                },
                new SelectListItem {
                    Text = "WALL PAINTING", Value = "WALL PAINTING"
                },
                new SelectListItem {
                    Text = "POSTERS", Value = "POSTERS"
                },
                new SelectListItem {
                    Text = "INTERNET SEARCH", Value = "INTERNET SEARCH"
                },
                new SelectListItem {
                    Text = "CALLS", Value = "CALLS"
                },
                new SelectListItem {
                    Text = "DOCTOR", Value = "DOCTOR"
                },
                new SelectListItem {
                    Text = "HOSPITAL", Value = "HOSPITAL"
                },
                new SelectListItem {
                    Text = "MAIL/MSG", Value = "MAILMSG"
                },
                new SelectListItem {
                    Text = "RANDOM", Value = "RANDOM"
                },

            };
            ViewBag.REFERBYS = referby;


            List<SelectListItem> UserList = new List<SelectListItem>();
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            #region Code to get doctors list for refer to
            QuerySnapshot snapUsersDoctors = await db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").WhereArrayContains("user_roles", "Doctor").GetSnapshotAsync();
            foreach (DocumentSnapshot docsnapUsers in snapUsersDoctors)
            {
                SelectListItem user = new SelectListItem();
                user.Text = docsnapUsers.GetValue<string>("name");
                user.Value = docsnapUsers.Id;
                if (docsnapUsers.Exists)
                {
                    UserList.Add(user);
                }
            }
            ViewBag.USERS = UserList;
            #endregion Code to get doctors list for refer to

            



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
                        

                        
                        if (ModelState.IsValid)
                        {
                            

                            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(id);
                            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"patient_name" ,patient.patient_name.ToLower()},
                            {"age" ,patient.age},
                            {"care_of" ,patient.care_of},
                            {"city" ,patient.city},
                            {"creation_date" ,DateTime.SpecifyKind(patient.creation_date, DateTimeKind.Utc)},
                            {"disease" ,patient.disease},
                            {"gender" ,patient.gender},
                            {"severity" ,patient.severity},
                            {"patient_id" ,patient.patient_id},
                            {"patient_mobile_number",patient.patient_mobile_number},
                            {"refer_by" ,patient.refer_by},
                            {"refer_to_doctor" ,patient.refer_to_doctor},
                            {"search_text" ,patient.patient_name+patient.patient_mobile_number+patient.patient_id}
                        };


                            if (docSnap.Exists)
                            {
                                await docRef.UpdateAsync(data1);
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {

                            return View(patient);
                        }


                    }
                    catch
                    {
                        return View(patient);
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    
                    
                    try
                    {
                        
                        if (ModelState.IsValid)
                        {
                            

                            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(id);
                            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                            Dictionary<string, object> data1 = new Dictionary<string, object>
                        {
                            {"patient_name" ,patient.patient_name.ToLower()},
                            {"age" ,patient.age},
                            {"care_of" ,patient.care_of},
                            {"city" ,patient.city},
                            {"creation_date" ,DateTime.SpecifyKind(patient.creation_date, DateTimeKind.Utc)},
                            {"disease" ,patient.disease},
                            {"gender" ,patient.gender},
                            {"severity" ,patient.severity},
                            {"patient_id" ,patient.patient_id},
                            {"patient_mobile_number",patient.patient_mobile_number},
                            {"refer_by" ,patient.refer_by},
                            {"refer_to_doctor" ,patient.refer_to_doctor},
                            {"search_text" ,patient.patient_name+patient.patient_mobile_number+patient.patient_id}
                        };


                            if (docSnap.Exists)
                            {
                                await docRef.UpdateAsync(data1);
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {

                            return View(patient);
                        }


                    }
                    catch
                    {
                        return View(patient);
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Patient/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Patient/Delete/5
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

        // GET: Patient/AddPres/5
        public ActionResult AddPres(int id)
        {
            return View();
        }

        // POST: Patient/AddPres/5
        [HttpPost]
        public ActionResult AddPres(int id, FormCollection collection)
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

        private string PatientLastId(string databaselastid)
        {
            int numericValue = Convert.ToInt32(databaselastid.Substring(databaselastid.Length - 4));
            string alphaValue = databaselastid.Substring(0, databaselastid.Length - 4);
            if (numericValue == 9999)
            {
                numericValue = 1;
                switch (alphaValue)
                {
                    case "A":
                        alphaValue = "B000";
                        break;
                    case "B":
                        alphaValue = "C000";
                        break;
                    default:
                        // code block
                        break;
                }

            } 
            else
            {
                numericValue = numericValue + 1;
                if(numericValue.ToString().Length == 1)
                {
                    alphaValue = alphaValue + "000";
                }
                if (numericValue.ToString().Length == 2)
                {
                    alphaValue = alphaValue + "00";
                }
                if (numericValue.ToString().Length == 3)
                {
                    alphaValue = alphaValue + "0";
                }
            }

            
            databaselastid = alphaValue + numericValue.ToString();
            return databaselastid;
        }

        // POST: Patient/Delete/5

        [HttpPost]
        public async Task<ActionResult> CreateFutureAppointment(FormCollection collection)
        {
            DateTime CompletionDate;//field used when refer to has Chemist role
            string fee = "";
            string days = "";
            List<Appointment> AppointmentList = new List<Appointment>();

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

                        string patientAutoId = collection["patientAutoId"];
                        string token = collection["tokennumber"];
                        string appointmentDate = collection["datepicker"];
                        string referto = collection["referto"];
                        



                        DateTime ConvertedAppDate = DateTime.SpecifyKind(Convert.ToDateTime(appointmentDate).AddHours(-5).AddMinutes(-30), DateTimeKind.Utc);





                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRefPatientUID = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId);
                        DocumentSnapshot docsnapPatientUID = await docRefPatientUID.GetSnapshotAsync();

                        string PatientUID = docsnapPatientUID.GetValue<string>("patient_id");
                        string severity = "";
                        try { severity = docsnapPatientUID.GetValue<string>("severity"); }
                        catch
                        {
                            severity = "Low";
                        }

                        string PatientName = docsnapPatientUID.GetValue<string>("patient_name");


                        Query Qref = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", patientAutoId).WhereGreaterThanOrEqualTo("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.Date)).WhereLessThan("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.AddDays(1))).WhereEqualTo("status", "Waiting");
                        QuerySnapshot snap = await Qref.GetSnapshotAsync();
                        if (snap.Count > 0)
                        {

                            TempData["Message"] = "Appointment of " + PatientName + "(" + PatientUID + ") for " + appointmentDate + " already exists. ";

                            return RedirectToAction("Index", "Patient");

                        }
                        else
                        {

                            #region Code to create new appointment id for today

                            #region Check refer to is chemist

                            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(referto);
                            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                            User user = docSnap.ConvertTo<User>();

                            CollectionReference colAppountments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments");


                            if (user.user_roles.Contains("Chemist"))
                            {
                                Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId).Collection("prescriptions").OrderByDescending("timeStamp");


                                QuerySnapshot snapPres = await QrefPrescriptions.GetSnapshotAsync();
                                if (snapPres.Count == 0)
                                {
                                    TempData["Message"] = "Selected Patient has no Prescription";
                                    return RedirectToAction("Index", "Patient");
                                }

                                Query QrefCompletedAppointments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", patientAutoId).WhereEqualTo("status","Completed");
                                QuerySnapshot snapCompletedAppointments = await QrefCompletedAppointments.GetSnapshotAsync();
                                if (snapCompletedAppointments.Count > 0)
                                {
                                    foreach (DocumentSnapshot docsnapCompletedAppointment in snapCompletedAppointments)
                                    {
                                        Appointment appointment = docsnapCompletedAppointment.ConvertTo<Appointment>();
                                        AppointmentList.Add(appointment);
                                    }

                                    AppointmentList = AppointmentList.OrderByDescending(a => a.raisedDate).ToList();

                                    Appointment app = AppointmentList.FirstOrDefault();

                                    fee = app.fee;
                                    days = app.days;
                                        
                                }
                                else
                                {
                                    TempData["Message"] = "Selected Patient has not been checked by Doctor yet";
                                    return RedirectToAction("Index", "Patient");
                                }

                                CompletionDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                                string message = await UpdateTokenNumber(appointmentDate, token);
                                if (message != null)
                                {
                                    TempData["Message"] = message;
                                    return RedirectToAction("Index", "Patient");
                                }

                                Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,CompletionDate},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,days},
                                    {"fee" ,fee},
                                    {"patient" ,patientAutoId},
                                    {"patient_id" ,PatientUID},
                                    {"raisedDate",CompletionDate},
                                    {"reminder_sms" ,false},
                                    {"severity" ,severity},
                                    {"status" ,"Completed"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,token},
                                    {"refertodoctor" ,referto}
                                };
                                await colAppountments.Document().SetAsync(dataAppointment);
                            }
                            else
                            {
                                string message = await UpdateTokenNumber(appointmentDate, token);
                                if (message != null)
                                {
                                    TempData["Message"] = message;
                                    return RedirectToAction("Index", "Patient");
                                }

                                Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,null},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,""},
                                    {"fee" ,""},
                                    {"patient" ,patientAutoId},
                                    {"patient_id" ,PatientUID},
                                    {"raisedDate",ConvertedAppDate},
                                    {"reminder_sms" ,false},
                                    {"severity" ,severity},
                                    {"status" ,"Waiting"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,token},
                                    {"refertodoctor" ,referto}
                                };
                                await colAppountments.Document().SetAsync(dataAppointment);
                            }
                            #endregion Check refer to is chemist or Doctor
                            #endregion Code to create new appointment id for today
                        }
                        return RedirectToAction("Index", "Appointment");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                }
                else
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name.Split('-')[0], Session["sessionid"].ToString());
                    try
                    {

                        string patientAutoId = collection["patientAutoId"];
                        string token = collection["tokennumber"];
                        string appointmentDate = collection["datepicker"];
                        string referto = collection["referto"];

                        DateTime ConvertedAppDate = DateTime.SpecifyKind(Convert.ToDateTime(appointmentDate).AddHours(-5).AddMinutes(-30), DateTimeKind.Utc);





                        string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                        FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        DocumentReference docRefPatientUID = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId);
                        DocumentSnapshot docsnapPatientUID = await docRefPatientUID.GetSnapshotAsync();

                        string PatientUID = docsnapPatientUID.GetValue<string>("patient_id");
                        string severity = "";
                        try { severity = docsnapPatientUID.GetValue<string>("severity"); }
                        catch
                        {
                            severity = "Low";
                        }

                        string PatientName = docsnapPatientUID.GetValue<string>("patient_name");


                        Query Qref = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", patientAutoId).WhereGreaterThanOrEqualTo("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.Date)).WhereLessThan("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.AddDays(1))).WhereEqualTo("status", "Waiting");
                        QuerySnapshot snap = await Qref.GetSnapshotAsync();
                        if (snap.Count > 0)
                        {
                            TempData["Message"] = "Appointment of " + PatientName + "(" + PatientUID + ") for " + appointmentDate + " already exists. ";

                            return RedirectToAction("Index", "Patient");

                        }
                        else
                        {

                            #region Code to create new appointment id for today

                            



                            #region Check refer to is chemist

                            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(referto);
                            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                            User user = docSnap.ConvertTo<User>();






                            CollectionReference colAppountments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments");


                            if (user.user_roles.Contains("Chemist"))
                            {
                                Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId).Collection("prescriptions").OrderByDescending("timeStamp");


                                QuerySnapshot snapPres = await QrefPrescriptions.GetSnapshotAsync();
                                if (snapPres.Count == 0)
                                {
                                    TempData["Message"] = "Selected Patient has no Prescription";
                                    return RedirectToAction("Index", "Patient");
                                }

                                Query QrefCompletedAppointments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", patientAutoId).WhereEqualTo("status", "Completed");
                                QuerySnapshot snapCompletedAppointments = await QrefCompletedAppointments.GetSnapshotAsync();
                                if (snapCompletedAppointments.Count > 0)
                                {
                                    foreach (DocumentSnapshot docsnapCompletedAppointment in snapCompletedAppointments)
                                    {
                                        Appointment appointment = docsnapCompletedAppointment.ConvertTo<Appointment>();
                                        AppointmentList.Add(appointment);
                                    }

                                    AppointmentList = AppointmentList.OrderByDescending(a => a.raisedDate).ToList();

                                    Appointment app = AppointmentList.FirstOrDefault();

                                    fee = app.fee;
                                    days = app.days;

                                }
                                else
                                {
                                    TempData["Message"] = "Selected Patient has not been checked by Doctor yet";
                                    return RedirectToAction("Index", "Patient");
                                }

                                CompletionDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                                string message = await UpdateTokenNumber(appointmentDate, token);
                                if (message != null)
                                {
                                    TempData["Message"] = message;
                                    return RedirectToAction("Index", "Patient");
                                }

                                Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,CompletionDate},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,days},
                                    {"fee" ,fee},
                                    {"patient" ,patientAutoId},
                                    {"patient_id" ,PatientUID},
                                    {"raisedDate",CompletionDate},
                                    {"reminder_sms" ,false},
                                    {"severity" ,severity},
                                    {"status" ,"Completed"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,token},
                                    {"refertodoctor" ,referto}
                                };
                                await colAppountments.Document().SetAsync(dataAppointment);
                            }
                            else
                            {
                                string message = await UpdateTokenNumber(appointmentDate, token);
                                if (message != null)
                                {
                                    TempData["Message"] = message;
                                    return RedirectToAction("Index", "Patient");
                                }
                                Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                                {
                                    {"bill_sms" ,false},
                                    {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                                    {"completionDate" ,null},
                                    {"completiondateChemist" ,null},
                                    {"completiondateCashier" ,null},
                                    {"statusChemist" ,null},
                                    {"statusCashier" ,null},
                                    {"date" ,""},
                                    {"days" ,""},
                                    {"fee" ,""},
                                    {"patient" ,patientAutoId},
                                    {"patient_id" ,PatientUID},
                                    {"raisedDate",ConvertedAppDate},
                                    {"reminder_sms" ,false},
                                    {"severity" ,severity},
                                    {"status" ,"Waiting"},
                                    {"timeStamp" ,DateTime.UtcNow},
                                    {"token" ,token},
                                    {"refertodoctor" ,referto}
                                };
                                await colAppountments.Document().SetAsync(dataAppointment);
                            }
                            #endregion Check refer to is chemist or Doctor
                            #endregion Code to create new appointment id for today
                        }
                        return RedirectToAction("Index", "Appointment");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Home");
            }
        }
        // POST: Patient/Delete/5
        [HttpPost]
        public async Task<ActionResult> CreateFutureAppointment1(FormCollection collection)
        {
            try
            {

                string patientAutoId = collection["patientAutoId"];
                string token = collection["tokennumber"];
                string appointmentDate = collection["datepicker"];

                DateTime ConvertedAppDate = DateTime.SpecifyKind(Convert.ToDateTime(appointmentDate).AddHours(-5).AddMinutes(-30), DateTimeKind.Utc);

                
                
                

                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                DocumentReference docRefPatientUID = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId);
                DocumentSnapshot docsnapPatientUID = await docRefPatientUID.GetSnapshotAsync();

                string PatientUID = docsnapPatientUID.GetValue<string>("patient_id");
                string severity = "";
                try { severity = docsnapPatientUID.GetValue<string>("severity"); }
                catch {
                    severity = "Low";
                }
                                
                string PatientName = docsnapPatientUID.GetValue<string>("patient_name");
                

                Query Qref = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").WhereEqualTo("patient", patientAutoId).WhereGreaterThanOrEqualTo("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.Date)).WhereLessThan("raisedDate", Timestamp.FromDateTime(ConvertedAppDate.AddDays(1))).WhereEqualTo("status","Waiting");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();
                if (snap.Count > 0)
                {
                    TempData["Message"] = "Appointment of " + PatientName + "(" + PatientUID + ") for " + appointmentDate + " already exists. ";

                    return RedirectToAction("Index", "Patient");
                    
                }
                else
                {

                    #region Code to create new appointment id for today

                    string message = await UpdateTokenNumber(appointmentDate, token);
                    if (message != null)
                    {
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Patient");
                    }

                    CollectionReference colAppountments = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments");

                    Dictionary<string, object> dataAppointment = new Dictionary<string, object>
                        {
                            {"bill_sms" ,false},
                            {"clinic_id" ,GlobalSessionVariables.ClinicDocumentAutoId},
                            {"completionDate" ,null},
                            {"completiondateChemist" ,null},
                            {"completiondateCashier" ,null},
                            {"statusChemist" ,null},
                            {"statusCashier" ,null},
                            {"date" ,""},
                            {"days" ,""},
                            {"fee" ,""},
                            {"patient" ,patientAutoId},
                            {"patient_id" ,PatientUID},
                            {"raisedDate",ConvertedAppDate},
                            {"reminder_sms" ,false},
                            {"severity" ,severity},
                            {"status" ,"Waiting"},
                            {"timeStamp" ,DateTime.UtcNow},
                            {"token" ,token}
                        };
                    await colAppountments.Document().SetAsync(dataAppointment);
                    #endregion Code to create new appointment id for today
                }
                return RedirectToAction("Index", "Appointment");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Patient");
            }
        }

        private async Task<string> UpdateTokenNumber(string appointmentDate, string token)
        {
            string lastTokenNumber = "";
            string flag = "Y";
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            DateTime futAppDate = Convert.ToDateTime(appointmentDate);

            #region Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection
            DocumentReference docRefTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(futAppDate.ToString("dd_MM_yyyy"));
            DocumentSnapshot docsnapTokenNumber = await docRefTokenNumber.GetSnapshotAsync();


            if (docsnapTokenNumber.Exists)
            {
                lastTokenNumber = docsnapTokenNumber.GetValue<string>("last_token");
            }
            else
            {
                lastTokenNumber = "0";
                CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                            {
                                {"last_token" ,"0"},
                                {"assigned_tokens" ,null}
                            };
                await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
            }

            string lastTokenNumberReturned = "";

            if (lastTokenNumber != "0")
            {
                string[] assignedtokens = docsnapTokenNumber.GetValue<string[]>("assigned_tokens");


                if (assignedtokens != null)
                {
                    int[] assignedtokensInt = Array.ConvertAll(assignedtokens, int.Parse);

                    Array.Sort(assignedtokensInt);
                    int j = 0;
                    for (int i = 0; i < assignedtokens.Length; i++)
                    {
                        if (Convert.ToInt32(lastTokenNumber) + 1 > assignedtokensInt[i])
                        {
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
                            continue;
                        }
                        else if (Convert.ToInt32(lastTokenNumber) + 1 + j == assignedtokensInt[i])
                        {
                            j++;
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1 + j).ToString();
                        }
                        else
                        {
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1 + j).ToString();
                            break;
                        }

                    }
                }
                else
                {
                    lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
                }

            }
            else
            {
                lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
            }



            if (Convert.ToInt32(token) < Convert.ToInt32(lastTokenNumberReturned))
            {
                ViewBag.Message = "Token Number " + token + " is already assigned.";

            }
            if (Convert.ToInt32(token) < 1)
            {
                ViewBag.Message = "Token Number can not be negative.";

            }
            if (Convert.ToInt32(token) >= Convert.ToInt32(lastTokenNumberReturned))
            {
                if (Convert.ToInt32(token) == 1)
                {

                    DocumentReference docRefTokenNumber2 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(futAppDate.ToString("dd_MM_yyyy"));
                    DocumentSnapshot docsnapTokenNumber2 = await docRefTokenNumber2.GetSnapshotAsync();
                    string[] assignedtokens = docsnapTokenNumber2.GetValue<string[]>("assigned_tokens");

                    List<string> assignedtokenList = new List<string>();

                    if (assignedtokens != null)
                    {
                        for (int i = 0; i < assignedtokens.Length; i++)
                        {
                            if (token == assignedtokens[i].ToString())
                            {
                                TempData["Message"] = "Token Number " + token + " is already assigned.";
                                flag = "N";
                            }

                        }
                        assignedtokenList = assignedtokens.ToList();
                    }


                    if(flag=="Y")
                    {
                        CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");
                        if (assignedtokens != null)
                        {
                            Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                            {
                                {"last_token" ,token},
                                {"assigned_tokens" ,assignedtokenList.ToArray()}
                            };
                            await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
                        }
                        else
                        {
                            Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                            {
                                {"last_token" ,token},
                                {"assigned_tokens" ,null}
                            };
                            await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
                        }
                    }
                    



                }
                else if (Convert.ToInt32(token) == Convert.ToInt32(lastTokenNumberReturned))
                {

                    string[] assignedtokens = docsnapTokenNumber.GetValue<string[]>("assigned_tokens");
                    List<string> assignedtokenList = new List<string>();
                    if (assignedtokens != null)
                    {
                        for (int i = 0; i < assignedtokens.Length; i++)
                        {
                            if (token == assignedtokens[i].ToString())
                            {
                                ViewBag.Message = "Token Number " + token + " is already assigned.";
                                flag = "N";
                            }

                        }
                        assignedtokenList = assignedtokens.ToList();

                        if(flag == "Y")
                        {
                            CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                            Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                            {
                                {"last_token" ,token},
                                {"assigned_tokens" ,assignedtokenList.ToArray()}
                            };
                            await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
                        }
                        
                    }
                    else
                    {
                        CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                        Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                            {
                                {"last_token" ,token},
                                {"assigned_tokens" ,null}
                            };
                        await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
                    }
                }
                else
                {
                    DocumentReference docRefTokenNumber1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(futAppDate.ToString("dd_MM_yyyy"));
                    DocumentSnapshot docsnapTokenNumber1 = await docRefTokenNumber1.GetSnapshotAsync();
                    string[] assignedtokens = docsnapTokenNumber1.GetValue<string[]>("assigned_tokens");

                    List<string> assignedtokenList = new List<string>();

                    if (assignedtokens != null)
                    {
                        for (int i = 0; i < assignedtokens.Length; i++)
                        {
                            if (token == assignedtokens[i].ToString())
                            {
                                ViewBag.Message = "Token Number " + token + " is already assigned.";
                                flag = "N";
                            }

                        }
                        assignedtokenList = assignedtokens.ToList();
                    }



                    assignedtokenList.Add(token);

                    if (Convert.ToInt32(token) != Convert.ToInt32(lastTokenNumberReturned))
                    {
                        token = lastTokenNumber;
                    }

                    if(flag == "Y")
                    {
                        CollectionReference colTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber");

                        Dictionary<string, object> dataTokenNumber = new Dictionary<string, object>
                        {
                            {"last_token" ,token},
                            {"assigned_tokens" ,assignedtokenList.ToArray()}
                        };
                        await colTokenNumber.Document(futAppDate.ToString("dd_MM_yyyy")).SetAsync(dataTokenNumber);
                    }

                    
                }
            }

            return ViewBag.Message;
            #endregion Code to get latest token number, increament it
        }

        [HttpPost]
        public async Task<string> getLatestToken(string futureAppointmentDate)
        {
            string lastTokenNumber = "";
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

            DateTime futAppDate = Convert.ToDateTime(futureAppointmentDate);

            #region Code to get latest token number, increament it, set in new appointment and update increamented token number back in collection
            DocumentReference docRefTokenNumber = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("tokenNumber").Document(futAppDate.ToString("dd_MM_yyyy"));
            DocumentSnapshot docsnapTokenNumber = await docRefTokenNumber.GetSnapshotAsync();

            if (docsnapTokenNumber.Exists)
            {
                lastTokenNumber = docsnapTokenNumber.GetValue<string>("last_token");
            }
            else
            {
                lastTokenNumber = "0";
            }

            string lastTokenNumberReturned = "";

            if (lastTokenNumber != "0")
            {
                string[] assignedtokens = docsnapTokenNumber.GetValue<string[]>("assigned_tokens");

                
                if (assignedtokens != null)
                {
                    int[] assignedtokensInt = Array.ConvertAll(assignedtokens, int.Parse);

                    Array.Sort(assignedtokensInt);

                    int j = 0;
                    for (int i = 0; i < assignedtokens.Length; i++)
                    {
                        if(Convert.ToInt32(lastTokenNumber) + 1 > assignedtokensInt[i])
                        {
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
                            continue;
                        }
                        else if(Convert.ToInt32(lastTokenNumber) + 1 + j == assignedtokensInt[i])
                        {
                            j++;
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1 + j).ToString();
                        }
                        else
                        {
                            lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1 + j).ToString();
                            break;
                        }

                    }

                        
                }
                else
                {
                    lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
                }
                
            }
            else
            {
                lastTokenNumberReturned = (Convert.ToInt32(lastTokenNumber) + 1).ToString();
            }

            



            #endregion Code to get latest token number, increament it
            //ViewData["tokenNumber"] = lastTokenNumberReturned;
            
            
            return lastTokenNumberReturned;
        }

        [HttpPost]
        public string getCurrentServerDate(string xyz)
        {
            return DateTime.Now.ToString();
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
                    if (docsnapLoggedInUsers.GetValue<string>("sessionid") != sid)
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

        [HttpPost]
        public JsonResult cityautocomplete(string city)//I think that the id that you are passing here needs to be the search term. You may not have to change anything here, but you do in the $.ajax() call
        {
                List<city> cityList = new List<city>();
                string json = System.IO.File.ReadAllText(Server.MapPath(@"~/Content/JsonFiles/cities.json"));
                cityList = JsonConvert.DeserializeObject<List<city>>(json);
                cityList = cityList.Where(a => a.name.StartsWith(city.ToLower())).ToList();
                return Json(cityList);
        }
    }
}
