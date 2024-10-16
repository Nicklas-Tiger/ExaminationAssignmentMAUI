﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiExamResources.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Tjänst för att hantera produkter, injiceras via konstruktor (Dependency Injection).
    private readonly IProductService<Product, Product> _productService;

    // ObservableCollection för att hålla en lista över produkter.
    // ObservableCollection meddelar UI:t om förändringar (som tillägg eller borttagning av produkter).
    [ObservableProperty]
    private ObservableCollection<Product> _products;

    // Den aktuella produkten som användaren skapar eller redigerar.
    [ObservableProperty]
    private Product _currentProduct;

    // Felmeddelande för ogiltigt produktnamn, används för validering.
    [ObservableProperty]
    private string invalidName = null!;

    // Felmeddelande för ogiltig produktbeskrivning, används för validering.
    [ObservableProperty]
    private string invalidDescription = null!;

    // Felmeddelande för ogiltigt pris, används för validering.
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

        InvalidName = string.IsNullOrWhiteSpace(CurrentProduct.ProductName) ? "You must enter a name!" : "";


        InvalidDescription = string.IsNullOrWhiteSpace(CurrentProduct.ProductDescription) ? "You must enter a description!" : "";

        if (string.IsNullOrWhiteSpace(CurrentProduct.Price))
        {
            InvalidPrice = "You must enter a valid price!";
        }
        else if (!decimal.TryParse(CurrentProduct.Price, out _))
        {
            InvalidPrice = "Price must be a number!";
        }
        else
        {
            InvalidPrice = ""; 
        }
       
        var duplicateProduct = Products.FirstOrDefault(p => p.ProductName.Equals(CurrentProduct.ProductName));

        if (duplicateProduct != null && duplicateProduct.ProductId != CurrentProduct.ProductId)
        {
            DuplicateProduct = "Product with the same name already exists!"; 
            return; 
        }
        else
        {
            DuplicateProduct = ""; // Töm felmeddelandet om det inte finns en dubblett
        }

        // Om det finns något felmeddelande, returnera och tillåt inte att produkten sparas.
        if (string.IsNullOrEmpty(InvalidName) && string.IsNullOrEmpty(InvalidDescription) 
            && string.IsNullOrEmpty(InvalidPrice) && string.IsNullOrEmpty(DuplicateProduct))
        {
            try
            {
                // Kolla om produkten redan finns i listan (baserat på ProductId).
                var existingProduct = Products.FirstOrDefault(p => p.ProductId == CurrentProduct.ProductId);

                if (existingProduct != null)
                {
                    // Uppdatera produkten om den redan finns.
                    var updateResult = _productService.UpdateProduct(CurrentProduct.ProductId, CurrentProduct);

                    if (updateResult.Success)
                    {
                        // Ta bort den gamla versionen av produkten från listan och lägg till den uppdaterade produkten.
                        Products.Remove(existingProduct);
                        Products.Add(CurrentProduct);

                        // Återställ CurrentProduct till en ny produkt för framtida skapande.
                        CurrentProduct = new Product();
                    }
                    else
                    {
                        // Om uppdateringen misslyckas, visa felmeddelande.
                        DuplicateProduct = "";
                    }
                }
                else
                {
                    // Skapa en ny produkt om den inte redan finns.
                    var result = _productService.CreateProduct(CurrentProduct);

                    if (result.Success)
                    {
                        Products.Add(result.Result!); // Lägg till den nya produkten i listan.
                        CurrentProduct = new Product(); // Återställ CurrentProduct till en ny produkt.
                    }
                    else
                    {
                        // Om skapandet misslyckas, visa felmeddelande.
                        Console.WriteLine(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    // Kommando för att förbereda en produkt för uppdatering (kopierar värden till CurrentProduct).
    [RelayCommand]
    public void Update(Product selectedProduct)
    {
        try
        {
            if (selectedProduct != null)
            {
                // Kopierar värden från den valda produkten till CurrentProduct för redigering.
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

    // Kommando för att ta bort en produkt från listan och tjänsten.
    [RelayCommand]
    public void Delete(Product productToDelete)
    {
        try
        {
            if (productToDelete != null)
            {
                // Försöker ta bort produkten från tjänsten.
                var result = _productService.DeleteProduct(productToDelete.ProductId);
                if (result.Success)
                {
                    // Om borttagningen lyckas, ta bort produkten från listan.
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
