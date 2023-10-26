using CommunityToolkit.Mvvm.Input;
using EquationProcessing;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace Calculator_App;

class MainViewModel: INotifyPropertyChanged
{
    private Calculator _calculator;
    private string? _result = string.Empty;
    public MainViewModel()
    {
        _calculator = new Calculator();

        CleanCommand = new RelayCommand(() =>
        {
            PastValue = string.Empty;
            PastOperation = string.Empty;
            CurrentValue = string.Empty;
            _result = string.Empty;
        });

        InputCommand = new RelayCommand<string>(x =>
        {
            CurrentValue += x;
        });

        OperationCommand = new RelayCommand<string>(x =>
        {
            PastValue = CurrentValue;
            PastOperation = x;
            _result = CurrentValue + PastOperation;

            CurrentValue = string.Empty;
        });

        СalculationsCommand = new RelayCommand(() =>
        {
            _result += CurrentValue;
            CurrentValue = Calculator.Calculation(_result);

            PastValue = string.Empty; 
            PastOperation = string.Empty;
            _result = string.Empty;
        });
    }

    private string? _pastOperation = "";
    public string? PastOperation
    {
        get => _pastOperation;
        set
        {
            _pastOperation = value;
            OnPropertyChanged();
        }
    }

    private string? _pastValue = "";
    public string? PastValue
    {
        get => _pastValue;
        set
        {
            _pastValue = value;
            OnPropertyChanged();
        }
    } 

    private string? _currentValue = "0";
    public string? CurrentValue
    {
        get => _currentValue;
        set
        {
            _currentValue = Calculator.Handler(value);

            _currentValue = (_currentValue == "0" || _currentValue.Contains(",") || _currentValue.Contains(".")) ? _currentValue : _currentValue.TrimStart('0');
            OnPropertyChanged();
        } 
    }


    public ICommand CleanCommand { get; }
    public RelayCommand<string> InputCommand { get; }
    public RelayCommand<string> OperationCommand { get; }
    public RelayCommand СalculationsCommand { get; }



    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
