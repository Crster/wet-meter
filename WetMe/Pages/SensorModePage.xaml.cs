using WetMe.Models;
using WetMe.Services;

namespace WetMe.Pages;

public partial class SensorModePage : ContentPage
{
    RestService restService;

    IDispatcherTimer timer;
    public SensorModePage()
    {
        InitializeComponent();
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(1000);
        timer.Tick += Timer_Tick;

        MoistureGraph.LowLevel = 300;
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
        MoistureData data = new MoistureData()
        {
            DateTime = DateTime.Now,
            Moisture = restService.GetWaterLevel()
        };

        MoistureLog.Text = MoistureLog.Text.Insert(MoistureLog.Text.Length,Environment.NewLine + data.DateTime.ToString() + ": " + data.Moisture.ToString());
        MoistureGraph.AddData(data);
        MoistureGraphView.Invalidate();
    }

    private async void Submit_Clicked(object sender, EventArgs e)
    {
        int lowLevel = Convert.ToInt32(SoilMoisture.Text);

        if (lowLevel < 1024 && lowLevel > 50)
        {
            if (restService.SetSensorMode(lowLevel))
            {
                MoistureGraph.LowLevel = lowLevel;
            }
        }
        else
        {
            MoistureGraph.LowLevel = 300;
            await DisplayAlert("Invalid Level", "Low water level should be 50 to 1024", "Gotcha!");
        }
    }
}