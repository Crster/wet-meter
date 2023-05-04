using WetMe.Services;

namespace WetMe.Pages;

public partial class ModePage : ContentPage
{
    RestService restService;

    IDispatcherTimer timer;
    public ModePage()
	{
		InitializeComponent();

        ManualButton.IsEnabled = false;
        SensorButton.IsEnabled = false;
        TimerButton.IsEnabled = false;

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(1000);
        timer.Tick += Timer_Tick;

        restService = new RestService();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        timer.Start();
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        timer.Stop();
        base.OnNavigatedFrom(args);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (restService.IsConnected())
        {
            ManualButton.IsEnabled = true;
            SensorButton.IsEnabled = true;
            TimerButton.IsEnabled = true;
        } 
        else
        {
            ManualButton.IsEnabled = false;
            SensorButton.IsEnabled = false;
            TimerButton.IsEnabled = false;
        }
    }

    async private void ManualMode_Clicked(object sender, EventArgs e)
    {
        if (restService.IsConnected())
        {
            await Shell.Current.GoToAsync("/loading", new Dictionary<string, object>
            {
                { "mode", "MANUAL" }
            });
        }
        else
        {
            await DisplayAlert("Device Offline", "Please turn on watering device", "Okay");
        }
    }

    async private void SensorMode_Clicked(object sender, EventArgs e)
    {
        if (restService.IsConnected())
        {
            await Shell.Current.GoToAsync("/loading", new Dictionary<string, object>
            {
                { "mode", "SENSOR" }
            });
        }
        else
        {
            await DisplayAlert("Device Offline", "Please turn on watering device", "Okay");
        }
    }

    async private void TimerMode_Clicked(object sender, EventArgs e)
    {
        if (restService.IsConnected())
        {
            await Shell.Current.GoToAsync("/loading", new Dictionary<string, object>
            {
                { "mode", "TIMER" }
            });
        }
        else
        {
            await DisplayAlert("Device Offline", "Please turn on watering device", "Okay");
        }
    }
}