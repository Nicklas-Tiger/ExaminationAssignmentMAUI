using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using System.Collections.ObjectModel;

namespace MauiExamResources.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProductService<Product, Product> _productService;

    [ObservableProperty]
    private ObservableCollection<Product> _products;

    [ObservableProperty]
    private Product _currentProduct;

    //[ObservableProperty]
    //private string invalidName;

    //[ObservableProperty]
    //private string invalidDescription;

    //[ObservableProperty]
    //private string invalidPrice; 

    public MainViewModel(IProductService<Product, Product> productService)
    {
        _productService = productService;
        _products = new ObservableCollection<Product>(_productService.GetAllProducts().Result ?? new List<Product>());
        _currentProduct = new Product();
    }

    [RelayCommand]
    public void Save(Product product)
    {
        try
        {
            //InvalidName = string.IsNullOrWhiteSpace(product.ProductName) ? "You must enter a name" : "";
            //InvalidDescription = string.IsNullOrWhiteSpace(product.ProductDescription) ? "You must enter a description" : "";
            //InvalidPrice = string.IsNullOrWhiteSpace(product.Price) ? "You must enter a valid price" : "";

            if (!string.IsNullOrWhiteSpace(CurrentProduct.ProductName))
            {
                var result = _productService.CreateProduct(CurrentProduct);

                if (result.Success)
                {
                    Products.Add(result.Result!);
                    CurrentProduct = new Product();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    [RelayCommand]
    public void Update(Product selectedProduct)
    {
        try
        {
            if (selectedProduct != null)
            {
                CurrentProduct = new Product
                {
                    ProductId = selectedProduct.ProductId,
                    ProductName = selectedProduct.ProductName,
                    ProductCategory = selectedProduct.ProductCategory,
                    ProductDescription = selectedProduct.ProductDescription,
                    Price = selectedProduct.Price
                };

                Products.Remove(selectedProduct);
            }
        }
        catch (Exception)
        {
        }

    }
    [RelayCommand]
    public void Delete(Product productToDelete)
    {
        try
        {
            if (productToDelete != null)
            {
                var result = _productService.DeleteProduct(productToDelete.ProductId);
                if (result.Success)
                    Products.Remove(productToDelete); 

            }
        }
        catch (Exception) {}
       

    }

}
