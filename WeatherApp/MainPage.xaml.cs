using Newtonsoft.Json;
using System;
using System.Net.Http;
using WeatherApp.Models;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        private double _temperature;
        private int _humidity;
        private int _cloudCover;
        private double _windSpeed;
        private string _currentCity;

        public double Temperature
        {
            get { return _temperature; }
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        public int Humidity
        {
            get { return _humidity; }
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        public int CloudCover
        {
            get { return _cloudCover; }
            set
            {
                _cloudCover = value;
                OnPropertyChanged();
            }
        }

        public double WindSpeed
        {
            get { return _windSpeed; }
            set
            {
                _windSpeed = value;
                OnPropertyChanged();
            }
        }

        public string CurrentCity
        {
            get { return _currentCity; }
            set
            {
                _currentCity = value;
                OnPropertyChanged();
            }
        }

        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            GetCurrentLocation();
        }

        private async void GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    // Store latitude and longitude
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;

                    // Call GetCityAndWeather method with current location
                    GetCityAndWeather(latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get location: {ex}");
            }
        }

        private async void GetCityAndWeather(double latitude, double longitude)
        {
            try
            {
                // Get the city name using reverse geocoding
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    CurrentCity = placemark.Locality;
                }

                // Fetch weather data using the city name
                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={CurrentCity}&appid=6bc966250c4ebf44f3fff0210d1d43c8&units=metric";
                string response = await _httpClient.GetStringAsync(new Uri(apiUrl));
                Root responseData = JsonConvert.DeserializeObject<Root>(response);

                if (responseData != null)
                {
                    Temperature = responseData.main.temp;
                    Humidity = responseData.main.humidity;
                    CloudCover = responseData.clouds.all;
                    WindSpeed = responseData.wind.speed * 3.6;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error fetching weather data: {ex}");
            }
        }
    }
}
