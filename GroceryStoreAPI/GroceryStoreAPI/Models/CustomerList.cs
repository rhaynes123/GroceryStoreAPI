using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroceryStoreAPI.Models
{
    public class CustomerList
    {
        [JsonPropertyName("customers")]
        public List<Customer> Customers { get; set; }
    }
}
