using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using GPS.Messages;
using GPS.ViewModels;
using Plugin.Geolocator;

namespace GPS
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var viewModel = new MainPageViewModel();

            BindingContext = viewModel;

            HandleReceivedMessages();
        }

        public void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<LocationMessage>(this, "LocationMessage", message => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var viewModel = (BindingContext as MainPageViewModel);
                    var position = message.Position;

                    viewModel.Posicao = $"Lat: {position.Latitude.ToString("N2")}; Long: {position.Longitude.ToString("N2")}; Prec: {position.Accuracy.ToString("N2")}; às: {DateTime.Now.ToString("hh:mm:ss")}";
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, "CancelledMessage", message => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var viewModel = (BindingContext as MainPageViewModel);
                    viewModel.Posicao = "Servico desativado";
                });
            });
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var message = new StartLocationServiceMessage();
            MessagingCenter.Send(message, "StartLocationServiceMessage");

            if (!CrossGeolocator.Current.IsGeolocationEnabled)
            {
                await DisplayAlert("GPS desligado", "Por favor, ative o GPS", "OK");
                return;
            }

            try
            {
                var viewModel = (BindingContext as MainPageViewModel);

                var position = await viewModel.GetPositionAsync();

                if (position != null)
                    viewModel.Posicao = $"Lat: {position.Latitude.ToString("N2")}; Long: {position.Longitude.ToString("N2")}; Precisao: {position.Accuracy.ToString("N2")}";
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private void Button_StopClicked(object sender, EventArgs e)
        {
            var message = new StopLocationServiceMessage();
            MessagingCenter.Send(message, "StopLocationServiceMessage");
        }
    }
}
