using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PublicLibrary.Domain;
using PublicLibrary.Web.Models;

namespace PublicLibrary.Web.Controllers
{
    public class AuthorsController : Controller
    {
        static string MyConnectoinString = System.Configuration.ConfigurationManager.ConnectionStrings["PublicLibraryWebContext"].ConnectionString;

        private PublicLibraryWebContext db = new PublicLibraryWebContext();
        private AuthorRepository db1 = new AuthorRepository(MyConnectoinString);

        // GET: Authors
        public ActionResult Index()
        {
            //return View(db.Authors.ToList());
            return View(db1.GetAuthors());
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db1.GetAuthor(id.GetValueOrDefault());//db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DeathDate,Age,Sex,FirstName,LastName,BornDate")] Author author)
        {
            if (ModelState.IsValid)
            {
                db1.AddAuthor(author);
                //db.Authors.Add(author);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db1.GetAuthor(id.GetValueOrDefault());//db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DeathDate,Age,Sex,FirstName,LastName,BornDate")] Author author)
        {
            if (ModelState.IsValid)
            {
                db1.EditAuthor(author);
                //db.Entry(author).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db1.GetAuthor(id.GetValueOrDefault());//db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db1.DeleteAuthor(id);

            //Author author = db.Authors.Find(id);
            //db.Authors.Remove(author);
            //db.SaveChanges();
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
