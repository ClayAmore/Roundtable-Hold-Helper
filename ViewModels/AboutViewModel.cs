using CommunityToolkit.Mvvm.ComponentModel;
using Roundtable.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Wpf.Ui.Common.Interfaces;

namespace Roundtable.ViewModels
{
    public partial class AboutViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private IEnumerable<AboutColor> _colors;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            var random = new Random();
            var colorCollection = new List<AboutColor>();

            for (int i = 0; i < 8192; i++)
                colorCollection.Add(new AboutColor
                {
                    Color = new SolidColorBrush(Color.FromArgb(
                        (byte)200,
                        (byte)random.Next(0, 250),
                        (byte)random.Next(0, 250),
                        (byte)random.Next(0, 250)))
                });

            Colors = colorCollection;

            _isInitialized = true;
        }
    }
}
