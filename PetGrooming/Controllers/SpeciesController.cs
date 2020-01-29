using System;
using System.Collections.Generic;
using System.Data;
//required for SqlParameter class
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetGrooming.Data;
using PetGrooming.Models;
using System.Diagnostics;

namespace PetGrooming.Controllers
{
    public class SpeciesController : Controller
    {
        // database connection will be assign to db
        private PetGroomingContext db = new PetGroomingContext();
        // GET: Species
        public ActionResult Index()
        {
            return View();
        }

        //TODO: Each line should be a separate method in this class
        // List
        //Add Pagination on the List 
        public ActionResult List(string search)
        {
            //create a variable assigning to the database species
            var species = from s in db.Species select s;
            //if search value has a value
            if (!String.IsNullOrEmpty(search))
            {
                species = species.Where(s => s.Name.Contains(search));
            }


            return View(species.ToList());
        }

       
        //This contoller is not neccessary cause species table has only name on it does it can be shown clearly on the LIST controller
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // pet pet = db.pets.find(id); //ef 6 technique
            Species species = db.Species.SqlQuery("select * from species where speciesid=@speciesid", new SqlParameter("@speciesid", id)).FirstOrDefault();
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }

        // TODO: Add Validation
        [HttpPost]
        public ActionResult Add(string SpeciesName)
        {

            string query = "insert into species (Name) values (@Name)";
            SqlParameter sqlparams = new SqlParameter("@Name", SpeciesName);

            db.Database.ExecuteSqlCommand(query, sqlparams);


            //run the list method to return to a list of pets so we can see our new one!
            return RedirectToAction("List");
        }

        public ActionResult Add()
        {

            return View();
        }

        public ActionResult Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //need information about a particular pet
            Species selectedspecies = db.Species.SqlQuery("select * from species where SpeciesID = @id", new SqlParameter("@id", id)).FirstOrDefault();
            if (selectedspecies == null)
            {
                return HttpNotFound();
            }
            return View(selectedspecies);
        }

        // TODO: Add Validation
        [HttpPost]
        public ActionResult Update(string SpeciesName, int id)
        {

            string query = "update species set Name = @Name where SpeciesID = @id";
            SqlParameter[] sqlparams = new SqlParameter[2]; 
            sqlparams[0] = new SqlParameter("@Name", SpeciesName);
            sqlparams[1] = new SqlParameter("@id", id);


            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");

        }


        //TODO:
        //Update
        //[HttpPost] Update
        //[HttpPost] Delete
        //(optional) Delete


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //need information about a particular species
            Species selectedspecies = db.Species.SqlQuery("select * from species where SpeciesID = @id", new SqlParameter("@id", id)).FirstOrDefault();
            if (selectedspecies == null)
            {
                return HttpNotFound();
            }
            return View(selectedspecies);
        }

        // TODO: Add Validation
        [HttpPost]
        [ActionName("Delete")]
        //This ActionNAme refer to the ActionResult Delete, you cannot have 2 action with similar name in MVC asp.net, but having different parameter will work or by doing similar with this.
        public ActionResult Deletemethod(int id)
        {

            string query = "delete from species where SpeciesID = @id";
            SqlParameter sqlparams = new SqlParameter("@id", id); 

            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

    }
}