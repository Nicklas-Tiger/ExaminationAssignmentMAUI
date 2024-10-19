using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiExamResources.ViewModels;

public partial class MainViewModel : ObservableObject
{

    private readonly IProductService<Product, Product> _productService;


    [ObservableProperty]
    private ObservableCollection<Product> _products;


    [ObservableProperty]
    private Product _currentProduct;

  
    [ObservableProperty]
    private string invalidName = null!;

    [ObservableProperty]
    private string invalidDescription = null!;

    [ObservableProperty]
    private string invalidCategory = null!;

    [ObservableProperty]
    private string invalidPrice = null!;

    [ObservableProperty]
    private string duplicateProduct = null!;


    public MainViewModel(IProductService<Product, Product> productService)
    {
        _productService = productService;
        _products = new ObservableCollection<Product>(_productService.GetAllProducts().Result ?? new List<Product>());
        _currentProduct = new Product(); 
    }

    
    [RelayCommand]
    public void Save()
    {
        InvalidCategory = string.IsNullOrEmpty(CurrentProduct.ProductCategory.Name) ? "You must enter a category!" : "";
        InvalidName = string.IsNullOrWhiteSpace(CurrentProduct.ProductName) ? "You must enter a name!" : "";

        if (string.IsNullOrWhiteSpace(CurrentProduct.Price))
            InvalidPrice = "You must enter a valid price!";
        else if (!decimal.TryParse(CurrentProduct.Price, out _))
            InvalidPrice = "Price must be a number!";
        else
            InvalidPrice = ""; 
       
        var duplicateProduct = Products.FirstOrDefault(p => p.ProductName.Equals(CurrentProduct.ProductName));

        if (duplicateProduct != null && duplicateProduct.ProductId != CurrentProduct.ProductId)
        {
            DuplicateProduct = "Product with the same name already exists!"; 
            return; 
        }
        else
            DuplicateProduct = ""; 


        if (string.IsNullOrEmpty(InvalidName) && string.IsNullOrEmpty(InvalidPrice) && string.IsNullOrEmpty(DuplicateProduct))
        {
            try
            {

                var existingProduct = Products.FirstOrDefault(p => p.ProductId == CurrentProduct.ProductId);

                if (existingProduct != null)
                {

                    var updateResult = _productService.UpdateProduct(CurrentProduct.ProductId, CurrentProduct);

                    if (updateResult.Success)
                    {

                        Products.Remove(existingProduct);
                        Products.Add(CurrentProduct);

                        CurrentProduct = new Product();
                    }
                    else
                        DuplicateProduct = "";
                }
                else
                {
                    var result = _productService.CreateProduct(CurrentProduct);

                    if (result.Success)
                    {
                        Products.Add(result.Result!); 
                        CurrentProduct = new Product(); 
                    }
                    else
                        Console.WriteLine(result.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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
                {
                   
                    Products.Remove(productToDelete);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
