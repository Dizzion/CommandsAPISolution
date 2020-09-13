using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Controllers;
using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        CommandsController controller;
        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemBD");
            dbContext = new CommandContext(optionsBuilder.Options);

            controller = new CommandsController(dbContext);
        }
        public void Dispose()
        {
            optionsBuilder = null;
            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }
        //ACTION 1 Tests: GET /api/commands
        //TEST 1.1 REQUEST OBJECTS WHEN NONE EXIST – RETURN "NOTHING"
        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //Arrange
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.Empty(result.Value);
        }
        //TEST 1.2: RETURNING A COUNT OF 1 FOR A SINGLE COMMAND OBJECT
        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.Single(result.Value);
        }
        //TEST 1.3: RETURNING A COUNT OF N FOR N COMMAND OBJECTS
        [Fact]
        public void GetCommandItemsReturnNItemsWhenDBHasNObjects()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            var command2 = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.CommandItems.Add(command2);
            dbContext.SaveChanges();
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.Equal(2, result.Value.Count());
        }
        //TEST 1.4: RETURNS THE EXPECTED TYPE
        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {
            //Arrange
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }
        //TEST 2.1 INVALID RESOURCE ID – NULL OBJECT VALUE RESULT
        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid
            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.Null(result.Value);
        }
        //TEST 2.2 INVALID RESOURCE ID – 404 NOT FOUND RETURN CODE
        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid
            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        //TEST 2.3 VALID RESOURCE ID – CHECK CORRECT RETURN TYPE
        [Fact]
        public void GetCommandItemReturnsTheCorrectType()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;
            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.IsType<ActionResult<Command>>(result);
        }
        //TEST 2.4 VALID RESOURCE ID – CORRECT RESOURCE RETURNED
        [Fact]
        public void GetCommandItemReturnsTheCorrectResouce()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;
            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.Equal(cmdId, result.Value.Id);
        }
        //TEST 3.1 VALID OBJECT SUBMITTED – OBJECT COUNT INCREMENTS BY 1
        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            var oldCount = dbContext.CommandItems.Count();
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
        }
        //TEST 3.2 VALID OBJECT SUBMITTED – 201 CREATED RETURN CODE
        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }
        //TEST 4.1 VALID OBJECT SUBMITTED – ATTRIBUTE IS UPDATED
        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;
            command.HowTo = "UPDATED";
            //Act
            controller.PutCommandItem(cmdId, command);
            var result = dbContext.CommandItems.Find(cmdId);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }
        //TEST 4.2 VALID OBJECT SUBMITTED – 204 RETURN CODE
        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;
            command.HowTo = "UPDATED";
            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }
        //TEST 4.3 INVALID OBJECT SUBMITTED – 400 RETURN CODE
        [Fact]
        public void PutCommandItem_Returns400_WhenInvalidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id + 1;
            command.HowTo = "UPDATED";
            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<BadRequestResult>(result);
        }
        //TEST 4.4 INVALID OBJECT SUBMITTED – OBJECT REMAINS UNCHANGED
        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var command2 = new Command
            {
                Id = command.Id,
                HowTo = "UPDATED",
                Platform = "UPDATED",
                CommandLine = "UPDATED"
            };
            //Act
            controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.CommandItems.Find(command.Id);

            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }
    }
}
