using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorPickerWPF.Code;
using TouchbarMaker.Core;
using TouchbarMaker.Tools;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;

namespace TouchbarMaker.ViewModels
{
    public class ElementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum ElementType
        {
            Button
        }

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
                Bitmap = Converter.ConvertBitmapImage(_icon);
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

        private bool _doTintText;
        public bool DoTintText
        {
            get => _doTintText;
            set
            {
                _doTintText = value;
                OnPropertyChanged();

                if (!_doTintText)
                {
                    TextColor = null;
                }
            }
        }

        private Color? _textColor;
        public Color? TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                if (TextColor.HasValue)
                    PreviewTitleColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_textColor.Value.A,
                        _textColor.Value.R, _textColor.Value.G, _textColor.Value.B));
                else
                    PreviewTitleColor = _disabledColor;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PreviewTitleColor));
            }
        }

        private bool _doTintBackground;
        public bool DoTintBackground
        {
            get => _doTintBackground;
            set
            {
                _doTintBackground = value;
                OnPropertyChanged();

                if (!_doTintBackground)
                {
                    BackgroundColor = null;
                }
            }
        }

        private Color? _backgroundColor;
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                if (_backgroundColor.HasValue)
                    PreviewBackgroundColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_backgroundColor.Value.A,
                        _backgroundColor.Value.R, _backgroundColor.Value.G, _backgroundColor.Value.B));
                else
                    PreviewBackgroundColor = _disabledColor;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PreviewBackgroundColor));
            }
        }

        public Brush PreviewTitleColor { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 232, 232));
        public Brush PreviewBackgroundColor { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 232, 232));

        public ElementType Type { get; }
        public ICommand SetTitleColorCommand { get; set; }
        public ICommand SetBackgroundColorCommand { get; set; }

        private readonly SolidColorBrush _disabledColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 232, 232));

        public ElementViewModel(ElementType type, string id = null)
        {
            Type = type;
            Id = id ?? Guid.NewGuid().ToString().Substring(0, 8);

            SetTitleColorCommand = new Commander(o =>
            {
                if (ColorPickerWPF.ColorPickerWindow.ShowDialog(out var color, ColorPickerDialogOptions.SimpleView))
                {
                    TextColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                }
                else
                {
                    TextColor = null;
                }
            }, o => true);

            SetBackgroundColorCommand = new Commander(o =>
            {
                if (ColorPickerWPF.ColorPickerWindow.ShowDialog(out var color, ColorPickerDialogOptions.SimpleView))
                {
                    BackgroundColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                }
            }, o => true);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}