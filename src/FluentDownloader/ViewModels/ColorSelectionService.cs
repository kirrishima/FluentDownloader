using FluentDownloader.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Devices;
using Windows.UI;

namespace FluentDownloader.ViewModels;

public class ColorSelectionService : INotifyPropertyChanged
{
    private static ColorSelectionService? _instance;
    public static ColorSelectionService Instance => _instance ??= new ColorSelectionService();

    private Color _selectedColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);

    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                OnPropertyChanged();
            }
        }
    }

    private double _opacity = 45;
    public double Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity != value)
            {
                _opacity = value;
                OnPropertyChanged();
            }
        }
    }

    private double _tintOpacity = 80;

    public double TintOpacity
    {
        get => _tintOpacity;
        set
        {
            if (_tintOpacity != value)
            {
                _tintOpacity = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _areSlidersVisible = false;
    public bool AreSlidersVisible
    {
        get => _areSlidersVisible;
        set
        {
            if (_areSlidersVisible != value)
            {
                _areSlidersVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public event Action? ShowColorPickerRequested;

    public void RequestShowColorPickerWithoutSliders()
    {
        AreSlidersVisible = false;
        ShowColorPickerRequested?.Invoke();
    }

    public void RequestShowColorPickerWithSliders()
    {
        AreSlidersVisible = true;
        ShowColorPickerRequested?.Invoke();
    }

    public event EventHandler? SaveClicked;

    public void SendSaveButtonClicked(ColorPickerControl control, EventArgs eventArgs)
        => SaveClicked?.Invoke(control, eventArgs);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null!) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
