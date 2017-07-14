using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using GPS.Messages;
using Android.Content;
using GPS.Droid.Services;
using Android.Support.V4.App;

namespace GPS.Droid
{
    [Activity(Label = "GPS", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            WireUpTrackLocationService();
        }

        public void WireUpTrackLocationService()
        {
            AlarmManager alarm = (AlarmManager)this.GetSystemService(Context.AlarmService);

            MessagingCenter.Subscribe<StartLocationServiceMessage>(this, "StartLocationServiceMessage", message =>
            {
                //Xamarin.Forms.Forms.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings));

                var intent = new Intent(this, typeof(TrackLocationService));

                //StartService(intent);

                //PendingIntent pendingServiceIntent = PendingIntent.GetBroadcast(this, 0, new Intent(intent), PendingIntentFlags.UpdateCurrent);
                PendingIntent pendingServiceIntent = PendingIntent.GetService(this, 0, new Intent(intent), PendingIntentFlags.UpdateCurrent);


                alarm.SetInexactRepeating(AlarmType.RtcWakeup, 60 * 1000, 60 * 1000, pendingServiceIntent);
                //alarm.SetRepeating(AlarmType.RtcWakeup, 0, 10 * 1000, pendingServiceIntent);
            });

            MessagingCenter.Subscribe<StopLocationServiceMessage>(this, "StopLocationServiceMessage", message =>
            {
                var intent = new Intent(this, typeof(TrackLocationService));
                StopService(intent);

                PendingIntent pendingServiceIntent = PendingIntent.GetService(this, 0, new Intent(intent), PendingIntentFlags.UpdateCurrent);
                alarm.Cancel(pendingServiceIntent);

                // Build the notification:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
                    .SetContentIntent(pendingServiceIntent)
                    .SetOngoing(false);

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(1000, builder.Build());
            });


        }
    }


}

