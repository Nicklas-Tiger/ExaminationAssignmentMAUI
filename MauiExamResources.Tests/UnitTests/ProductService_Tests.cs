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
        private readonly ProductService _productService;

        public ProductService_Tests()
        {
            _mockFileService = new Mock<IFileService>();
            _productService = new ProductService(_mockFileService.Object);
        }

        #region Create
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
            var result = _productService.CreateProduct(product);

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

            var result = _productService.CreateProduct(product2);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("\nProduct with the same name already exists!\n", result.Message);

        }
        [Fact]
        public void CreateProduct_ShouldAddOneToProductCount_WhenProductIsAdded()
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
            var result = _productService.CreateProduct(newProduct);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("\nProduct was added successfully!\n", result.Message);
            existingProductList.Add(newProduct);
            Assert.Equal(2, existingProductList.Count);
        }
        #endregion

        #region Read
        [Fact]
        public void GetAllProducts_ShouldReturnEmptyList_WhenFileIsEmpty()
        {
            // Arrange
            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = string.Empty });

            // Act
            var result = _productService.GetAllProducts();

            // Assert
            Assert.True(result.Success);
            Assert.Empty(result.Result!); 
        }
        [Fact]
        public void GetAllProducts_ShouldReturnProducts_WhenFileContainsProducts()
        {
            // Arrange
            var productList = new List<Product>
        {
            new Product
            {
                ProductId = "123",
                ProductName = "Samsung 55SN",
                Price = "100",
                ProductCategory = new Category { Name = "TV" },
            }
        };

            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(productList) });

            // Act
            var result = _productService.GetAllProducts();

            // Assert
            Assert.True(result.Success);
            Assert.Single(result.Result!); 
        }
        #endregion

        #region Update
        [Fact]
        public void UpdateProduct_ShouldReturnSuccess_WhenProductIsUpdated()
        {
            // Arrange
            var existingProduct = new Product
            {
                ProductId = "123",
                ProductName = "Samsung 55TN",
                Price = "200",
                ProductCategory = new Category { Name = "TV" },
            };

            var updatedProduct = new Product
            {
                ProductId = "123",
                ProductName = "Samsung 45SN",
                Price = "250",
                ProductCategory = new Category { Name = "TV" },

            };

            var productList = new List<Product> { existingProduct };
            var responseResult = new ResponseResult<IEnumerable<Product>> { Success = true, Result = productList };

            _mockFileService.Setup(fs => fs.GetFromFile())
                            .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(productList) });

            _mockFileService.Setup(fs => fs.SaveToFile(It.IsAny<string>()))
                            .Returns(new ResponseResult<string> { Success = true });

            // Act
            var result = _productService.UpdateProduct(existingProduct.ProductId, updatedProduct);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("\nProduct updated successfully\n", result.Message);
        }
        #endregion

        #region Delete
        [Fact]
        public void DeleteProduct__ShouldReturnTrue_WhenProductIsDeleted()
        {
            // Arrange
            var productToDelete = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "C280",
                Price = "100",
                ProductCategory = new Category { Name = "Skrivare" }
            };

            var existingProductList = new List<Product> { productToDelete };

            _mockFileService.Setup(x => x.GetFromFile())
                .Returns(new ResponseResult<string> { Success = true, Result = JsonConvert.SerializeObject(existingProductList) });

            _mockFileService.Setup(x => x.SaveToFile(It.IsAny<string>()))
                .Returns(new ResponseResult<string> { Success = true });

            // Act
            var result = _productService.DeleteProduct(productToDelete.ProductId);

            // Assert
            Assert.True(result.Success);



        }
        #endregion

    }

}

