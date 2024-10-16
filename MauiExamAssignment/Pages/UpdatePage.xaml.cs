using MauiExamResources.ViewModels;

namespace MauiExamAssignment.Pages;

public partial class UpdatePage : ContentPage
{
	public UpdatePage(UpdateViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
	}
}