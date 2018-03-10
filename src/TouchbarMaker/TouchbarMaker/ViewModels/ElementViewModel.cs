using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using TouchbarMaker.Core;

namespace TouchbarMaker.ViewModels
{
    public class ElementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
                Bitmap = Tools.Converter.ConvertBitmapImage(_icon);
            }
        }

        private Bitmap _bitmap;
        public Bitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EncodedIcon));
            }
        }

        public string EncodedIcon => Bitmap?.ToEncodedIconFromBitmap();

        private string _keyCode;
        public string KeyCode
        {
            get => _keyCode;
            set
            {
                _keyCode = value;
                OnPropertyChanged();
            }
        }

        private bool _scaleImage2X;
        public bool ScaleImage2X
        {
            get => _scaleImage2X;
            set
            {
                _scaleImage2X = value;
                OnPropertyChanged();
            }
        }

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        private Color? _textColor;
        public Color? TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public ElementViewModel(string id = null)
        {
            Id = id ?? Guid.NewGuid().ToString().Substring(0, 8);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}