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
    [AccessDeniedAuthorize(Roles = "SuperAdmin")]
    public class SuperUserController : Controller
    {
        // GET: SuperUser
        [Authorize]
        public async Task<ActionResult> Index()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            List<SuperUser> SuperUserList = new List<SuperUser>();
            //Query Qref = db.Collection("Students").WhereEqualTo("StudentName","Suvidhi");
            Query Qref = db.Collection("SuperUsers");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnap in snap)
            {
                SuperUser superuser = docsnap.ConvertTo<SuperUser>();
                if (docsnap.Exists)
                {
                    SuperUserList.Add(superuser);
                }
            }

            return View(SuperUserList);
        }

        // GET: SuperUser/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SuperUser/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperUser/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(SuperUser superuser)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"greenpaperdev-firebase-adminsdk-8k2y5-fb46e63414.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path);
            FirestoreDb db = FirestoreDb.Create("greenpaperdev");


            try
            {

                CollectionReference col1 = db.Collection("SuperUsers");
                Dictionary<string, object> data1 = new Dictionary<string, object>
                {
                    {"UserId" ,superuser.UserId },
                    {"UserName" ,superuser.UserName},
                    {"Password",superuser.Password}
                    
                };

                col1.AddAsync(data1);


                // TODO: Add insert logic here
                //var result = await fireBaseClient.Child("Students").PostAsync(std);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SuperUser/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SuperUser/Edit/5
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

        // GET: SuperUser/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SuperUser/Delete/5
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
