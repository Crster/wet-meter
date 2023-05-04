namespace WetMe.Pages;

[QueryProperty(nameof(Mode), "mode")]
public partial class LoadingPage : ContentPage
{
    public string Mode { get; set; }
	public LoadingPage()
	{
		InitializeComponent();
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await Task.Delay(3000);

        switch (this.Mode)
        {
            case "MANUAL":
                await Shell.Current.GoToAsync("/manual-mode");
                break;
            case "TIMER":
                await Shell.Current.GoToAsync("/timer-mode");
                break;
            case "SENSOR":
                await Shell.Current.GoToAsync("/sensor-mode");
                break;
            default:
                await Shell.Current.GoToAsync("///mode");
                break;
        }

        base.OnNavigatedTo(args);

        Navigation.RemovePage(this);
    }
}