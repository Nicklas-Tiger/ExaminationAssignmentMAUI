using MauiExamResources.ViewModels;

namespace MauiExamAssignment.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

}