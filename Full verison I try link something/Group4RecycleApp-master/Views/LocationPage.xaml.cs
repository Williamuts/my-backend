using Group4RecycleApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Group4RecycleApp.Views;

public partial class LocationPage : ContentPage
{
    private readonly LocationViewModel _viewModel;

    public LocationPage(LocationViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;

        // Move camera to Sibu initially
        RecycleMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(2.2873, 111.8305), Distance.FromKilometers(5)));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadPins();
    }

    void LoadPins()
    {
        RecycleMap.Pins.Clear();
        foreach (var center in _viewModel.Centers)
        {
            var pin = new Pin
            {
                Label = center.Name,
                Address = center.Address,
                Type = PinType.Place,
                Location = center.Location
            };

            // Handle Click
            pin.MarkerClicked += (s, args) =>
            {
                _viewModel.PinClickedCommand.Execute(center);
            };

            RecycleMap.Pins.Add(pin);
        }
    }
}