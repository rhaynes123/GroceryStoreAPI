using System;
using System.Text.Json.Serialization;
using System.IO;
using GroceryStoreAPI.Models;
using System.Text.Json;
using System.Linq;

namespace GroceryStoreAPI.Data
{
    public class JsonFileHelper : IJsonFileHelper
    {
        public readonly string _JsonDataFile;
        public JsonFileHelper()
        {
            _JsonDataFile = "database.json";
        }

        public void SaveChanges(Customer customer)
        {
            try
            {
                CustomerList customerList = new();
                Customer newCustomer = customer;
                if (File.Exists(_JsonDataFile))
                {
                    customerList = JsonSerializer.Deserialize<CustomerList>(File.ReadAllText(_JsonDataFile));
                    if(customerList != null)
                    {
                        Customer foundCustomer = newCustomer.Id > 0
                            ? customerList.Customers.FirstOrDefault(r => r.Id == customer.Id)
                            : throw new ArgumentException("Invalid Id Passed");
                        if (foundCustomer == null)
                        {
                            customerList.Customers.Add(newCustomer);
                        }
                        else
                        {
                            int foundcustomerindex = customerList.Customers.IndexOf(foundCustomer);
                            customerList.Customers[foundcustomerindex] = customer;
                        }
                    }
                }
                else
                {
                    customerList.Customers.Add(newCustomer);
                }
                File.WriteAllText(_JsonDataFile, JsonSerializer.Serialize(customerList));
            }
            catch(Exception)
            {
                throw new InvalidOperationException($"Could not Save {customer} to {_JsonDataFile}");
            }
            
        }

        public CustomerList ReadFrom()
        {
            return File.Exists(_JsonDataFile)
                ? JsonSerializer.Deserialize<CustomerList>(File.ReadAllText(_JsonDataFile))
                : new CustomerList();
        }

        public Customer ReadFromById(int id)
        {
            return ReadFrom().Customers.FirstOrDefault(r =>r.Id == id) ?? new Customer();
        }
    }
}
