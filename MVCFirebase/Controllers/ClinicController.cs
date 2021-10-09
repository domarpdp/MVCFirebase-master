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
    [AccessDeniedAuthorize(Roles = "SuperAdmin,Admin")]
    public class ClinicController : Controller
    {
        // GET: Clinic
        [Authorize]
        public async Task<ActionResult> Index()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            List<Clinic> ClinicList = new List<Clinic>();
            //Query Qref = db.Collection("users").WhereEqualTo("StudentName", "Suvidhi");
            Query Qref = db.Collection("clinics");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnap in snap)
            {
                Clinic clinic = docsnap.ConvertTo<Clinic>();
                if (docsnap.Exists)
                {
                    ClinicList.Add(clinic);
                }
            }

            return View(ClinicList);
        }

        [Authorize]
        // GET: Clinic/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Clinic/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clinic/Create
        [HttpPost]
        [Authorize]
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

        // GET: Clinic/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Clinic/Edit/5
        [HttpPost]
        [Authorize]
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

        // GET: Clinic/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Clinic/Delete/5
        [HttpPost]
        [Authorize]
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
