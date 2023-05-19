using WetMe.Models;
using WetMe.Services;

namespace WetMe.Pages;

public partial class ManualModePage : ContentPage
{
    string historyLogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/manual.log";
    private RestService restService;

    IDispatcherTimer timer;
    public ManualModePage()
    {
        InitializeComponent();
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(1000);
        timer.Tick += Timer_Tick;

        restService = new RestService();
        MoistureGraph.LowLevel = 300;
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

        MoistureLog.Text = MoistureLog.Text.Insert(0, Environment.NewLine + data.DateTime.ToString() + ": " + data.Moisture.ToString());
        MoistureGraph.AddData(data);
        MoistureGraphView.Invalidate();

        if (MoistureGraph.IsWaterLevelChanged)
        {
            History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + ": " + $"Water Level is {MoistureGraph.WaterLevelStatus} ({MoistureGraph.WaterLevel})");
            File.WriteAllText(historyLogPath, History.Text);
        }
    }

    private void Submit_Clicked(object sender, EventArgs e)
    {
        restService.SetManualMode();
        History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + ": Manual mode activated");
        File.WriteAllText(historyLogPath, History.Text);
    }

    private void Stop_Clicked(object sender, EventArgs e)
    {
        restService.StopPump();
        History.Text = History.Text.Insert(0, Environment.NewLine + DateTime.Now.ToString() + ": Pump stopped");
        File.WriteAllText(historyLogPath, History.Text);
    }
}