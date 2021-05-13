using System;
using GroceryStoreAPI.Models;

namespace GroceryStoreAPI.Data
{
    public interface IJsonFileHelper
    {
        void SaveChanges(Customer customer);
        CustomerList ReadFrom();
        Customer ReadFromById(int id);
    }
}
