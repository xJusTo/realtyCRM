using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Services;
using Xunit;

namespace RealtyCRM.Tests
{
    public class DealServiceTests
    {
        private readonly Mock<IDealRepository> _dealRepositoryMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly DealService _dealService;

        public DealServiceTests()
        {
            _dealRepositoryMock = new Mock<IDealRepository>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _dealService = new DealService(_dealRepositoryMock.Object, _propertyRepositoryMock.Object);
        }

        [Fact]
        public async Task Test_CreateDeal_Success()
        {
            // Arrange
            var property = new Property { Id = 1, Status = PropertyStatus.Available };
            var deal = new Deal { PropertyId = 1, FinalPrice = 100000 };

            _propertyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            var result = await _dealService.CreateDealAsync(deal);

            // Assert
            result.Should().NotBeNull();
            property.Status.Should().Be(PropertyStatus.Sold);
            _dealRepositoryMock.Verify(r => r.AddAsync(deal), Times.Once);
            _propertyRepositoryMock.Verify(r => r.UpdateAsync(property), Times.Once);
        }

        [Fact]
        public async Task Test_CreateDeal_ThrowsException_WhenPropertyNotAvailable()
        {
            // Arrange
            var property = new Property { Id = 1, Status = PropertyStatus.Sold };
            var deal = new Deal { PropertyId = 1 };

            _propertyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            Func<Task> act = async () => await _dealService.CreateDealAsync(deal);

            // Assert
            await act.Should().ThrowAsync<InvalidDealStateException>()
                .WithMessage("Недвижимость недоступна для сделки");
        }

        [Fact]
        public async Task Test_CreateDeal_CallsUpdateRepository()
        {
            // Arrange
            var property = new Property { Id = 1, Status = PropertyStatus.Available };
            var deal = new Deal { PropertyId = 1 };

            _propertyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _dealService.CreateDealAsync(deal);

            // Assert
            _propertyRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Property>(p => p.Id == 1 && p.Status == PropertyStatus.Sold)), Times.Once);
        }
    }
}
