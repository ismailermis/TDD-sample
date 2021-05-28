using System;
using Xunit;
using Play.Catalog.Service;
using Play.Catalog.Service.Repositories;
using Moq;
using Play.Catalog.Service.Entities;
using Microsoft.Extensions.Logging;
using Play.Catalog.Service.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using FluentAssertions;

namespace Catalog.UnitTests
{
    public class ItemControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loogerStub = new();
        private readonly Random rnd = new();

        [Fact]
        //UnitOfWork_StateUnderTest_ExpectedBehavior ->isimlendirme templati
        public async Task GetByIdAsync_WithUnexistingItem_ReturnNotFound()
        {
            // Arrange

            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            // Act
            var result = await controller.GetByIdAsync(Guid.NewGuid());

            // Assert
            // Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();

        }
        [Fact]
        public async Task GetByIdAsync_WithUnexistingItem_ReturnExpectedItem()
        {
            // Arrange
            var expectedItem = CreatRandomItem();
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            // Act

            var result = await controller.GetByIdAsync(Guid.NewGuid());

            // Assert
            //result.Value.Should().BeEquivalentTo(expectedItem);

            result.Value.Should().BeEquivalentTo(expectedItem,
                options => options.ComparingByMembers<Item>());

            // Eski kod
            //Assert.IsType<ItemDto>(result.Result);
            //var dto = (result as ActionResult<ItemDto>).Value;
            //Assert.Equal(expectedItem.Id,dto.Id);  
            //Assert.Equal(expectedItem.Name,dto.Name);

        }

        [Fact]
        public async Task GetByIdAsync_WithUnexistingItem_ReturnsAllItem()
        {

            // Arrange
            var expextedItems = new[] { CreatRandomItem(), CreatRandomItem(), CreatRandomItem() };

            repositoryStub.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expextedItems);

            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            // Assert.IsType<NotFoundResult>(result.Result);
            result.Should().BeEquivalentTo(expextedItems,
                options => options.ComparingByMembers<Item>()
                );
        }

        [Fact]
        //UnitOfWork_StateUnderTest_ExpectedBehavior ->isimlendirme templati
        public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {

            var itemToCreate = new CreateItemDto("qwerty","abc",rnd.Next(1000));

            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            var result = await controller.PostAsync(itemToCreate);

            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;

            result.Should().BeEquivalentTo(createdItem,
               options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
               );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 2000);

        }
        [Fact]
        //UnitOfWork_StateUnderTest_ExpectedBehavior ->isimlendirme templati
        public async Task UpdateItemAsync_WithExistItem_ReturnNoContent()
        {
            // Arrange
            var existingItem = CreatRandomItem();
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
           .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto("deneme", "test", existingItem.Price + 3);

            // Act
            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            var result = await controller.PutAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();


        }
        [Fact]
        public async Task DeleteItemAsync_WithExistItem_ReturnNoContent()
        {
            // Arrange
            var existingItem = CreatRandomItem();
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
           .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;

            // Act
            var controller = new ItemsController(repositoryStub.Object, loogerStub.Object);

            var result = await controller.DeleteAsync(itemId);

            //Assert
            result.Should().BeOfType<NoContentResult>();


        }
        private Item CreatRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                Description = "Description",
                Name = "Test",
                Price = rnd.Next(1000)
            };
        }

    }
}
