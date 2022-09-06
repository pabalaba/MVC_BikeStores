using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_BikeStores.Models;

namespace MVC_BikeStores.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class StaffsController : Controller
    {
        private BikeStoresContext db = new BikeStoresContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var list = db.Staffs.OrderBy(x => x.Last_Name).ThenBy(x => x.First_Name).ToList();

            return View(list);
        }
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var staff = db.Staffs.Find(id);

            if (staff == null)
                return HttpNotFound();

            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            ViewBag.Manager_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name");
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name");
            return View();
        }

        // POST: Staffs/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Staff_Id,First_Name,Last_Name,Email,Phone,Active,Store_Id,Manager_Id")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Manager_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", staff.Manager_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", staff.Store_Id);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.Manager_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", staff.Manager_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", staff.Store_Id);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Staff_Id,First_Name,Last_Name,Email,Phone,Active,Store_Id,Manager_Id")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Manager_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", staff.Manager_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", staff.Store_Id);
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
