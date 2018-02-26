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

        public ActionResult AddNewCustomer(Customer newCustomer) //would previously pass strings into here. now we are making a class that holds all the information and just calling that
             //can't talk to the database until you make the ORM object
        {
            //to fully connect the database:
            //1. Create an object from the ORM class in the .context file
            NorthwindEntities MyORM = new NorthwindEntities(); 

            MyORM.Customers.Add(newCustomer);  /*this addds the new customer. because we connected the tables, we will add data to the form in the correct place. Don't need to create individual objects now*/

            //can add, remove, insert customers now

            MyORM.SaveChanges();  /*MUST save changes. this moves the change to the database. otherwise you just added it to a list that didn't go beyond this file*/

            return View();
        }

        public ActionResult CustomersPage()
        {

            //1. ORM create the object that will pull the data from the dbase and create it as a list
            NorthwindEntities NorthwindORM = new NorthwindEntities();  //class from the dbase -- name the ORM -- = new instance of Northwind Entities 

            // select * from custoemrs

            //2. Create a viewbag and a list that will hold the new data in c#
            ViewBag.CustomersData = NorthwindORM.Customers.ToList();
                //ORM represents all the data coming fromthe custoemrs table
                //can do customers.toList, array, etc
                //add (); to indicate it's a method
                //Lazy Loading: before this line, nothing was fetched  Eager Loading: it loads now
                //included as a viewbag so that it can be displayed elsewhere
            return View();
        }

        public ActionResult GetCustomersByCountry(string Country)  //adding parameter here because this is the only way we are searching. if we were searching for more, we would have to add more parameters. but this is a simple example.
        {
            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();

          ViewBag.CustomersData=  NorthwindORM.Customers.Where(x => x.Country == Country).ToList();
            //where is a Linq keyword that means we are looking through the object for a certain thing for any matchees and will return it back to the list. There are many Linq keywords, mostly related to sql functions
            //declare a temp varaible called x. this thing is called a lambda expression.
            //lambdas are like shortcuts to creating functions on the fly without creating a separate function.
            //once we have the list, need to show the results on the same viewbagCustomersData

            return View("CustomersPage");
        }



        public ActionResult GetCustomersByCompanyName(string CompanyName)  
        {
            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();

            //2. find the orders and create a viewbag to display it

            ViewBag.CustomersData = NorthwindORM.Customers.Where(x => x.CompanyName.Contains(CompanyName)).ToList(); 
            //select customers Where input = company name which contains whatever input we put in the company name field on the form
            //then send it to the list
        
            return View("CustomersPage"); //name the view and return it

            //return the same view, but the content is different based on the search parameters
        }


        public ActionResult GetOrdersByCID(string CustomerID)  //this is the specific parameter I am searching by
        {

            //1. ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();


            //2 query and display
            ViewBag.CustomerOrders = //first build the query and then attach the viewbag and all it customer orders
           NorthwindORM.Customers.Find(CustomerID).Orders.ToList();
           
            //this is a subquery. first find the customer, then find the orders
            //this says: go to customer table and find the custID that has orders attached, and send that info to a list
            //the Linq for Find allows a specific search
            //every entitiy has a navigation property that allows for searching. they are built in and defined by the foreign key. if the relationship is not built, that navigation won't be an available property. it's a wya for us to jump from one table to another



            return View("OrderDetails");  //3 now go create the view
        }

        public ActionResult DeleteOrdersbyCID(string CustomerID)  //there is a corresponding input on the form for thise
        {
            //1 ORM
            NorthwindEntities NorthwindORM = new NorthwindEntities();  //copy the same ORM for each of these

            //2 have to find the customer, then remove
            //if the record has any dependency, delete those first and then the dependency (i.e, .if they have orders, must delete those first and THEN the customer)
            NorthwindORM.Customers.Remove(NorthwindORM.Customers.Find(CustomerID));

            //3 save
            NorthwindORM.SaveChanges();

            //4
            //return View("CustomersPage");  //return to the customer page when done

            //4 option: before you to go the final view, execute this action
            return RedirectToAction("CustomersPage");

        }


        //TO EDIT A CUSTOMER
        //1 Find the customer 2 load the existing data onto a form 3 save changes


        public ActionResult UpdateCustomerbyCID(string CustomerID) //there is a corresponding input on the form for thise
        {
            //1 ORM

            NorthwindEntities NorthwindORM = new NorthwindEntities();  //copy the same ORM for each of these


            //2 Find the customer
            Customer ToBeUpdated = NorthwindORM.Customers.Find(CustomerID);  //find allows a search by primary key  //ToBeUpdated is a var I just declared for this search

            //3 send data to the view
            ViewBag.CustomerToBeUpdated = ToBeUpdated;
            return View("CustomerDetails");

            //4 create a new html page to show the cust info for updating

        }

        public ActionResult SaveCustomerChanges(Customer NewCustomer)  //this is the data updated from the form
        {
            //0 Validation (built this last, but it's really the first step)

            if (!ModelState.IsValid)

            //this statement checks the data validations in the model.tt/customer.cs page
            //if model is not valid, go to an error page. if it is, return the view
            {
                //if someone is trying to bypass my validation in the model, can request their ip address and other info to see who they are with Request.UserHostAddress()

                ////a way to show idnividual errors per field:
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
            //1 ORM

            NorthwindEntities NorthwindORM = new NorthwindEntities();  //copy the same ORM for each of these


            //2 Find the customer
            NorthwindORM.Entry(NorthwindORM.Customers.Find(NewCustomer.CustomerID)).CurrentValues.SetValues(NewCustomer);
            //to change the old valuees to new values, need to first find the old customer object by doing Find. Instead ofgoing to each field and changing individ, Entry will change them all at once based on what you entered. 
            //Take the current values and replace it with the new stuff from the form (setvalues)
            //can also play with .originalvalues vs 
            //it uses curernt values to update the form
            //"currentvalue.setvalue" means take the current value and set it based on what I entered
            
            //3 save
            NorthwindORM.SaveChanges();

            //4 go to customer view (but first need to load customer data)
            return RedirectToAction("CustomersPage");
        }


    }
}

////do validation, but also do put each orm code block in its own try and catch (include the return view) becuase this is wehre it connects to thedatabase and errors often occur here. for example

//    //public ActionResult AddNewCustomer(Customer newCustomer) 
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