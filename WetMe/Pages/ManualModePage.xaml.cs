using WetMe.Models;
using WetMe.Services;

namespace WetMe.Pages;

public partial class ManualModePage : ContentPage
{
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

    private void Submit_Clicked(object sender, EventArgs e)
    {
        restService.SetManualMode();
    }

    private void Stop_Clicked(object sender, EventArgs e)
    {
        restService.StopPump();
    }
}