using PetGrooming.Data;
using PetGrooming.Models;
using System;
using System.Collections.Generic;
//required for SqlParameter class
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace PetGrooming.Controllers
{
    public class PetController : Controller
    {
        /*
        These reading resources will help you understand and navigate the MVC environment
 
        Q: What is an MVC controller?

        - https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/aspnet-mvc-controllers-overview-cs

        Q: What does it mean to "Pass Data" from the Controller to the View?

        - http://www.webdevelopmenthelp.net/2014/06/using-model-pass-data-asp-net-mvc.html

        Q: What is an SQL injection attack?

        - https://www.w3schools.com/sql/sql_injection.asp

        Q: How can we prevent SQL injection attacks?

        - https://www.completecsharptutorial.com/ado-net/insert-records-using-simple-and-parameterized-query-c-sql.php

        Q: How can I run an SQL query against a database inside a controller file?

        - https://www.entityframeworktutorial.net/EntityFramework4.3/raw-sql-query-in-entity-framework.aspx
 
         */
        private PetGroomingContext db = new PetGroomingContext();

        //public ActionResult Index(string sortOrder, string searchString)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
        //    var pets = from s in db.Pets
        //               select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        pets = pets.Where(s => s.PetName.Contains(searchString)
        //                               || s.Species.Name.Contains(searchString));
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            pets = pets.OrderByDescending(s => s.PetName);
        //            break;
        //        case "Date":
        //            pets = pets.OrderBy(s => s.Weight);
        //            break;
        //        case "date_desc":
        //            pets = pets.OrderByDescending(s => s.Species);
        //            break;
        //        default:
        //            pets = pets.OrderBy(s => s.PetName);
        //            break;
        //    }
        //    return View(pets.ToList());
        //}


        // GET: Pet
        //TODO: Add Pagination on the list page      
        //[Route("Pet/List/{page}")] // using custom route
        public ActionResult List(string search)
        {
            //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            //How could we modify this to include a search bar?
            //var pets = db.Pets.SqlQuery("Select * from Pets");
            // Source by https://www.youtube.com/watch?v=_DqGODw6Htg by ASP.NET MVC March 8,2017. They show how to add a search bar with searching with the name
            //var pet = db.Pets.SqlQuery("select * from pets").FirstOrDefault();
            var pets = from s in db.Pets select s;
           // var petty = from
            if (!String.IsNullOrEmpty(search))
            {
                pets = pets.Where(s => s.PetName.Contains(search) || s.Species.Name.Contains(search));
            }


            return View(pets.ToList());

        }

        // GET: Pet/Details/5
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Pet pet = db.Pets.Find(id); //EF 6 technique
            Pet pet = db.Pets.SqlQuery("select * from pets where petid=@PetID", new SqlParameter("@PetID", id)).FirstOrDefault();
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        //THE [HttpPost] Means that this method will only be activated on a POST form submit to the following URL
        //URL: /Pet/Add
        // TODO: Add Validation
        [HttpPost]
        public ActionResult Add(string PetName, Double PetWeight, String PetColor, int SpeciesID, string PetNotes)
        {
            //STEP 1: PULL DATA! The data is access as arguments to the method. Make sure the datatype is correct!
            //The variable name  MUST match the name attribute described in Views/Pet/Add.cshtml

            //Tests are very useul to determining if you are pulling data correctly!
            //Debug.WriteLine("Want to create a pet with name " + PetName + " and weight " + PetWeight.ToString()) ;

            //STEP 2: FORMAT QUERY! the query will look something like "insert into () values ()"...
            string query = "insert into pets (PetName, Weight, color, SpeciesID, Notes) values (@PetName,@PetWeight,@PetColor,@SpeciesID,@PetNotes)";
            SqlParameter[] sqlparams = new SqlParameter[5]; //0,1,2,3,4 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@PetName", PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@SpeciesID", SpeciesID);
            sqlparams[4] = new SqlParameter("@PetNotes", PetNotes);

            //db.Database.ExecuteSqlCommand will run insert, update, delete statements
            //db.Pets.SqlCommand will run a select statement, for example.
            db.Database.ExecuteSqlCommand(query, sqlparams);


            //run the list method to return to a list of pets so we can see our new one!
            return RedirectToAction("List");
        }


        public ActionResult New()
        {
            //STEP 1: PUSH DATA!
            //What data does the Add.cshtml page need to display the interface?
            //A list of species to choose for a pet

            //alternative way of writing SQL -- will learn more about this week 4
            //List<Species> Species = db.Species.ToList();

            List<Species> species = db.Species.SqlQuery("select * from Species").ToList();

            return View(species);
        }

        public ActionResult Update(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //need information about a particular pet
            Pet selectedpet = db.Pets.SqlQuery("select * from pets where petid = @id", new SqlParameter("@id", id)).FirstOrDefault();
            
            if (selectedpet == null)
            {
                return HttpNotFound();
            }
            return View(selectedpet);
         
        }

        // TODO: Add Validation
        [HttpPost]
        public ActionResult Update(string PetName, string PetColor, double PetWeight, string PetNotes, int id)
        {

            //Debug.WriteLine("I am trying to edit a pet's name to "+PetName+" and change the weight to "+PetWeight.ToString());

            string query = "update pets set PetName = @PetName, Weight = @PetWeight, color = @PetColor, Notes = @PetNotes where PetID = @id";
            SqlParameter[] sqlparams = new SqlParameter[5];
            sqlparams[0] = new SqlParameter("@PetName", PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@PetNotes", PetNotes);
            sqlparams[4] = new SqlParameter("@id", id);

            //db.Database.ExecuteSqlCommand will run insert, update, delete statements
            //db.Pets.SqlCommand will run a select statement, for example.
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");

        }

 
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get the value of a specific petID
            Pet selectedpet = db.Pets.SqlQuery("select * from pets where petid = @id", new SqlParameter("@id", id)).FirstOrDefault();
            
            if (selectedpet == null)
            {
                return HttpNotFound();
            }

            return View(selectedpet);
        }


        [HttpPost]
        //This will refer to the controller "Delete" 
        [ActionName("Delete")]
        //since you cannot use Actionresult with similar Name and similar parameter, use ActionName and refer the controller to the Delete Controller
        //Another way is to add extra paramater with similar name, but I dont actually need the extra parameter for the method/controller
        public ActionResult Deletemethod(int id)
        // public ActionResult Delete(int id, FormCollection collection)
        {
            //get the value in the form to use to delete data
            string query = "delete from pets where PetID= @id";
            SqlParameter sqlparams = new SqlParameter("@id", id);

            //execute the query and all the parameter in the database
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
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
