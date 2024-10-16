using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiExamResources.Models;

public class Product : ObservableObject
{

    public string ProductId { get; set; } = Guid.NewGuid().ToString();
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public Category ProductCategory { get; set; } = new();
    public string? Price { get; set; }


}
