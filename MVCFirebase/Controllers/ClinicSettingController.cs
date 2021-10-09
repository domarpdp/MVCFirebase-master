using Google.Cloud.Firestore;
using MVCFirebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCFirebase.Controllers
{
    public class ClinicSettingController : Controller
    {
        // GET: ClinicSetting
        public async Task<ActionResult> Index()
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            List<ClinicSettings> clinicSettingsList = new List<ClinicSettings>();


            //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
            Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
            QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnapClinics in snapClinics)
            {
                Clinic clinic = docsnapClinics.ConvertTo<Clinic>();
                QuerySnapshot snapSettings = await docsnapClinics.Reference.Collection("settings").GetSnapshotAsync();

                foreach (DocumentSnapshot docsnapSettings in snapSettings)
                {


                    //ClinicSettings settings = docsnapSettings.ConvertTo<ClinicSettings>();
                    
                    if (docsnapSettings.Exists)
                    {
                        ClinicSettings settings = new ClinicSettings();
                        settings.bill_sms = docsnapSettings.GetValue<bool>("bill_sms");
                        try 
                        { 
                            settings.inventoryon = docsnapSettings.GetValue<bool>("inventoryon"); 
                        }
                        catch 
                        {
                            settings.inventoryon = false;
                        }
                        settings.reminder_sms = docsnapSettings.GetValue<bool>("reminder_sms");
                        settings.fee1 = docsnapSettings.GetValue<int>("fee1");
                        settings.fee2 = docsnapSettings.GetValue<int>("fee2");
                        settings.fee3 = docsnapSettings.GetValue<int>("fee3");
                        settings.days1 = docsnapSettings.GetValue<int>("days1");
                        settings.days2 = docsnapSettings.GetValue<int>("days2");
                        settings.days3 = docsnapSettings.GetValue<int>("days3");
                        settings.whofirst = docsnapSettings.GetValue<string>("whofirst");
                        settings.consultationfee = docsnapSettings.GetValue<bool>("consultationfee");
                        settings.id = docsnapSettings.Id;
                        clinicSettingsList.Add(settings);
                    }
                }

            }

            return View(clinicSettingsList);
            //return View();
        }

        // GET: ClinicSetting/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClinicSetting/Create
        public ActionResult Create()
        {
            List<SelectListItem> whofirst = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Chemist", Value = "Chemist"
                },
                new SelectListItem {
                    Text = "Cashier", Value = "Cashier"
                },
                

            };
            ViewBag.WHOFIRSTS = whofirst;

            return View();
        }

        // POST: ClinicSetting/Create
        [HttpPost]
        public async Task<ActionResult> Create(ClinicSettings settings)
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            try
            {
                List<SelectListItem> whofirst = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Chemist", Value = "Chemist"
                    },
                    new SelectListItem {
                        Text = "Cashier", Value = "Cashier"
                    },
                };
                ViewBag.WHOFIRSTS = whofirst;

                if (ModelState.IsValid)
                {


                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                    Query QrefSettings = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("settings");
                    QuerySnapshot snapSettings = await QrefSettings.GetSnapshotAsync();

                    if (snapSettings.Count == 0)
                    {
                        CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("settings");
                        Dictionary<string, object> data1 = new Dictionary<string, object>
                            {
                                {"bill_sms" ,settings.bill_sms},
                                {"reminder_sms" ,settings.reminder_sms},
                                {"fee1" ,settings.fee1},
                                {"fee2" ,settings.fee2},
                                {"fee3" ,settings.fee3},
                                {"inventoryon" ,settings.inventoryon},
                                {"days1" ,settings.days1},
                                {"days2" ,settings.days2},
                                {"days3" ,settings.days3},
                                {"whofirst" ,settings.whofirst},
                                {"consultationfee" ,settings.consultationfee}
                            };

                        await col1.Document().SetAsync(data1);
                    }
                    else
                    {
                        ViewBag.Message = "Settings already exists for Clinic.";
                        return View(settings);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(settings);
                }
            }
            catch
            {
                return View(settings);
            }
        }

        // GET: ClinicSetting/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            List<SelectListItem> whofirst = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Chemist", Value = "Chemist"
                    },
                    new SelectListItem {
                        Text = "Cashier", Value = "Cashier"
                    },
                };
            ViewBag.WHOFIRSTS = whofirst;

            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                        
            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("settings").Document(id);
            DocumentSnapshot docsnapSettings = await docRef.GetSnapshotAsync();
            ClinicSettings settings = new ClinicSettings();
            settings.bill_sms = docsnapSettings.GetValue<bool>("bill_sms");
            settings.reminder_sms = docsnapSettings.GetValue<bool>("reminder_sms");
            try
            {
                settings.inventoryon = docsnapSettings.GetValue<bool>("inventoryon");
            }
            catch
            {
                settings.inventoryon = false;
            }
            
            settings.fee1 = docsnapSettings.GetValue<int>("fee1");
            settings.fee2 = docsnapSettings.GetValue<int>("fee2");
            settings.fee3 = docsnapSettings.GetValue<int>("fee3");
            settings.days1 = docsnapSettings.GetValue<int>("days1");
            settings.days2 = docsnapSettings.GetValue<int>("days2");
            settings.days3 = docsnapSettings.GetValue<int>("days3");
            settings.whofirst = docsnapSettings.GetValue<string>("whofirst");
            settings.consultationfee = docsnapSettings.GetValue<bool>("consultationfee");
            settings.id = docsnapSettings.Id;

            

            return View(settings);
        }

        // POST: ClinicSetting/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ClinicSettings settings)
        {
            List<SelectListItem> whofirst = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Chemist", Value = "Chemist"
                    },
                    new SelectListItem {
                        Text = "Cashier", Value = "Cashier"
                    },
                };
            ViewBag.WHOFIRSTS = whofirst;
            try
            {
                if (ModelState.IsValid)
                {
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                    DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("settings").Document(id);
                    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();



                    Dictionary<string, object> data1 = new Dictionary<string, object>
                            {
                                {"bill_sms" ,settings.bill_sms},
                                {"reminder_sms" ,settings.reminder_sms},
                                {"fee1" ,settings.fee1},
                                {"fee2" ,settings.fee2},
                                {"fee3" ,settings.fee3},
                                {"inventoryon" ,settings.inventoryon},
                                {"days1" ,settings.days1},
                                {"days2" ,settings.days2},
                                {"days3" ,settings.days3},
                                {"whofirst" ,settings.whofirst},
                                {"consultationfee" ,settings.consultationfee}
                            };


                    if (docSnap.Exists)
                    {
                        await docRef.UpdateAsync(data1);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(settings);
                }
                // TODO: Add update logic here

                
            }
            catch
            {
                return View(settings);
            }
        }

        // GET: ClinicSetting/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClinicSetting/Delete/5
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
    }
}
