using WetMe.Models;
using WetMe.Services;

namespace WetMe.Pages;

public partial class SensorModePage : ContentPage
{
    string historyLogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/manual.log";
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
        if (File.Exists(historyLogPath))
        {
            History.Text = File.ReadAllText(historyLogPath);
        }

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

        MoistureLog.Text = MoistureLog.Text.Insert(0 ,Environment.NewLine + data.DateTime.ToString() + ": " + data.Moisture.ToString());
        MoistureGraph.AddData(data);
        MoistureGraphView.Invalidate();

        if (MoistureGraph.IsWaterLevelChanged)
        {
            History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + ": " + $"Water Level is {MoistureGraph.WaterLevelStatus} ({MoistureGraph.WaterLevel})");
            File.WriteAllText(historyLogPath, History.Text);
        }
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

            History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + ": Sensor mode activated");
            History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + $": Low level is set to {lowLevel}");
            File.WriteAllText(historyLogPath, History.Text);
        }
        else
        {
            MoistureGraph.LowLevel = 300;
            await DisplayAlert("Invalid Level", "Low water level should be 50 to 1024", "Gotcha!");
        }
    }
}