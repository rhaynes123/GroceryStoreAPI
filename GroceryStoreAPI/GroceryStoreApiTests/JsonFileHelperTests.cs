using System;
using System.IO;
using System.Linq;
using GroceryStoreAPI.Models;
using GroceryStoreAPI.Data;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace GroceryStoreApiTests
{
    public class JsonFileHelperTests
    {
        private readonly JsonFileHelper jsonFileHelper;
        private readonly Mock<IJsonFileHelper> mockJsonfileHelper;
        private readonly Mock<IConfiguration> mockConfig = new();
        private readonly Customer fakecustomer;
        private const string dbFile = "database.json";
        public JsonFileHelperTests()
        {
            mockJsonfileHelper = new Mock<IJsonFileHelper>();
            jsonFileHelper = new(mockConfig.Object);
            fakecustomer = new Customer
            {
                Id = 5,
                Name = "Richard"
            };
        }
        [Fact]
        public void TestRealFilesExists()
        {
            //Arrange
            //Act
            //Assert
            Assert.True(File.Exists(dbFile));
        }
        [Fact]
        public void TestRealFileHasData()
        {
            //Arrange
            var customerList = JsonSerializer.Deserialize<CustomerList>(File.ReadAllText(dbFile));
            //Act
            //Assert
            Assert.NotNull(customerList);
            Assert.NotEmpty(customerList.Customers);
        }
        [Fact]
        public void TestReadFromRetunsTypeOfCustomerList()
        {
            //Arrange
            mockJsonfileHelper.Setup(r => r.ReadFrom()).Returns(new CustomerList());
            //Act
            var result = mockJsonfileHelper.Object.ReadFrom(); ;
            //Assert
            Assert.IsType<CustomerList>(result);
        }
        
        [Fact]
        public void TestReadFromReturnsMockData()
        {
            //Arrange
            var customers = new CustomerList
            {
                Customers = new List<Customer>
                {
                    new Customer
                    {
                        Id = 1,
                        Name = "Richard"
                    }
                }
            };
            //Act
            mockJsonfileHelper.Setup(r => r.ReadFrom()).Returns(customers);
            //Assert
            Assert.NotNull(customers);
            Assert.NotEmpty(customers.Customers);

        }
        [Fact]
        public void TestReadFromByIdReturnsMockData()
        {
            int testId = 2;
            //Arrange
            var customers = new CustomerList
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
            //Act
            Customer expectedCustomer = customers.Customers.FirstOrDefault(r => r.Id == testId);
            mockJsonfileHelper.Setup(r => r.ReadFromById(testId)).Returns(customers.Customers.FirstOrDefault( r=>r.Id == testId));
            var actual = mockJsonfileHelper.Object.ReadFromById(testId);
            //Assert
            Assert.Equal(actual, expectedCustomer);

        }
        [Fact]
        public void TestSaveChangesCanUpdateFile()
        {
            //Arrange
            //Act
            mockJsonfileHelper.Setup(r => r.SaveChanges(It.IsAny<Customer>()));
            //Assert
            mockJsonfileHelper.Object.SaveChanges(fakecustomer);
            mockJsonfileHelper.Verify(r => r.SaveChanges(fakecustomer));
        }

        [Fact]
        public void TestSaveChangesCanUpdateFileThrowsInvalidOperationException()
        {
            //Arrange
            //Act
            mockJsonfileHelper.Setup(r => r.SaveChanges(It.IsAny<Customer>())).Throws<InvalidOperationException>();
            //Assert
            Assert.Throws<InvalidOperationException>(() => mockJsonfileHelper.Object.SaveChanges(It.IsAny<Customer>())); 
        }
    }
}
