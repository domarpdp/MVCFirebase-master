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
    public class ImageController : Controller
    {
        // GET: Image
        public async Task<ActionResult> Index(string id,string patient)
        {
            ImageModel _objuserloginmodel = new ImageModel();
            ViewBag.SelectedId = 0;
            TempData["SelectedId"] = 0;
            TempData["appointmentAutoId"] = id;
            TempData["patientAutoId"] = patient;

            List<ImageViewModel> ImageList = new List<ImageViewModel>();
            //string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            //string ClinicMobileNumber = "9811035028";
            
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            int i = 1;


            Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patient).Collection("prescriptions").OrderByDescending("timeStamp");
            

            QuerySnapshot snapPres = await QrefPrescriptions.GetSnapshotAsync();
            if(snapPres.Count > 0)
            {
                foreach (DocumentSnapshot docsnapPres in snapPres)
                {

                    if (docsnapPres.Exists)
                    {
                        ImageViewModel img = new ImageViewModel();
                        img.Id = i;
                        img.ImageUrl = "data:image/png;base64," + docsnapPres.GetValue<string>("file");
                        ImageList.Add(img);
                        i++;
                    }

                }
                _objuserloginmodel.SelectedImage = ImageList[0];
            }

            //_objuserloginmodel.SelectedImage = _objuserloginmodel.GetList()[0];
            TempData["CurrentSelectedId"] = snapPres.Count;
            TempData["TotalPrescriptions"] = snapPres.Count;
            decimal totalprice = 0;

            List<Medicine> medicineList = new List<Medicine>();
            Query QrefMedicines = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(id).Collection("medicines");
            QuerySnapshot snapMedicines = await QrefMedicines.GetSnapshotAsync();

            if (snapMedicines.Count > 0)
            {
                foreach (DocumentSnapshot docsnapMedicines in snapMedicines)
                {

                    if (docsnapMedicines.Exists)
                    {
                        Medicine med = new Medicine();
                        med.id = docsnapMedicines.Id;
                        med.medicinename = docsnapMedicines.GetValue<string>("medicinename");
                        med.quantity = docsnapMedicines.GetValue<string>("quantity");
                        med.Price = (Convert.ToDecimal(docsnapMedicines.GetValue<string>("quantity")) * Convert.ToDecimal(docsnapMedicines.GetValue<string>("unitmrp"))).ToString();
                        med.inventoryid = docsnapMedicines.GetValue<string>("inventoryid");
                        medicineList.Add(med);
                        totalprice = totalprice + Convert.ToDecimal(docsnapMedicines.GetValue<string>("quantity")) * Convert.ToDecimal(docsnapMedicines.GetValue<string>("unitmrp"));
                    }
                }
            }


            TempData["medicine"] = medicineList;
            
            
            QuerySnapshot snapSettings = await db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("settings").Limit(1).GetSnapshotAsync();
            if (snapSettings.Count > 0)
            {
                DocumentSnapshot docSnapSettings = snapSettings.Documents[0];

                if (docSnapSettings.Exists)
                {
                    try {
                        if (docSnapSettings.GetValue<Boolean>("inventoryon"))
                        {
                            TempData["inventoryon"] = "true";
                        }
                        else
                        {
                            TempData["inventoryon"] = "false";
                        }
                    }
                    catch 
                    {
                        TempData["inventoryon"] = "false";
                    }
                    
                    
                }
                else
                {
                    TempData["inventoryon"] = "false";
                }
            }

            DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

            totalprice = totalprice + Convert.ToInt32(docSnap.GetValue<string>("fee"));

            if (totalprice == 0)
            {
                TempData["TotalPrice"] = 0;
            }
            else
            {
                TempData["TotalPrice"] = totalprice;
            }

            TempData["days"] = docSnap.GetValue<string>("days");
            TempData["fee"] = docSnap.GetValue<string>("fee");


            if (docSnap.GetValue<Timestamp>("raisedDate").ToDateTime().Date < DateTime.Now.Date)
            {
                ViewData["DateType"] = "OldDate";
            }
            else
            {
                ViewData["DateType"] = "CurrentDate";
            }

            try {
                if (docSnap.GetValue<string>("statusChemist") == "Completed")
                {
                    ViewData["FromPage"] = "Completed";
                }
                else
                {
                    ViewData["FromPage"] = "Waiting";
                }
            }
            catch 
            {
                ViewData["FromPage"] = "Waiting";
            }
            

            

            TempData.Keep();


            return View(_objuserloginmodel);

        }

        [HttpPost]
        public async Task<ActionResult> GetNextOrPrevImage(ImageViewModel SelectedImage, string ButtonType)
        {
            ImageModel _objuserloginmodel = new ImageModel();
            string patientAutoId = TempData["patientAutoId"].ToString();
            //List<ImageViewModel> GetList = _objuserloginmodel.GetList();

            List<ImageViewModel> GetList = new List<ImageViewModel>();

            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            int i = 1;
            

            Query QrefPrescriptions = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document(patientAutoId).Collection("prescriptions").OrderByDescending("timeStamp");
            QuerySnapshot snapPres = await QrefPrescriptions.GetSnapshotAsync();
            if (snapPres.Count > 0)
            {
                foreach (DocumentSnapshot docsnapPres in snapPres)
                {
                    if (docsnapPres.Exists)
                    {
                        ImageViewModel img = new ImageViewModel();
                        img.Id = i;
                        img.ImageUrl = "data:image/png;base64," + docsnapPres.GetValue<string>("file");
                        GetList.Add(img);
                        i++;
                    }
                   
                    

                }
            }

            int id = System.Convert.ToInt32(TempData["SelectedId"]);
            

            if (ButtonType.Trim() == "<")

                _objuserloginmodel.SelectedImage = GetList[++id < GetList.Count ? id : --id];
            else if (ButtonType.Trim() == ">")
                _objuserloginmodel.SelectedImage = GetList[--id > -1 ? id : ++id];

            TempData["SelectedId"] = id;
            TempData["CurrentSelectedId"] = snapPres.Count - id;
            TempData["patientAutoId"] = patientAutoId;
            TempData["TotalPrescriptions"] = snapPres.Count;
            
 

            return PartialView("_PartialImage", _objuserloginmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Submit()
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
                            {"completiondateChemist" ,DateTime.UtcNow},
                            {"statusChemist" ,"Completed"}

                        };

                    //await docRef.UpdateAsync( "completionMedicine",DateTime.UtcNow);
                    await docRef.UpdateAsync(data1);
                }
                // TODO: Add delete logic here

                return RedirectToAction("Index","Appointment");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> MedicineDetail(string query)
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            List<Inventory> medicineList = new List<Inventory>();


            Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
            QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnapClinics in snapClinics)
            {
                Clinic clinic = docsnapClinics.ConvertTo<Clinic>();
                QuerySnapshot snapMedicines = await docsnapClinics.Reference.Collection("inventory").WhereEqualTo("medicinename",query).GetSnapshotAsync();
                if(snapMedicines.Count > 0)
                {
                    foreach (DocumentSnapshot docsnapMedicines in snapMedicines)
                    {
                        Inventory inventory = new Inventory();
                        inventory.id = docsnapMedicines.Id;
                        inventory.medicinename = docsnapMedicines.GetValue<string>("medicinename");
                        inventory.shortname = docsnapMedicines.GetValue<string>("shortname");
                        inventory.quantitybalance = docsnapMedicines.GetValue<int>("quantitybalance");
                        inventory.expirydate = docsnapMedicines.GetValue<string>("expirydate");
                        medicineList.Add(inventory);
                    }
                }
            }
            return View(medicineList);
        }


        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix)//I think that the id that you are passing here needs to be the search term. You may not have to change anything here, but you do in the $.ajax() call
        {
            List<autocomplete> autocompleteList = new List<autocomplete>();
            try
            {//prefix = Request.QueryString["term"];

                string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");
                

                string searchInputToLower = prefix.ToLower();

                Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
                QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();

                foreach (DocumentSnapshot docsnapClinics in snapClinics)
                {
                    Clinic clinic = docsnapClinics.ConvertTo<Clinic>();
                    QuerySnapshot snapMedicines = await docsnapClinics.Reference.Collection("inventory").OrderBy("medicinename").StartAt(searchInputToLower).EndAt(searchInputToLower + "\uf8ff").GetSnapshotAsync();
                    if (snapMedicines.Count > 0)
                    {
                        foreach (DocumentSnapshot docsnapMedicines in snapMedicines)
                        {
                            if(Convert.ToInt32(docsnapMedicines.GetValue<int>("quantitybalance")) > 0)
                            { 
                                autocomplete inventory = new autocomplete();
                                inventory.val = docsnapMedicines.Id + "-" + docsnapMedicines.GetValue<string>("unitmrp");
                                inventory.label = docsnapMedicines.GetValue<string>("shortname") + "-"+ docsnapMedicines.GetValue<Timestamp>("expirydate").ToDateTime().ToString("MM/dd/yyyy") + "-" + docsnapMedicines.GetValue<int>("quantitybalance");
                                //inventory.shortname = docsnapMedicines.GetValue<string>("shortname");
                                //inventory.quantitybalance = docsnapMedicines.GetValue<int>("quantitybalance");
                                //inventory.expirydate = docsnapMedicines.GetValue<Timestamp>("expirydate").ToDateTime().ToString("MM/dd/yyyy");
                                autocompleteList.Add(inventory);
                            }
                        }
                    }
                }

                return Json(autocompleteList);
            
                
            }
            catch (Exception ex)
            {
                return Json(autocompleteList);
            }
            
        }

        // GET: Appointment/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Appointment/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, string appointmentid)
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