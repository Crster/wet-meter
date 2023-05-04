namespace WetMe.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    async private void Login_Clicked(object sender, EventArgs e)
    {
        if (Password.Text == "wetman")
        {
            await Shell.Current.GoToAsync("///mode");
        }
        else
        {
            await DisplayAlert("Invalid Password!", "Please remember your password and try again.", "Try Again");
        }
    }
}