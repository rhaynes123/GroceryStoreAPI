using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace GroceryStoreAPI.Models
{
    public class Customer
    {
        
        [Required,JsonPropertyName("id")]
        public int Id { get; set; }
       
        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid Characters"), MaxLength(50),JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
