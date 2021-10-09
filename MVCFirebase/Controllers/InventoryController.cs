using Google.Cloud.Firestore;
using MVCFirebase.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCFirebase.Controllers
{
    
    public class InventoryController : Controller
    {
        // GET: Inventory
        
        public async Task<ActionResult> Index()
        {
            string ClinicMobileNumber = GlobalSessionVariables.ClinicMobileNumber;
           
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");
            List<Inventory> InventoryList = new List<Inventory>();

            Query Qref = db.Collection("clinics").WhereEqualTo("clinicmobilenumber", ClinicMobileNumber);
            QuerySnapshot snapClinics = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnapClinics in snapClinics)
            {
                Clinic clinic = docsnapClinics.ConvertTo<Clinic>();
                QuerySnapshot snapMedicines = await docsnapClinics.Reference.Collection("inventory").OrderByDescending("medicinename").GetSnapshotAsync();

                foreach (DocumentSnapshot docsnapMedicines in snapMedicines)
                {


                    //Inventory inventory = docsnapMedicines.ConvertTo<Inventory>();
                    Inventory inventory = new Inventory();
                    inventory.id = docsnapMedicines.Id;
                    
                    inventory.shortname = docsnapMedicines.GetValue<string>("shortname");
                    inventory.quantitypurchased = docsnapMedicines.GetValue<int>("quantitypurchased");
                    inventory.quantitygiven = docsnapMedicines.GetValue<int>("quantitygiven");
                    inventory.quantitybalance = docsnapMedicines.GetValue<int>("quantitybalance");
                    inventory.quantitymin = docsnapMedicines.GetValue<int>("quantitymin");
                    inventory.medicinename = docsnapMedicines.GetValue<string>("medicinename");
                    inventory.unitmrp = docsnapMedicines.GetValue<string>("unitmrp");
                    inventory.purchasedunitprice = docsnapMedicines.GetValue<string>("purchasedunitprice");
                    inventory.expirydate = docsnapMedicines.GetValue<Timestamp>("expirydate").ToDateTime().ToString("MM/dd/yyyy");
                    inventory.dateadded = docsnapMedicines.GetValue<Timestamp>("dateadded").ToDateTime().ToString("MM/dd/yyyy");
                    inventory.vendorname = docsnapMedicines.GetValue<string>("vendorname");
                    inventory.vendormobilenumber = docsnapMedicines.GetValue<string>("vendormobilenumber");
                    //QuerySnapshot snapPatient = await docsnapClinics.Reference.Collection("patientList").WhereEqualTo("patient_id", appointment.patient_id).Limit(1).GetSnapshotAsync();
                    //DocumentSnapshot docsnapPatient = snapPatient.Documents[0];

                    //Patient patient = docsnapPatient.ConvertTo<Patient>();
                    if (docsnapMedicines.Exists)
                    {
                        InventoryList.Add(inventory);
                    }
                }

            }

            return View(InventoryList);
        }

        // GET: Inventory/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inventory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        public async Task<ActionResult> Create(Inventory inventory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                    FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                    //CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("patientList").Document("test");
                    CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("inventory");


                    Dictionary<string, object> data1 = new Dictionary<string, object>
                    {
                        {"shortname" ,inventory.shortname.ToLower()},
                        {"quantitypurchased" ,inventory.quantitypurchased},
                        {"quantitymin" ,inventory.quantitymin},
                        {"medicinename" ,inventory.medicinename.ToLower()},
                        {"unitmrp" ,inventory.unitmrp.ToString()},
                        {"dateadded" ,DateTime.UtcNow},
                        {"expirydate" ,DateTime.SpecifyKind(Convert.ToDateTime(inventory.expirydate), DateTimeKind.Utc)},
                        {"purchasedunitprice" ,inventory.purchasedunitprice.ToString()},
                        {"vendorname",inventory.vendorname.ToLower()},
                        {"vendormobilenumber" ,inventory.vendormobilenumber},
                        {"quantitygiven" ,0},
                        {"quantitybalance" ,inventory.quantitypurchased}
                    };

                    await col1.Document().SetAsync(data1);



                    return RedirectToAction("Index");
                }
                else
                {
                    return View(inventory);
                }
                // TODO: Add insert logic here

                
            }
            catch (Exception ex)
            {
                return View(inventory);
            }
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Inventory/Edit/5
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

        // GET: Inventory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Inventory/Delete/5
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

        public ActionResult AddMedicine()
        {
            List<Medicine> medicine = new List<Medicine>();

            TempData["medicine"] = medicine;
            TempData.Keep();
            return View();

            //DataTable dt = new DataTable();
            //dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id"), new DataColumn("Name"), new DataColumn("Country") });
            //dt.Rows.Add(1, "John Hammond", "United States");
            //dt.Rows.Add(2, "Mudassar Khan", "India");
            //dt.Rows.Add(3, "Suzanne Mathews", "France");
            //dt.Rows.Add(4, "Robert Schidner", "Russia");
            //TempData["abc"] = dt.AsEnumerable();
            
            //ViewData.Model = dt.AsEnumerable();
            //TempData.Keep();
            //return View();
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        public async Task<ActionResult> AddMedicine(FormCollection collection)
        {
            try
            {
                string appautoid = collection["appointmentAutoId"];
                string patientautoid = collection["patientAutoId"];
                string inventoryautoid = collection["hfinventoryid"].Split('-')[0];
                string unitmrp = collection["hfinventoryid"].Split('-')[1];
                string quantitybalance = collection["hfquantitybalance"].Split('-')[2];
                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");


                //CollectionReference col1 = db.Collection("clinics").Document("ly0N6C9cO0crz0s6LMUi").Collection("appointments").Document(appautoid).Collection("medicines");
                CollectionReference col1 = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appautoid).Collection("medicines");

                Dictionary<string, object> data1 = new Dictionary<string, object>
                    {
                        {"medicinename" ,collection["Medicine"]},
                        {"quantity" ,collection["Quantity"]},
                        {"inventoryid" ,inventoryautoid},
                        {"unitmrp" ,unitmrp}
                    };
                
                await col1.Document().SetAsync(data1);


                DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("inventory").Document(inventoryautoid);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

                int updatedQuantityBalance = Convert.ToInt32(quantitybalance) - Convert.ToInt32(collection["Quantity"]);

                Dictionary<string, object> data2 = new Dictionary<string, object>
                            {
                                {"quantitybalance" ,updatedQuantityBalance}
                            };


                if (docSnap.Exists)
                {
                    await docRef.UpdateAsync(data2);
                }

                return RedirectToAction("Index","Image",new { id = appautoid , patient= patientautoid });
                //int serialnoCount = 0;
                //List<Medicine> medicine = new List<Medicine>();
                //medicine = TempData["medicine"] as List<Medicine>;
                //medicine = medicine.OrderByDescending(a => a.serialno).ToList();

                //if (medicine.Count > 0)
                //{
                //    serialnoCount = medicine.FirstOrDefault().serialno;
                //}
                //Medicine med = new Medicine();
                //med.serialno = serialnoCount + 1;
                //med.medicinename = collection["Medicine"];
                //med.Quantity = collection["Quantity"];

                //medicine.Add(med);

                //TempData["medicine"] = medicine;
                ////string blah = myDataSet.Tables[0].Rows[0]["Name"].ToString();

                //// TODO: Add delete logic here
                //TempData.Keep();
                //return RedirectToAction("/Image/Index");
            }
            catch
            {
                return RedirectToAction("Index", "Image");
            }
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteMedicine(string id,string appointmentid,string patientid,string quantity,string inventoryid)
        {
            try
            {
                string appautoid = appointmentid;
                string patientautoid = patientid;
                string medicineId = id;
                string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
                FirestoreDb db = FirestoreDb.Create("greenpaperdev");

                //CollectionReference col1 = db.Collection("clinics").Document("ly0N6C9cO0crz0s6LMUi").Collection("appointments").Document(appautoid).Collection("medicines");
                DocumentReference docRef = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("appointments").Document(appautoid).Collection("medicines").Document(medicineId);
                await docRef.DeleteAsync();

                DocumentReference docRefInventory = db.Collection("clinics").Document(GlobalSessionVariables.ClinicDocumentAutoId).Collection("inventory").Document(inventoryid);
                DocumentSnapshot docSnapInventory = await docRefInventory.GetSnapshotAsync();

                int updatedQuantityBalance = docSnapInventory.GetValue<int>("quantitybalance") + Convert.ToInt32(quantity);

                Dictionary<string, object> data2 = new Dictionary<string, object>
                            {
                                {"quantitybalance" ,updatedQuantityBalance}
                            };


                if (docSnapInventory.Exists)
                {
                    await docRefInventory.UpdateAsync(data2);
                }

                return RedirectToAction("Index", "Image", new { id = appautoid, patient = patientautoid });
                
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
