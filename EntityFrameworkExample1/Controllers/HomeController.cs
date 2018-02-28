using System;
using System.Collections.Generic;
using System.Linq;   //must have this enabled in order to use linq
using System.Web;
using System.Web.Mvc;
using EntityFrameworkExample1.Models; //added this so that it will access the new model

namespace EntityFrameworkExample1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddNewCustomer(Customer newCustomer)
        //we would previously pass strings in as parameters. now we are making a class that holds all the information and just calling that object. can't talk to the database until you make the ORM object
        {

            //1. Create the ORM to convert the database info into an object we can work with

            NorthwindEntities MyORM = new NorthwindEntities();
            //{class from the dbase}{name the ORM} = new instance of Northwind Entities
            //can do customers.toList, array, etc
            //add (); to indicate it's a method
            //included as a viewbag so that it can be displayed elsewhere


            //2. Action: Add new customer and save

            MyORM.Customers.Add(newCustomer);
            //this addds the new customer. because we connected the tables, we will add data to the form in the correct place. Don't need to create individual objects now. can add, remove, insert customers now

            MyORM.SaveChanges();
            //MUST save changes. this moves the change to the database. otherwise you just added it to a list that didn't go beyond this file

            //3. Send data to the view
            return View();
        }

        public ActionResult CustomersPage()
        {

            //1. Create the ORM 

            NorthwindEntities NorthwindORM = new NorthwindEntities();
            //copy the same ORM for each of these

            //2. Action: Create list that will hold the new data 

            ViewBag.CustomersData = NorthwindORM.Customers.ToList();

            //3. Send data to the view
            return View();
        }

        public ActionResult GetCustomersByCountry(string Country)
        //there is a corresponding input on the form for this field
        //adding parameter here because this is the only thing we are searching for. if we were searching for more, we would have to add more parameters. but this is a simple example.
        {
            //1. ORM 
            NorthwindEntities NorthwindORM = new NorthwindEntities();

            //2. Action: Convert this into a list that can be displayed

            ViewBag.CustomersData = NorthwindORM.Customers.Where(x => x.Country == Country).ToList();
            //declare a temp varaible called x. this thing is called a lambda expression.
            //where is a Linq keyword that means we are looking through the object for a certain thing for any matchees and will return it back to the list. There are many Linq keywords, mostly related to sql functions
            //lambdas are like shortcuts to creating functions on the fly without creating a separate function.
            //once we have the list, need to show the results on the same page for Customers

            //3. Send data to the view
            return View("CustomersPage");
        }



        public ActionResult GetCustomersByCompanyName(string CompanyName)
        {
            //1. ORM 
            NorthwindEntities NorthwindORM = new NorthwindEntities();

            //2. Action: find the orders and create a viewbag to display it

            ViewBag.CustomersData = NorthwindORM.Customers.Where(x => x.CompanyName.Contains(CompanyName)).ToList();
            //select customers Where input = company name which contains whatever input we put in the company name field on the form then send it to the list

            //3. Send data to the view
            return View("CustomersPage");
            //return to the same view as above, but the content is different based on the search parameters
        }


        public ActionResult GetOrdersByCID(string CustomerID)  //this is the specific parameter I am searching by
        {

            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();


            //2. Action: query and display

            ViewBag.CustomerOrders = NorthwindORM.Customers.Find(CustomerID).Orders.ToList();
            //first build the query and then attach the viewbag and all it customer orders
            //this is a subquery. first find the customer, then find the orders
            //this says: go to customer table and find the custID that has orders attached, and send that info to a list
            //the Linq for Find allows a specific search by primary key
            //every entity has a navigation property that allows for searching. they are built in sql and defined by the foreign key. if the relationship is not built, that navigation won't be an available property. it's a way for us to jump from one table to another


            //3. Send data to the view
            return View("OrderDetails");
        }

        public ActionResult DeleteOrdersbyCID(string CustomerID)
        {
            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();

            //2. Action: Find the customer, remove, then save changes

            NorthwindORM.Customers.Remove(NorthwindORM.Customers.Find(CustomerID));
            //if the record has any dependency, delete those first and then the dependency (i.e, if they have orders, must delete those first and THEN the customer)

            NorthwindORM.SaveChanges();

            //3. Send data to the view
            //return View("CustomersPage");  

            //3a option: before you to go the final view, execute this action
            return RedirectToAction("CustomersPage");

        }


        public ActionResult UpdateCustomerbyCID(string CustomerID) 
        {
            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();


            //2. Action: Find the customer, load the existing data onto a form

            Customer ToBeUpdated = NorthwindORM.Customers.Find(CustomerID);  
            ViewBag.CustomerToBeUpdated = ToBeUpdated;
            //ToBeUpdated is a var I just declared for this search

            //3. Send data to the view
           
            return View("CustomerDetails");

           

        }

        public ActionResult SaveCustomerChanges(Customer NewCustomer)  //this is the data updated from the form
        {
            //0. Validation (build this last, but it's really the first step that gets executed)

            if (!ModelState.IsValid)

            //this statement checks the data validations in the model.tt/customer.cs page
            //if model is not valid, go to an error page. if it is, return the view
            {
                //if someone is trying to bypass my validation in the model, can request their ip address like this and other info to see who they are with Request.UserHostAddress()

                ////a way to show individual errors per field:
                //foreach (ModelState s in ModelState.Values)

                //{
                //    foreach (ModelError r in s.Errors)
                //    {
                //        ////individual error for each field
                //        //r.ErrorMessage
                //    }
                //}


                return View("../Shared/Error"); //go to error page 
            }
            
            //1. ORM

            NorthwindEntities NorthwindORM = new NorthwindEntities();  //copy the same ORM for each of these


            //2. Action: Find the customer, update values, save
            NorthwindORM.Entry(NorthwindORM.Customers.Find(NewCustomer.CustomerID)).CurrentValues.SetValues(NewCustomer);

            //to change the old values to new values, need to first find the old customer object by doing Find. 
            //Instead ofgoing to each field and changing individ, Entry will change them all at once based on what you entered. 
            //Take the current values and replace it with the new stuff from the form (setvalues)
            //can also play with .originalvalues vs currentvalues
            //it uses current values to update the form
            //"currentvalue.setvalue" means take the current value and set it based on what I entered
            
            
            NorthwindORM.SaveChanges();

            //3. Go to customer view (but first need to load customer data(??))
            return RedirectToAction("CustomersPage");
        }


    }
}

////do validation, but also do put each orm code block in its own try and catch (include the return view) becuase this is where it connects to the database and errors often occur here. for example:

//public ActionResult AddNewCustomer(Customer newCustomer) 
//try     
//{
//{
//            //1. Create an object from the ORM class in the .context file
//            NorthwindEntities MyORM = new NorthwindEntities();

//MyORM.Customers.Add(newCustomer); 
//            MyORM.SaveChanges();  
//            return View();
//        }

//catch (Exception e)
//    {
//    ModelState.Values.Add(new ModelState());

//    return View("..Shared/Error";)  
//}
//}