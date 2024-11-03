using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

public class RecipeItem : INotifyPropertyChanged
{
    private string _name;
    private string _url;
    private ImageSource _imageSource;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
        }
    }

    public ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            _imageSource = value;
            OnPropertyChanged();
        }
    }

    public RecipeItem(string name, string url, ImageSource imageSource)
    {
        Name = name;
        Url = url;
        ImageSource = imageSource;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

