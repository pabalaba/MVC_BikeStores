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
    [Authorize]
    public class OrdersController : Controller
    {
        private BikeStoresContext db = new BikeStoresContext();

        // GET: Orders
        [Authorize(Roles = "Administrators,Employees")]
        public ActionResult Index(string cercaStato)
        {
            var stati = db.Order_Status.Select(x => x.Status_Name).Distinct().ToList();
            ViewBag.cercaStato = new SelectList(stati);

            var orders = db.Orders.Include(o => o.Customer)
                .Include(o => o.Order_Status)
                .Include(o => o.Staff)
                .Include(o => o.Store)
                .OrderByDescending(x => x.Order_Date)
                .ToList();

            if (!string.IsNullOrEmpty(cercaStato))
                orders = db.Orders.Include(o => o.Customer)
                .Include(o => o.Order_Status)
                .Include(o => o.Staff)
                .Include(o => o.Store)
                .Where(x => x.Order_Status.Status_Name.Equals(cercaStato))
                .OrderByDescending(x => x.Order_Date)
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Employees")]
        public ActionResult IndexEmployees(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var staff = db.Staffs.FirstOrDefault(s => s.Email == email);

            var orders = db.Orders.Include(o => o.Customer)
                .Include(o => o.Order_Status)
                .Include(o => o.Staff)
                .Include(o => o.Store)
                .Where(o => o.Staff_Id == staff.Staff_Id)
                .OrderByDescending(o => o.Order_Date)
                .ToList();
            return View("Index", orders);
        }

        [Authorize(Roles = "Customers")]
        public ActionResult IndexCustomers(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);

            var orders = db.Orders.Include(o => o.Customer)
                .Include(o => o.Order_Status)
                .Include(o => o.Staff)
                .Include(o => o.Store)
                .Where(o => o.Customer_Id == customer.Customer_Id)
                .OrderByDescending(o => o.Order_Date)
                .ToList();
            return View("Index", orders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders
                .Include(x=>x.Customer)
                .Include(x=>x.Staff)
                .Include(x=>x.Order_Items)
                .FirstOrDefault(x => x.Order_Id == id);
            if(order.Customer.Email != User.Identity.Name && User.IsInRole("Customers"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "Administrators,Employees")]
        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.Customer_Id = new SelectList(db.Customers, "Customer_Id", "First_Name");
            ViewBag.Order_Status_Id = new SelectList(db.Order_Status, "Status_Id", "Status_Name");
            ViewBag.Staff_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name");
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name");
            return View();
        }

        // POST: Orders/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Employees")]
        public ActionResult Create([Bind(Include = "Order_Id,Customer_Id,Order_Status_Id,Order_Date,Required_Date,Shipped_Date,Store_Id,Staff_Id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Customer_Id = new SelectList(db.Customers, "Customer_Id", "First_Name", order.Customer_Id);
            ViewBag.Order_Status_Id = new SelectList(db.Order_Status, "Status_Id", "Status_Name", order.Order_Status_Id);
            ViewBag.Staff_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", order.Staff_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", order.Store_Id);
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Administrators,Employees,Customers")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Customer_Id = new SelectList(db.Customers, "Customer_Id", "First_Name", order.Customer_Id);
            ViewBag.Order_Status_Id = new SelectList(db.Order_Status, "Status_Id", "Status_Name", order.Order_Status_Id);
            ViewBag.Staff_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", order.Staff_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", order.Store_Id);
            return View(order);
        }

        // POST: Orders/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Employees,Customers")]
        public ActionResult Edit([Bind(Include = "Order_Id,Customer_Id,Order_Status_Id,Order_Date,Required_Date,Shipped_Date,Store_Id,Staff_Id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Customer_Id = new SelectList(db.Customers, "Customer_Id", "First_Name", order.Customer_Id);
            ViewBag.Order_Status_Id = new SelectList(db.Order_Status, "Status_Id", "Status_Name", order.Order_Status_Id);
            ViewBag.Staff_Id = new SelectList(db.Staffs, "Staff_Id", "First_Name", order.Staff_Id);
            ViewBag.Store_Id = new SelectList(db.Stores, "Store_Id", "Store_Name", order.Store_Id);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Administrators,Employees")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Employees")]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
