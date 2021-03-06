﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static Employee GetEmployee(string firstName, string lastName)
        {
            Employee employee = db.Employees.Where(e => e.FirstName == firstName && e.LastName == lastName).FirstOrDefault();

            return employee;
        }

        internal static List<Employee> GetEmployees()
        {
            List<Employee> allEmployees = db.Employees.ToList();

            return allEmployees;
        }

        
        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
       
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber) //Hello Jackson, again!
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                Employee newEmployeeUser = new Employee();
                newEmployeeUser.Email = email;
                newEmployeeUser.EmployeeNumber = employeeNumber;

                db.Employees.InsertOnSubmit(newEmployeeUser);
                db.SubmitChanges();

                employeeFromDb = newEmployeeUser;
                return employeeFromDb;
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    AddEmployee(employee);
                    break;
                case "read":
                    ReadEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "delete":
                    DeleteEmployee(employee);
                    break;
                default:
                    break;
            }
        }

        private static void ReadEmployee(Employee employeeNumber)
        {  
        }

        private static void AddEmployee(Employee employee)
        {   
        }

        internal static void AddEmployee(string firstName, string lastName, string userName, string password, int employeeNumber, string email)
        {
            Employee newEmployee = new Employee();

            newEmployee.FirstName = firstName;
            newEmployee.LastName = lastName;
            newEmployee.UserName = userName;
            newEmployee.Password = password;
            newEmployee.EmployeeNumber = employeeNumber;
            newEmployee.Email = email;

            db.Employees.InsertOnSubmit(newEmployee);
            db.SubmitChanges();
        }

        internal static void ReadEmployee(int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeNumber == employeeNumber).FirstOrDefault();

            Console.WriteLine(employeeFromDb.FirstName);
            Console.WriteLine(employeeFromDb.LastName);
            Console.WriteLine(employeeFromDb.EmployeeNumber);
        }

        internal static void UpdateEmployee(Employee employeeWithUpdates)
        {
            Employee employeeFromDb = null;
            try
            {
                employeeFromDb = db.Employees.Where(e => e.EmployeeId == employeeWithUpdates.EmployeeId).Single();
            }
            catch (InvalidOperationException c)
            {
                Console.WriteLine("No employees have an EmployeeId that matches the employee passed in.");
                Console.WriteLine("No updates have been made.");
                return;
            }

            employeeFromDb.FirstName = employeeWithUpdates.FirstName;
            employeeFromDb.LastName = employeeWithUpdates.LastName;
            employeeFromDb.UserName = employeeWithUpdates.UserName;
            employeeFromDb.Password = employeeWithUpdates.Password;
            employeeFromDb.Email = employeeWithUpdates.Email;

            db.Employees.InsertOnSubmit(employeeFromDb);
            db.SubmitChanges();
        }

        internal static void DeleteEmployee(Employee employeeDelete)
        {
            Employee employeeFromDb;


        }

        //TODO: Animal CRUD Operations
        internal static void RunAnimalQueries(Animal animal, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    AddAnimal(animal);
                    break;
                case "read":
                    ReadAnimal(animal);
                    break;
                case "update":
                    //UpdateAnimal();
                    break;
                case "delete":
                    RemoveAnimal(animal);
                    break;
                default:
                    break;

            }

        }

        internal static void AddAnimal(int categoryId, string name, int age, string demeanor, string kidFriendly, string petFriendly, int weight, string dietPlan)
        {
            Animal newAnimal = new Animal(); //Added criteria into this on 5/14 @ 11:27am ~ Steve

            newAnimal.CategoryId = categoryId;
            newAnimal.Name = name;
            newAnimal.Age = age;
            newAnimal.Demeanor = demeanor;
            newAnimal.KidFriendly = kidFriendly;
            newAnimal.PetFriendly = petFriendly;
            newAnimal.Weight = weight;
            newAnimal.DietPlanId = dietPlan;

            db.Animals.InsertOnSubmit(newAnimal);
            db.SubmitChanges();
            //AddAnimal(animal);
        }

        internal static void ReadAnimal(Animal animal)
        {
            RemoveAnimal(animal);
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            UpdateAnimal(animalId, updates);
        }

        internal static void RemoveAnimal(Animal animal)
        {
            RemoveAnimal(animal);
        }

        internal static void GetAnimalByID(int animalId)
        {
            GetAnimalByID(animalId);
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }

        // TODO: Animal CRUD Operations
        private static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        private static void GetAnimalByID(Animal animalId)
        {
            throw new NotImplementedException();
        }


        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?use a switch case to switch on the dictionary key
        {
            //switch(item.key)
            //    case "1":
            //    Console.WriteLine(db.Animals[0]);
            //    break;
            //case "2":
            //    Console.WriteLine(db.Animals[1]);
            //    break;
            //case "3":
            //    Console.WriteLine(db.Animals[2]);
            //    break;
            //case "4":
            //    Console.WriteLine(db.Animals[3]);
            //    break;
            //case "4":
            //    Console.WriteLine(db.Animals[4]);
            //    break;

            throw new NotImplementedException();
        }



        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void GetDietPlanId(Animal animal)
        {

        }

        internal static void GetCategoryId(Animal animal)
        {

        }


        // TODO: Adoption CRUD Operations
        internal static void RunAdoptionQueries(Animal animal, Client client, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    Adopt(animal, client);
                    break;
                case "read":
                    //Iqueryable?
                    break;
                case "update":
                    UpdateAdoption(animal, client);
                    break;
                case "delete":
                    RemoveAdoption(animal, client);
                    break;
                default:
                    break;
            }
        }

        private static void Adopt(Animal animal, Client client)
        {  
        }

        private static void RemoveAdoption(Animal animal, Client client)
        {
        }

        private static void UpdateAdoption(object isAdopted, object adoption)
        {  
        }

        internal static void Adopt(int animal, int client)
        {
            Adoption adoption = new Adoption();

            adoption.AnimalId = animal;
            adoption.ClientId = client;
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(Adoption adoptionUpdates)
        {
            Animal animal = null;

            try
            {
                Adoption updateApproval = db.Adoptions.Where(a => a.ApprovalStatus == adoptionUpdates.ApprovalStatus).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No animals with that status available.");
                Console.WriteLine("No updates have been made.");
                return;
            }

            animal.AdoptionStatus = adoptionUpdates.ApprovalStatus;

            db.SubmitChanges();
        }

        internal static void RemoveAdoption(Adoption adoptionRemove)
        {
            Animal animal = null;

            try
            {
                
            }
            catch (InvalidOperationException f)
            {

            }
        }



        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}