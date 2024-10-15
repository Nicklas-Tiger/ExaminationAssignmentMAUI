using MauiExamAssignment.Pages;
using MauiExamResources.Interfaces;
using MauiExamResources.Models;
using MauiExamResources.Services;
using MauiExamResources.ViewModels;
using Microsoft.Extensions.Logging;

namespace MauiExamAssignment;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("fa-regular-400.ttf", "far");
                fonts.AddFont("fa-solid-900.ttf", "fas");
                fonts.AddFont("Raleway-Regular.ttf", "RalewayRegular");
            });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddSingleton<IProductService<Product, Product>, ProductService>();
        builder.Services.AddSingleton<IFileService, FileService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
