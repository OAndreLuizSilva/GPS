using GPS.Messages;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GPS
{
    public class LocationTask
    {
        Random _random = new Random(175);

        public async Task SendLocation(Position position)
        {

        }

        public async Task GetLocationAsync(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                var message = new LocationMessage();

                message.Position = await CrossGeolocator.Current.GetPositionAsync(System.TimeSpan.FromSeconds(5));

                Device.BeginInvokeOnMainThread(() =>
                {
                    MessagingCenter.Send<LocationMessage>(message, "LocationMessage");
                });
            }, token);
        }
    }
}
