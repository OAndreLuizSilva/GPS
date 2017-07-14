using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace GPS.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _position;

        public string Posicao
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<Position> GetPositionAsync()
        {
            var locator = CrossGeolocator.Current;

            if (locator == null)
                return null;

            locator.DesiredAccuracy = 50;

            var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(5));

            return position;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
