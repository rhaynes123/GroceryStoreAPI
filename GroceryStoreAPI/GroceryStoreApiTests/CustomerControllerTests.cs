using System;
using System.Collections.Generic;
using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GroceryStoreAPI.Data;
using System.Linq;

namespace GroceryStoreApiTests
{
    public class CustomerControllerTests
    {
        private readonly Mock<ILogger<CustomerController>> customerControllerlogger = new();
        private readonly Mock<IJsonFileHelper> mockJsonFileHelper = new();
        private readonly CustomerController customerController;
        private readonly CustomerList fakecustomers;
        private readonly int testid = 2;

        public CustomerControllerTests()
        {
            customerController = new CustomerController( customerControllerlogger.Object
                , mockJsonFileHelper.Object);
            fakecustomers = new CustomerList
            {
                Customers = new List<Customer>
                {
                    new Customer
                    {
                        Id = 1,
                        Name = "Richard"
                    },
                    new Customer
                    {
                        Id = 2,
                        Name = "Mike"
                    }
                }
            };
        }
        #region Get Tests
        [Fact]
        public void GetCustomersReturnsTypeOfOkObject()
        {
            //Arrange
            var customers = fakecustomers;
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(customers);
           
            //Act
            
            var result = customerController.Get();
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCustomersReturnsTypeOf500()
        {
            //Arrange
            var customerController = new CustomerController(customerControllerlogger.Object , jsonFileHelper: mockJsonFileHelper.Object)
            {
                
                CustomersList = null //Attempting to mimic data corruption 
            };
            //Act
            ObjectResult result = (ObjectResult)customerController.Get();
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public void GetCustomersReturnsTypeOfNotFound()
        {
            //Arrange
            var CustomersList = new CustomerList
            {
                Customers = new List<Customer>()
            };
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(CustomersList);
            //Act
            var result = customerController.Get();
            //Assert
            Assert.IsType<NotFoundObjectResult>(result);// Expecting Any result other than 200 or Ok
        }


        #endregion Get Tests

        #region Get By Id Tests
        [Fact]
        public void GetCustomersByIDReturnsTypeOfOkObject()
        {
            //Arrange
            var customers = fakecustomers;
            mockJsonFileHelper.Setup(r => r.ReadFromById(testid)).Returns(customers.Customers.FirstOrDefault());

            //Act

            var result = customerController.Get(testid);
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCustomersByIDReturnsTypeOf500()
        {
            //Arrange
            var customerController = new CustomerController(customerControllerlogger.Object, jsonFileHelper: mockJsonFileHelper.Object)
            {

                CustomersList = null //Attempting to mimic data corruption 
            };
            mockJsonFileHelper.Setup(r => r.ReadFromById(testid)).Throws<Exception>();
            //Act
            ObjectResult result = (ObjectResult)customerController.Get(testid);
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public void GetCustomersByIDReturnsTypeOfNotFound()
        {
            //Arrange
            var CustomersList = new CustomerList
            {
                Customers = new List<Customer>()
            };
            mockJsonFileHelper.Setup(r => r.ReadFromById(testid)).Returns(CustomersList.Customers.FirstOrDefault());
            //Act
            var result = customerController.Get(testid);
            //Assert
            Assert.IsType<NotFoundObjectResult>(result);// Expecting Any result other than 200 or Ok
        }


        #endregion Get By Id Tests

        #region Post Tests
        [Fact]
        public void PostCustomerReturnsTypeOfCreatedAtActionResult()
        {
            //Arrange
            Customer newcustomer = new()
            {
                Id = 3,
                Name = "Dawn"
            };
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(newcustomer));
            //Act
            CreatedAtActionResult result = (CreatedAtActionResult)customerController.Post(newcustomer);
            //Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }
        [Fact]
        public void PostCustomerReturnsTypeOfBadRequestWhenAddingExistingCustomer()
        {
            //Arrange
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(fakecustomers.Customers[1]));
            //Act
            BadRequestObjectResult result = (BadRequestObjectResult)customerController.Post(fakecustomers.Customers[1]);
            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public void PostCustomerReturnsTypeOfBadRequestWhenIdIsZero()
        {
            //Arrange
            Customer newcustomer = new()
            {
                Id = 0,
                Name = "Dawn"
            };
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(newcustomer));
            //Act
            var result = customerController.Post(newcustomer);
            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public void PostCustomerReturnsTypeOfBadRequestWhenNameIsEmpty()
        {
            //Arrange
            Customer newcustomer = new()
            {
                Id = 3,
                Name = ""
            };
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(newcustomer));
            //Act
            var result = customerController.Post(newcustomer);
            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public void PostCustomerReturnsTypeOfBadRequestWhenExceptionIsRaised()
        {
            //Arrange
            Customer newcustomer = new()
            {
                Id = 3,
                Name = "3"
            };
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(newcustomer)).Throws<Exception>();
            //Act
            ObjectResult result = (ObjectResult)customerController.Post(newcustomer);
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, result.StatusCode);
        }
        #endregion Post Tests

        #region Put Tests
        [Fact]
        public void PutCustomerReturnsTypeOfOkObject()
        {
            //Arrange
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(fakecustomers.Customers[1]));
            //Act
            var result = customerController.Put(2, "Tom");
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void PutCustomerReturnsTypeOfBadRequestWhenNameIsEmpty()
        {
            //Arrange
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(fakecustomers.Customers[1]));
            //Act
            var result = customerController.Put(2, "");
            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public void PutReturnsTypeOfNotFoundObjectWhenIdIsNegative()
        {
            //Arrange
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(fakecustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(fakecustomers.Customers[1]));
            //Act
            var result = customerController.Put(-3, "Tom");
            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public void PutCustomerReturnsTypeOfServerErrorWhenExceptionIsRaised()
        {
            //Arrange
            
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Throws<Exception>();
            mockJsonFileHelper.Setup(r => r.SaveChanges(fakecustomers.Customers[1])).Throws<Exception>();
            //Act
            ObjectResult result = (ObjectResult)customerController.Put(2, "Tom");
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, result.StatusCode);
        }
        [Fact]
        public void PutCustomerReturnsTypeOfServerErrorWhenExceptionIsRaisedWhenListIsNull()
        {
            //Arrange
            CustomerList emptycustomers = null;
            mockJsonFileHelper.Setup(r => r.ReadFrom()).Returns(emptycustomers);
            mockJsonFileHelper.Setup(r => r.SaveChanges(It.IsAny<Customer>())).Throws<Exception>();
            //Act
            ObjectResult result = (ObjectResult)customerController.Put(2, "Tom");
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, result.StatusCode);
        }
        #endregion Put Tests
    }
}
