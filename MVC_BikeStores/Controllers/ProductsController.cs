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
    public class ProductsController : Controller
    {
        private BikeStoresContext db = new BikeStoresContext();

        // GET: Products
        [AllowAnonymous]
        public ActionResult Index(string cercaQuantita)
        {
            var quantita = new List<string> { "Esaurito", "Scorta" };
            ViewBag.cercaQuantita = new SelectList(quantita);

            var products = db.Products.Take(50).ToList();

            if (!string.IsNullOrEmpty(cercaQuantita))
            {
                if (cercaQuantita == "Esaurito")
                {
                    products = db.Stocks.Include(x => x.Product_Id)
                        .Where(x => x.Quantity == 0).Select(x => x.Product).ToList();
                }else if (cercaQuantita == "Scorta")
                {
                    products = db.Stocks.Include(x => x.Product_Id)
                        .Where(x => x.Quantity >= 1 && x.Quantity <= 9).Select(x => x.Product).ToList();
                }
            }
            return View(products);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }
        [AllowAnonymous]
        public ActionResult ProductsListByBrand(int? id)
        {
            var quantita = new List<string> { "Esaurito", "Scorta" };
            ViewBag.cercaQuantita = new SelectList(quantita);
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Where(x => x.Brand_Id == id).ToList();

            if (product == null)
                return HttpNotFound();

            return View("Index", product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Brand_Name");
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Name");
            return View();
        }

        // POST: Products/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_Id,Product_Name,Brand_Id,Category_Id,Model_Year,List_Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Brand_Name", product.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_Id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Brand_Name", product.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_Id);
            return View(product);
        }

        // POST: Products/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_Id,Product_Name,Brand_Id,Category_Id,Model_Year,List_Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Brand_Name", product.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_Id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
