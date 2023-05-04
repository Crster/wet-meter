namespace WetMe;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        //Register all routes
        Routing.RegisterRoute("login", typeof(Pages.LoginPage));
        Routing.RegisterRoute("loading", typeof(Pages.LoadingPage));
        Routing.RegisterRoute("mode", typeof(Pages.ModePage));
        Routing.RegisterRoute("manual-mode", typeof(Pages.ManualModePage));
        Routing.RegisterRoute("timer-mode", typeof(Pages.TimerModePage));
        Routing.RegisterRoute("sensor-mode", typeof(Pages.SensorModePage));
    }
}
