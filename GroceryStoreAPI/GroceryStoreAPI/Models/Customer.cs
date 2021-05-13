using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace GroceryStoreAPI.Models
{
    public class Customer
    {
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
       
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
