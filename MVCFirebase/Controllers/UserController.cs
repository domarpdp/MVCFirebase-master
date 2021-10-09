using Google.Cloud.Firestore;
using MVCFirebase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCFirebase.Controllers
{
    
    [AccessDeniedAuthorize(Roles = "SuperAdmin,Admin")]
    public class UserController : Controller
    {
        // GET: User
        [CheckSessionTimeOut]
        public async Task<ActionResult> Index()
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            List<User> UserList = new List<User>();


            //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
            Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
            QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnapClinics in snapClinics)
            {
                Clinic clinic = docsnapClinics.ConvertTo<Clinic>();
                QuerySnapshot snapUsers = await docsnapClinics.Reference.Collection("user").OrderByDescending("name").GetSnapshotAsync();

                foreach (DocumentSnapshot docsnapUsers in snapUsers)
                {


                    User user = docsnapUsers.ConvertTo<User>();
                    user.clinicmobilenumber = clinic.clinicmobilenumber;
                    user.Id = docsnapUsers.Id;
                    
                    //QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                    //DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                    //Patient patient = docsnapPatient.ConvertTo<Patient>();
                    if (docsnapUsers.Exists)
                    {
                        UserList.Add(user);
                    }
                }

            }

            return View(UserList);
            //return View();
        }

        // GET: User/Details/5
        [CheckSessionTimeOut]
        public async Task<ActionResult> Details(string id)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            User user = docSnap.ConvertTo<User>();
            user.Id = id;
            string[] roles = user.user_roles;

            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] == "Admin")
                {
                    ViewData["Admin"] = true;
                }
                if (roles[i] == "Doctor")
                {
                    ViewData["Doctor"] = true;
                }
                if (roles[i] == "Receptionist")
                {
                    ViewData["Receptionist"] = true;
                }
                if (roles[i] == "Chemist")
                {
                    ViewData["Chemist"] = true;
                }
                if (roles[i] == "Cashier")
                {
                    ViewData["Cashier"] = true;
                }

            }
            return View(user);
        }

        // GET: User/Create
        [CheckSessionTimeOut]
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [CheckSessionTimeOut]
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {try
            {
                if (ModelState.IsValid)
                {
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId);
                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                    string selectedPlan = docSnap.GetValue<string>("selected_plan");

                    string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                    Query QrefUsers = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user");
                    QuerySnapshot snapUsers = await QrefUsers.GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsers)
                    {
                        User dbuser = docsnapUsers.ConvertTo<User>();
                        if (dbuser.user_roles.Contains("Receptionist"))
                        {
                            if(user.user_roles[0].ToString().IndexOf("Receptionist") >= 0)
                            {
                                ViewBag.Message = "Only 1 Receptionist is allowed to create.";
                                return View(user);
                            }
                        }
                        if (dbuser.user_roles.Contains("Chemist"))
                        {
                            if (user.user_roles[0].ToString().IndexOf("Chemist") >= 0)
                            {
                                ViewBag.Message = "Only 1 Chemist is allowed to create.";
                                return View(user);
                            }
                        }
                        if (dbuser.user_roles.Contains("Cashier"))
                        {
                            if (user.user_roles[0].ToString().IndexOf("Cashier") >= 0)
                            {
                                ViewBag.Message = "Only 1 Cashier is allowed to create.";
                                return View(user);
                            }
                        }
                    }

                    switch (selectedPlan)
                    {
                        case "plan_silver":
                            if(snapUsers.Count > 1)
                            {
                                ViewBag.Message = "Please upgrade your Plan to create more users.";
                                return View(user);
                            }
                            break;
                        case "plan_gold":
                            if (snapUsers.Count > 2)
                            {
                                ViewBag.Message = "Please upgrade your Plan to create more users.";
                                return View(user);
                            }
                            break;
                        case "plan_platinum":
                            if (snapUsers.Count > 3)
                            {
                                ViewBag.Message = "Please upgrade your Plan to create more users.";
                                return View(user);
                            }
                            break;
                        default:
                            // code block
                            break;
                    }



                    //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
                    CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user");
                    
                    if(user.user_roles[0].ToString() == "")
                    {
                        ViewBag.Message = "Please select at leat one role.";
                        return View(user);
                    }
                    string[] roles = user.user_roles[0].Remove(user.user_roles[0].Length - 1,1).Split(',');
                    
                    Dictionary<string, object> data1 = new Dictionary<string, object>
                    {
                        {"name" ,user.name},
                        {"email" ,user.email},
                        {"idproof" ,user.idproof},
                        {"creation_date" ,DateTime.UtcNow},
                        {"mobile_number" ,user.mobile_number},
                        {"password" ,user.password},
                        {"signature" ,user.signature},
                        {"status_enable",user.status_enable},
                        {"user_qualification" ,user.user_qualification}
                        
                    };
                    data1.Add("user_roles", roles);

                    

                    await col1.Document().SetAsync(data1);

                    

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(user);
                }


            }
            catch (Exception ex)
            {
                return View(user);
            }
        }

        // GET: User/Edit/5
        [CheckSessionTimeOut]
        public async Task<ActionResult> Edit(string id)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            User user = docSnap.ConvertTo<User>();
            user.Id = id;
            string[] roles = user.user_roles;

            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] == "Admin")
                {
                    ViewData["Admin"] = true;
                }
                if (roles[i] == "Doctor")
                {
                    ViewData["Doctor"] = true;
                }
                if (roles[i] == "Receptionist")
                {
                    ViewData["Receptionist"] = true;
                }
                if (roles[i] == "Chemist")
                {
                    ViewData["Chemist"] = true;
                }
                if (roles[i] == "Cashier")
                {
                    ViewData["Cashier"] = true;
                }

            }
            return View(user);

        }

        // POST: User/Edit/5
        [HttpPost]
        [CheckSessionTimeOut]
        public async Task<ActionResult> Edit(string id, User user)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(id);
                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                    Query QrefUsers = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user");
                    QuerySnapshot snapUsers = await QrefUsers.GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnapUsers in snapUsers)
                    {
                        if (docsnapUsers.Id != id)
                        {
                            User dbuser = docsnapUsers.ConvertTo<User>();
                            if (dbuser.user_roles.Contains("Receptionist"))
                            {
                                if (user.user_roles[0].ToString().IndexOf("Receptionist") >= 0)
                                {
                                    ViewBag.Message = "Only 1 Receptionist is allowed to create.";
                                    return View(user);
                                }
                            }
                            if (dbuser.user_roles.Contains("Chemist"))
                            {
                                if (user.user_roles[0].ToString().IndexOf("Chemist") >= 0)
                                {
                                    ViewBag.Message = "Only 1 Chemist is allowed to create.";
                                    return View(user);
                                }
                            }
                            if (dbuser.user_roles.Contains("Cashier"))
                            {
                                if (user.user_roles[0].ToString().IndexOf("Cashier") >= 0)
                                {
                                    ViewBag.Message = "Only 1 Cashier is allowed to create.";
                                    return View(user);
                                }
                            }
                        }

                    }

                    if (user.user_roles[0].ToString() == "" || user.user_roles[0].ToString() == "System.String[]")
                    {
                        ViewBag.Message = "Please select at leat one role.";
                        return View(user);
                    }
                    string[] roles = user.user_roles[0].Remove(user.user_roles[0].Length - 1, 1).Split(',');

                    Dictionary<string, object> data1 = new Dictionary<string, object>
                    {
                        {"name" ,user.name},
                        {"email" ,user.email},
                        {"idproof" ,user.idproof},
                        {"creation_date" ,DateTime.SpecifyKind(user.creation_date, DateTimeKind.Utc)},
                        {"mobile_number" ,user.mobile_number},
                        {"password" ,user.password},
                        {"signature" ,user.signature},
                        {"status_enable",user.status_enable},
                        {"user_qualification" ,user.user_qualification}

                    };
                    data1.Add("user_roles", roles);

                    if(docSnap.Exists)
                    {
                        await docRef.UpdateAsync(data1);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    string[] roles = user.user_roles[0].Remove(user.user_roles[0].Length - 1, 1).Split(',');
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (roles[i] == "Admin")
                        {
                            ViewData["Admin"] = true;
                        }
                        if (roles[i] == "Doctor")
                        {
                            ViewData["Doctor"] = true;
                        }
                        if (roles[i] == "Receptionist")
                        {
                            ViewData["Receptionist"] = true;
                        }
                        if (roles[i] == "Chemist")
                        {
                            ViewData["Chemist"] = true;
                        }
                        if (roles[i] == "Cashier")
                        {
                            ViewData["Cashier"] = true;
                        }

                    }
                    return View(user);
                }

                
            }
            catch (Exception ex)
            {
                string[] roles = user.user_roles[0].Remove(user.user_roles[0].Length - 1, 1).Split(',');
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i] == "Admin")
                    {
                        ViewData["Admin"] = true;
                    }
                    if (roles[i] == "Doctor")
                    {
                        ViewData["Doctor"] = true;
                    }
                    if (roles[i] == "Receptionist")
                    {
                        ViewData["Receptionist"] = true;
                    }
                    if (roles[i] == "Chemist")
                    {
                        ViewData["Chemist"] = true;
                    }
                    if (roles[i] == "Cashier")
                    {
                        ViewData["Cashier"] = true;
                    }

                }
                return View(user);
            }
        }

        // GET: User/Delete/5
        [CheckSessionTimeOut]
        public async Task<ActionResult> Delete(string id)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            User user = docSnap.ConvertTo<User>();
            user.Id = id;
            string[] roles = user.user_roles;

            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] == "Admin")
                {
                    ViewData["Admin"] = true;
                }
                if (roles[i] == "Doctor")
                {
                    ViewData["Doctor"] = true;
                }
                if (roles[i] == "Receptionist")
                {
                    ViewData["Receptionist"] = true;
                }
                if (roles[i] == "Chemist")
                {
                    ViewData["Chemist"] = true;
                }
                if (roles[i] == "Cashier")
                {
                    ViewData["Cashier"] = true;
                }

            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        [CheckSessionTimeOut]
        public ActionResult Delete(string id, User user)
        {
            try
            {
                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
                DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("user").Document(id);
                docRef.DeleteAsync();
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View(user);
            }
        }
    }
}
