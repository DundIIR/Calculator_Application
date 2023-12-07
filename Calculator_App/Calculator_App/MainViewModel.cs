using CommunityToolkit.Mvvm.Input;
using EquationProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace Calculator_App;

class TextBoxSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    { 
        var str = value.ToString() ?? string.Empty;

        
        if (str.Length < 9)
            return 72;
        else if (str.Length < 12)
            return 56;
        else if (str.Length < 16)
            return 40;
        else if(str.Length < 20)
            return 32;
        else
            return 26;  
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    private Calculator _calculator;


    private bool OperationSelection = false;
    private int OpenBracket = 0;
    private bool CalculationSelection = false;

    public MainViewModel()
    {
        _calculator = new Calculator();

        // Очищаем все значения
        CleanCommand = new RelayCommand(() =>
        {
            PastValue = PastOperation = CurrentValue = TempResult = Result = string.Empty;
            OperationSelection = false;
            CalculationSelection = false;
            OpenBracket = 0;
        });

        // Инвертируем текущее значение
        InversionCommand = new RelayCommand(() =>
        {
            CurrentValue = CurrentValue.Length > 0 && CurrentValue[0] != '-' ? '-' + CurrentValue : CurrentValue.Substring(1);
        });


        // Light version
        InputCommand = new RelayCommand<string>(x =>
        {
            if (OperationSelection)
            {

                CurrentValue += PastOperation;
                OperationSelection = false;
            }
            else if (CalculationSelection)
            {
                CurrentValue = string.Empty;
                CalculationSelection = false;
            }

            CurrentValue += x;

        });

        OperationCommand = new RelayCommand<string>(x =>
        {
            if (PastOperation == "()")
                return;
            else if (x == "()")
            {
                if (OperationSelection)
                {
                    CurrentValue += PastOperation + "(";
                    OperationSelection = false;
                    OpenBracket += 1;
                }
                else if(OpenBracket != 0)
                {
                    CurrentValue += ")";
                    OperationSelection = false;
                    OpenBracket -= 1;
                }
            }
            else
            {
                OperationSelection = true;
                CalculationSelection = false;
            }

            PastOperation = x;

        });

        СalculationsCommand = new RelayCommand(() =>
        {
            if (!Valid)
            {
                _errors[nameof(CurrentValue)] = "Не хватает закрывающих скобок";
                OnPropertyChanged(nameof(CurrentValue));
            }
            else
            {
                CurrentValue = Calculator.Calculation(_result);
                CalculationSelection = true;

                PastOperation = string.Empty;
                OperationSelection = false;
            }
        });



        // Professional version
        /*
        InputCommand = new RelayCommand<string>(x =>
        {
            if (OperationSelection) // Выбрана операция
            {
                //if (CurrentValue.Contains("(")) Result = "(";
                CurrentValue = string.Empty;
                _result += PastValue + PastOperation;
                OperationSelection = false;
            }

            CurrentValue += x;
            PastValue = CurrentValue;
        });

        OperationCommand = new RelayCommand<string>(x =>
        {
            if (CalculationSelection) // если было нажато равно
            {
                PastValue = CurrentValue;
                CalculationSelection = false;
            }
            if (!string.IsNullOrEmpty(PastOperation) && (x == "+" || x == "-")) // Вводим не первую операцию +|-, вычисляем прошлую операцию
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
                TempResult += CurrentValue + PastOperation + "(";
                if (OperationSelection)
                {
                    _openBracket += 1;
                    x = "(";
                    _result += CurrentValue;
                    CurrentValue = Calculator.Calculation(_result);

                    OperationSelection = true;
                    CalculationSelection = true;

                    Result = string.Empty;
                    PastValue = CurrentValue;
                }
                else
                {

                }
                Result = string.Empty;
                CurrentValue = "0";
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
                if (!string.IsNullOrEmpty(TempResult)) OperationCommand.Execute("+");
                _result += CurrentValue;
            }

            if (!Valid)
            {
                _errors[nameof(CurrentValue)] = "Не хватает закрывающих скобок";
                OnPropertyChanged(nameof(CurrentValue));
            }
            else
            {
                CurrentValue = Calculator.Calculation(_result);
            }

            OperationSelection = true;
            CalculationSelection = true;

            Result = string.Empty;
        });
        */
    }

    private bool _valid = true;
    public bool Valid
    {
        get
        {
            int openingBrackets = _currentValue.Count(c => (c == '('));
            int closingBrackets = _currentValue.Count(c => (c == ')'));
            if(openingBrackets == closingBrackets) 
                _valid = true;
            else
                _valid = false;
            return _valid;
        }
        set
        {
            _valid = value;
        }
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

    private string _currentValue = "0";
    public string CurrentValue
    {
        get => _currentValue;
        set
        {
            _currentValue = Calculator.Handler(value);

            _currentValue = (_currentValue == "0" || _currentValue.Contains(",") || _currentValue.Contains(".")) ? _currentValue : _currentValue.TrimStart('0');

            OnPropertyChanged();

            _errors[nameof(CurrentValue)] = null;
        }
    }


    public RelayCommand CleanCommand { get; }
    public RelayCommand InversionCommand { get; }
    public RelayCommand<string> InputCommand { get; }
    public RelayCommand<string> OperationCommand { get; }
    public RelayCommand СalculationsCommand { get; }

    private Dictionary<string, string> _errors = new Dictionary<string, string>();
    public string Error
    {
        get
        {
            return string.Join(Environment.NewLine, _errors.Values);
        }
    }

    public string this[string columnName]
    {
        get
        {
            return _errors.TryGetValue(columnName, out var value) ? value : string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
