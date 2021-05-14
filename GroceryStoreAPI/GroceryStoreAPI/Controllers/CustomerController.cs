using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IJsonFileHelper _jsonFileHelper;
        private readonly string DatabaseNullError = "Customer Database was Null";
        public CustomerList CustomersList { get;  set; }
       
        public CustomerController(ILogger<CustomerController> logger
            , IJsonFileHelper jsonFileHelper)
        {
            _logger = logger;
            _jsonFileHelper = jsonFileHelper ?? throw new ArgumentNullException(nameof(_jsonFileHelper));
           
        }
        // GET: api/Customer
        /// <summary>
        /// Get
        /// </summary>
        /// <returns>Customer List </returns>
        /// <response code="200">Customer Found</response>
        /// <response code="404">Customer Not Found</response>
        /// <response code="500">Exception Raised</response>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                CustomersList = _jsonFileHelper.ReadFrom();
                if (CustomersList == null)
                {
                    _logger.LogCritical(DatabaseNullError);
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = DatabaseNullError });
                }
                if (CustomersList.Customers.Count == 0)
                {
                    return NotFound("No Customers Found");
                }
                else
                {
                    return Ok(CustomersList.Customers.OrderByDescending(r=>r.Id));
                }
                
            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception raised in Get. Exception:{ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = "An error was encountered looking for customers" });
            }
            
        }

        // GET: api/Customer/5
        /// <summary>
        /// Gets customer by id
        /// </summary>
        /// <param name="id">Customers Id</param>
        /// <returns>Found Customer</returns>
        /// <response code="200">Customer Found</response>
        /// <response code="404">Customer Not Found</response>
        /// <response code="500">Exception Raised</response>
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            try
            {
                Customer foundCustomer = _jsonFileHelper.ReadFromById(id);
                if(foundCustomer == null)
                {
                    return NotFound("No Customer Was Found with that Id");
                }
                return Ok(foundCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception raised in Get Id: {id}, Exception:{ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = $"An error was encountered looking for customer id: {id}" });
            }
        }

        // POST: api/Customer
        /// <summary>
        /// Creates a new Customer
        /// </summary>
        /// <param name="newCustomer"></param>
        /// <returns>Succesfull if Customer is Created</returns>
        /// <response code="201">Customer Created</response>
        /// <response code="400">Bad data provided</response>
        /// <response code="500">Exception Raised</response>
        [HttpPost]
        public ActionResult Post([FromBody] Customer newCustomer)
        {
            try
            {
                CustomersList = _jsonFileHelper.ReadFrom();
                if (CustomersList == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = DatabaseNullError });
                }
                if (string.IsNullOrWhiteSpace(newCustomer.Name))
                {
                    return BadRequest($"That Name cannot be empty: {newCustomer.Id}");
                }
                if (CustomersList.Customers.Exists(r => r.Id == newCustomer.Id) || newCustomer.Id <=0)
                {
                    return BadRequest($"That Id cannot be used id: {newCustomer.Id}");
                }
               
                _jsonFileHelper.SaveChanges(newCustomer);

                return CreatedAtAction(nameof(Get), new { id = newCustomer.Id }, newCustomer);
            }
            catch(Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = $"New Customer {newCustomer.Id}, {newCustomer.Name} couldn't be saved"});
            }
            
        }

        // PUT: api/Customer/5
        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Customername"></param>
        /// <returns>Successfully updated object</returns>
        /// <response code="200">Customer Found And Updated</response>
        /// <response code="404">Customer Not Found</response>
        /// <response code="400">Bad data provided</response>
        /// <response code="500">Exception Raised</response>
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string Customername)
        {
            
            try
            {
                Customer existingCostumer = new()
                {
                    Id = id,
                    Name = HttpUtility.HtmlEncode(Customername)//Sanitizing the name string
                };
                CustomersList = _jsonFileHelper.ReadFrom();
                if (CustomersList == null)
                {
                    return StatusCode(500, new { error = DatabaseNullError });
                }
                if (string.IsNullOrWhiteSpace(existingCostumer.Name))
                {
                    return BadRequest("That Name cannot be empty");
                }
                if (!CustomersList.Customers.Exists(r => r.Id == id))
                {
                    return NotFound($"A customer with this Id wasn't found id: {existingCostumer.Id}");
                }
                else
                {
                    _jsonFileHelper.SaveChanges(existingCostumer);
                    return Ok(existingCostumer);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = $"New Customer {id}, {Customername} couldn't be saved" });
            }
        }

        
    }
}
