using WetMe.Models;
using WetMe.Services;

namespace WetMe.Pages;

public partial class TimerModePage : ContentPage
{
    RestService restService;
    bool isTimerMode = false;

    IDispatcherTimer timer;
    public TimerModePage()
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

        string mode = restService.GetMode();
        if (mode == "Timer")
        {
            Submit.Text = "Deactivate Timer Mode";
            isTimerMode = true;
        }
        else
        {
            Submit.Text = "Activate Timer Mode";
            isTimerMode = false;
        }

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
        if (isTimerMode)
        {
            restService.StopPump();
        }
        else
        {
            string value = Interval.SelectedItem as string;

            switch (value)
            {
                case "3 Second":
                    restService.SetTimerMode(3000);
                    break;
                case "5 Second":
                    restService.SetTimerMode(5000);
                    break;
                case "5 Minutes":
                    restService.SetTimerMode(1000 * 60 * 5);
                    break;
                case "1 Hour":
                    restService.SetTimerMode(1000 * 60 * 60);
                    break;
            }
        }
    }
}