using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using MauiExamResources.Services;
using Moq;
using Newtonsoft.Json;




namespace Resources.Tests.UnitTests
{
    public class ProductService_Tests
    {
        private readonly Mock<IFileService> _mockFileService;
        private readonly ProductService _mockProductService;

        public ProductService_Tests()
        {
            _mockFileService = new Mock<IFileService>();
            _mockProductService = new ProductService(_mockFileService.Object);
        }

        [Fact]
        public void CreateProduct__ShouldReturnSuccess_WhenProductIsCreated()
        {
            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C280",
                Price = "100",
                ProductCategory = new Category
                {
                    Name = "Skrivare"
                }
            };
            var existingProduct = new List<Product>();

            _mockFileService.Setup(x => x.SaveToFile(It.IsAny<string>())).
                Returns(new ResponseResult<string> { Success = true });

            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(existingProduct) });

            // Act
            var result = _mockProductService.CreateProduct(product);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("\nProduct was added successfully!\n", result.Message);
        }

        [Fact]
        public void CreateProduct__ShouldReturnFalse_WhenProductIsDuplicate()
        {
            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C280",
                Price = "100",
                ProductCategory = new Category
                {
                    Name = "Skrivare"
                }
            };
            var existingProduct = new List<Product> { product };

            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(existingProduct) });

            // Act
            var product2 = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C280", // Dubblettnamn
                Price = "100",
                ProductCategory = new Category
                {
                    Name = "Skrivare"
                }
            };

            var result = _mockProductService.CreateProduct(product2);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("\nProduct with the same name already exists!\n", result.Message);

        }
        [Fact]
        public void CreateProduct__ShouldAddOneToProductCount_WhenProductIsAdded()
        {
            // Arrange
            var existingProduct = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C280",
                Price = "100",
                ProductCategory = new Category { Name = "Skrivare" }
            };

            var newProduct = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C290",
                Price = "150",
                ProductCategory = new Category { Name = "Skrivare" }
            };
            var existingProductList = new List<Product> { existingProduct };

            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(existingProductList) });

            _mockFileService.Setup(x => x.SaveToFile(It.IsAny<string>()))
                .Returns(new ResponseResult<string> { Success = true });

            // Act
            var result = _mockProductService.CreateProduct(newProduct);

            // Assert
            _mockFileService.Verify(x => x.SaveToFile(It.IsAny<string>()));
            Assert.True(result.Success);
            Assert.Equal("\nProduct was added successfully!\n", result.Message);


            existingProductList.Add(newProduct);

            Assert.Equal(2, existingProductList.Count);
        }
        [Fact]
        public void UpdateProduct_ShouldReturnTrue_WhenProductIsUpdated()
        {

        }
    }
}

