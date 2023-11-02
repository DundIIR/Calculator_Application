using CommunityToolkit.Mvvm.Input;
using EquationProcessing;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;


namespace Calculator_App;

class MainViewModel: INotifyPropertyChanged
{
    private Calculator _calculator;


    private bool OperationSelection = false;
    private bool CalculationSelection = false;

    public MainViewModel()
    {
        _calculator = new Calculator();

        CleanCommand = new RelayCommand(() =>
        {
            PastValue = string.Empty;
            PastOperation = string.Empty;
            CurrentValue = string.Empty;
            OperationSelection = false;
            TempResult = string.Empty;
            Result = string.Empty;
        });

        InversionCommand = new RelayCommand(() =>
        {
            if (CurrentValue.Length > 0 && CurrentValue[0] != '-')
                CurrentValue = '-' + CurrentValue;
            else
                CurrentValue = CurrentValue.Substring(1);

        });

        InputCommand = new RelayCommand<string>(x =>
        {
            if(OperationSelection)                       // Если вводим первую цифру после ввода операции
            {
                if (CurrentValue.Contains("(")) Result = "(";
                CurrentValue = string.Empty;
                _result += PastValue + PastOperation;
                OperationSelection = false;
            }

            CurrentValue += x;
            PastValue = CurrentValue;

            OnPropertyChanged(nameof(PastValue));
        });

        OperationCommand = new RelayCommand<string>(x =>
        {
            if (CalculationSelection)
            {
                PastValue = CurrentValue;
                CalculationSelection = false;
            }
            if (!string.IsNullOrEmpty(PastOperation) && (x == "+" || x == "-")) // Если мы вводим не первый МЗ, вычисляем прошлую операцию
            {
                Result = TempResult + Result;
                TempResult = string.Empty;

                if (!OperationSelection)
                {
                    СalculationsCommand.Execute(x);
                    PastValue = CurrentValue;
                }
            }
            else if (_pastOperation != "+" && _pastOperation != "-" && (x == "*" || x == "/"))
            {
                if (!OperationSelection)
                {
                    _result += CurrentValue;
                    CurrentValue = Calculator.Calculation(_result);

                    OperationSelection = true;
                    CalculationSelection = true;

                    Result = string.Empty;
                    PastValue = CurrentValue;
                }
            }
            else if (x == "()")
            {                
                TempResult += CurrentValue + PastOperation;
                Result = string.Empty;
                CurrentValue = "(0";
            }
            else
            {
                TempResult += Result;
                Result = string.Empty;
            }
            
            PastOperation = x;
            OperationSelection = true;
        });

        СalculationsCommand = new RelayCommand(() =>
        {
            
            if (OperationSelection)
            {
                _result += TempResult + CurrentValue + PastOperation + PastValue;
            }
            else
            {   
                if(!string.IsNullOrEmpty(TempResult)) OperationCommand.Execute("+");
                _result += CurrentValue;
            }
            CurrentValue = Calculator.Calculation(_result);
            
            OperationSelection = true;
            CalculationSelection = true;

            Result = string.Empty;
        });
    }

    private string? _tempResult = string.Empty;
    public string? TempResult
    {
        get => _tempResult;
        set
        {
            _tempResult = value;
            OnPropertyChanged();
        }
    }

    private string? _result = string.Empty;
    public string? Result
    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged();
        }
    }

    private string? _pastOperation = string.Empty;
    public string? PastOperation
    {
        get => _pastOperation;
        set
        {
            _pastOperation = value;     
            OnPropertyChanged();

        }
    }

    private string? _pastValue = string.Empty;
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


    public RelayCommand CleanCommand { get; }
    public RelayCommand InversionCommand { get; }
    public RelayCommand<string> InputCommand { get; }
    public RelayCommand<string> OperationCommand { get; }
    public RelayCommand СalculationsCommand { get; }



    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
