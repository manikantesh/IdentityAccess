using IdentityAccessShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IdentityAccess.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
            string accessToken = e.Parameter.ToString();

			HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

			var response = await client.GetAsync("http://localhost:44329/api/auth/weatherforecast");

			var responseBody = await response.Content.ReadAsStringAsync();

			var weatherForecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecastResponse>>(responseBody);

            lstWeatherForecast.ItemsSource = weatherForecast;

			base.OnNavigatedTo(e);
		}
	}
}
