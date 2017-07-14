using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Support.V4.App;
using System.Threading.Tasks;
using Xamarin.Forms;
using GPS.Messages;
using Plugin.Geolocator;

using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using OperationCanceledException = Android.OS.OperationCanceledException;

namespace GPS.Droid.Services
{
    [Service]
    public class TrackLocationService : Service
    {
        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void DisplayNotification(string message)
        {
            int notificationId = 1000;

            Intent resultIntent = new Intent(this, typeof(MainActivity));

            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            PendingIntent resultPendingIntent =
            stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // Pass the current button press count value to the next activity:
            Bundle valuesForActivity = new Bundle();

            // Build the notification:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetAutoCancel(false)                    // Dismiss from the notif. area when clicked
                .SetContentIntent(resultPendingIntent)
                .SetContentTitle("Content Title")      // Set its title
                .SetContentText("Content Text")
                .SetContentInfo("Content Info")
                .SetOngoing(true)
                .SetSmallIcon(Resource.Drawable.notification_icon_background)
                .SetContentText(message); // The message to display.


            // Finally, publish the notification:
            //NotificationManager notificationManager =
            //    (NotificationManager)GetSystemService(Context.NotificationService);
            //notificationManager.Notify(ButtonClickNotificationId, builder.Build());

            StartForeground(notificationId, builder.Build());

            // Increment the button press count:
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var message = new LocationMessage();

                    message.Position = CrossGeolocator.Current.GetPositionAsync(System.TimeSpan.FromSeconds(5)).Result;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<LocationMessage>(message, "LocationMessage");
                    });

                    Task.Run(async () =>
                    {
                        var locationTask = new LocationTask();

                        await locationTask.SendLocation(message.Position);

                    });

                    DisplayNotification($"{message.ToString()} às: {System.DateTime.Now.ToString("hh:mm:ss")}");
                    //DisplayNotification($"{message.Position.Latitude.ToString("N2")},{message.Position.Longitude.ToString("N2")} às: {System.DateTime.Now.ToString("hh:mm:ss")}");
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        Device.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(message, "CancelledMessage")
                        );
                    }
                }

            });

            return StartCommandResult.Sticky;

            //return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();

            StopForeground(true);
        }
    }
}