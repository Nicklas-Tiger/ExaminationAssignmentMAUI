using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using Newtonsoft.Json;


namespace MauiExamResources.Services;

public class ProductService : IProductService<Product, Product>
{
    private readonly IFileService _fileService;
    private List<Product> _products;



    public ProductService(IFileService fileService)
    {
        _fileService = fileService;
        _products = new List<Product>();
    }


    //CREATE
    public ResponseResult<Product> CreateProduct(Product product)
    {

        if (string.IsNullOrEmpty(product.ProductName) || string.IsNullOrEmpty(product.Price))
        {
            return new ResponseResult<Product> { Success = false, Message = "\nInvalid product information.\n" };
        }
        if (!decimal.TryParse(product.Price, out var parsedPrice))
        {
            return new ResponseResult<Product> { Success = false, Message = "\nPrice must be a valid number.\n" };
        }
        try
        {
            var response = GetAllProducts();
            if (response.Success)
                _products = response.Result!.ToList();
            else
                return new ResponseResult<Product> { Success = false, Message = "\nFailed to load products.\n" };

            if (!_products.Any(x => x.ProductName == product.ProductName))
            {
                _products.Add(product);
                var json = JsonConvert.SerializeObject(_products);
                var result = _fileService.SaveToFile(json);


                if (result.Success)
                    return new ResponseResult<Product> { Success = true, Message = "\nProduct was added successfully!\n", Result = product };
                else
                    return new ResponseResult<Product> { Success = false, Message = "\nProduct was not added successfully!\n" };
            }
            return new ResponseResult<Product> { Success = false, Message = "\nProduct with the same name already exists!\n" };
        }
        catch (Exception ex)
        {
            return new ResponseResult<Product> { Success = false, Message = ex.Message };

        }
    }

    //READ
    public ResponseResult<IEnumerable<Product>> GetAllProducts()
    {
        var content = _fileService.GetFromFile();

 
        if (string.IsNullOrWhiteSpace(content.Result))
        {
            return new ResponseResult<IEnumerable<Product>> { Success = true,Result = [] };
        }

        if (content.Success)
        {
            try
            {
                var products = JsonConvert.DeserializeObject<List<Product>>(content.Result!);

                if (products == null)
                {
                    return new ResponseResult<IEnumerable<Product>> { Success = true,Result = [] };
                }

                return new ResponseResult<IEnumerable<Product>> { Success = true, Result = products };
            }
            catch (Exception ex)
            {
                return new ResponseResult<IEnumerable<Product>> { Success = false, Message = $"Failed to load products: {ex.Message}" };
            }
        }
        else
        {
            return new ResponseResult<IEnumerable<Product>> { Success = false, Message = content.Message };
        }
    }

    //UPDATE
    public ResponseResult<Product> UpdateProduct(string id, Product updatedProduct)
    {
        try
        {
            if (string.IsNullOrEmpty(updatedProduct.ProductName) || string.IsNullOrEmpty(updatedProduct.Price))
            {
                return new ResponseResult<Product> { Success = false, Message = "\nInvalid product information.\n" };
            }
            if (!decimal.TryParse(updatedProduct.Price, out var parsedPrice))
            {
                return new ResponseResult<Product> { Success = false, Message = "\nPrice must be a valid number.\n" };
            }
            var response = GetAllProducts();  
            if (response.Success)
            {
                _products = response.Result!.ToList(); 

                var existingProduct = _products.FirstOrDefault(x => x.ProductId == id);
                if (existingProduct != null)
                {

                    existingProduct.ProductName = updatedProduct.ProductName;
                    existingProduct.Price = updatedProduct.Price;
                    existingProduct.ProductCategory = updatedProduct.ProductCategory;
                    existingProduct.ProductDescription = updatedProduct.ProductDescription;

                    var json = JsonConvert.SerializeObject(_products);
                    var result = _fileService.SaveToFile(json);

                    if (result.Success)
                        return new ResponseResult<Product> { Success = true, Message = "\nProduct updated successfully\n", Result = existingProduct };
                    else
                        return new ResponseResult<Product> { Success = false, Message = "\nFailed to update product.\n" };
                }
                else
                    return new ResponseResult<Product> { Success = false, Message = "\nProduct not found.\n" };
            }
            else
                return new ResponseResult<Product> { Success = false, Message = response.Message };
        }
        catch (Exception ex)
        {
            return new ResponseResult<Product> { Success = false, Message = $"\nSomething went wrong: {ex.Message}\n" };
        }
    }
    //DELETE
    public ResponseResult<Product> DeleteProduct(string id)
    {
        try
        {
            var response = GetAllProducts();
            if (!response.Success)
                return new ResponseResult<Product> { Success = false, Message = response.Message };

            var productList = response.Result!.ToList();
            var product = response.Result!.FirstOrDefault(x => x.ProductId == id);

            if (product != null)
            {
                productList.Remove(product);

                var json = JsonConvert.SerializeObject(productList);
                var result = _fileService.SaveToFile(json);

                if (result.Success)
                    return new ResponseResult<Product> { Success = true, Message = "\nProduct removed successfully\n" };
                else
                    return new ResponseResult<Product> { Success = false, Message = "\nFailed to remove product.\n" };
            }
            else
                return new ResponseResult<Product> { Success = false, Message = "\nProduct not found!\n" };
        }
        catch (Exception ex)
        {
            return new ResponseResult<Product> { Success = false, Message = $"\nSomething went wrong: {ex.Message}\n" };
        }

    }
}
