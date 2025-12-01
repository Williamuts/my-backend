using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class LocationDetailPage : ContentPage
{
	public LocationDetailPage(LocationDetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}