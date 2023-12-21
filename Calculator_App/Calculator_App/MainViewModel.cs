using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using EquationProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
    private readonly IMemory _memory;

    private bool OperationSelection = false;
    private bool CalculationSelection = false;
    private int OpenBracket = 0;

    public MainViewModel(IMemory memory)
    {
        _calculator = new Calculator();
        _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        OperationHistory = new ObservableCollection<string>(_memory.GetAllExpressions());

        // Добавление в память
        AddToMemoryCommand = new RelayCommand<string>(expression =>
        {
            if (!Valid)
            {
                OnPropertyChanged(nameof(Result));
            }
            else
            {
                if (_memory.Add(expression))
                {
                    OperationHistory.Add(expression);
                    //string temp = "=" + Calculator.Calculation(_result);
                    //_memory.Add(temp);
                    //OperationHistory.Add(temp);
                }
            }

        });

        GetExpression = new RelayCommand<string>(expression =>
        {
            if (PastOperation == "(")
                Result += expression;
            else
                Result = expression;
        });

        // Удаление из памяти
        DeleteToMemoryCommand = new RelayCommand<string>(expression =>
        {
            if(_memory.Delete(expression))
            {
                OperationHistory.Remove(expression);
                Result = "0";
            }
        });

        // Очищаем все значения
        CleanCommand = new RelayCommand(() =>
        {
            PastOperation = Result = string.Empty;
            OperationSelection = CalculationSelection = false;
            OpenBracket = 0;
        });

        // Инвертируем последние число
        InversionCommand = new RelayCommand(() =>
        {
            Regex regex = new Regex(@"[-+]?\b\d+\b");
            MatchCollection matches = regex.Matches(_result);

            // Если найдено хотя бы одно число
            if (matches.Count > 0)
            {
                // Получаем последнее число в строке
                Match lastNumberMatch = matches[matches.Count - 1];
                int number = int.Parse(lastNumberMatch.Value);

                // Меняем знак числа и обновляем строку
                int newNumber = -number;
                string temp = string.Empty;
                if(newNumber >= 0 && (PastOperation == "-" || PastOperation == "+" || PastOperation == "("))
                    temp = "+" + newNumber;
                else
                    temp = newNumber.ToString();
                Result = _result.Remove(lastNumberMatch.Index, lastNumberMatch.Length).Insert(lastNumberMatch.Index, temp);
            }
        });

        // Light version

        InputCommand = new RelayCommand<string>(x =>
        {
            if (PastOperation == ")")
                return;

            if (OperationSelection)  // Вводим значение после ввода операции
            {
                Result += PastOperation;
                OperationSelection = false;
            }
            else if (CalculationSelection)  // Вводим значение после вычисления выражения
            {
                if(x != ",")
                    Result = string.Empty;
                CalculationSelection = false;
            }
            else
            {
                PastOperation = string.Empty;
            }

            Result += x;
        });

        OperationCommand = new RelayCommand<string>(x =>
        {
            if (x == "(" && (OperationSelection || PastOperation == "(" || Result == "0"))
            { 
                if (PastOperation == "(")
                    Result += "(";
                else
                    Result += PastOperation + "(";
                OpenBracket += 1;
                OperationSelection = false;
                PastOperation = "(";
            }
            else if (x == ")" && OpenBracket != 0)
            {

                Result += ")";
                OperationSelection = false;
                OpenBracket -= 1;

                PastOperation = ")";
            }
            else if ("+-*/".Contains(x)) // Вводим операцию
            {
                OperationSelection = true;
                CalculationSelection = false;
                PastOperation = x;
            }

        });

        CalculationsCommand = new RelayCommand(() =>
        {
            if (!Valid)
            {
                OnPropertyChanged(nameof(Result));
            }
            else
            {
                Result = Calculator.Calculation(_result);
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
                    OpenBracket += 1;
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

    private ObservableCollection<string> _operationHistory;
    public ObservableCollection<string> OperationHistory
    {
        get => _operationHistory; 
        set
        {
            if (_operationHistory != value)
            {
                _operationHistory = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _valid = true;
    public bool Valid
    {
        get
        {
            int openingBrackets = _result.Count(c => (c == '('));
            int closingBrackets = _result.Count(c => (c == ')'));
            if (openingBrackets > closingBrackets)
            {
                _errors[nameof(Result)] = "Не хватает закрывающих скобок";
                _valid = false;
            }
            else if (openingBrackets < closingBrackets)
            {
                _errors[nameof(Result)] = "Не хватает открывающих скобок";
                _valid = false;
            }
            else if (!IsExpressionBetweenBrackets(_result) && openingBrackets != 0)
            {
                _errors[nameof(Result)] = "Между скобками не хватает выражения";
                _valid = false;
            }
            else
            {
                _valid = true;
            }
            return _valid;
        }
        set
        {
            _valid = value;
        }
    }

    static bool IsExpressionBetweenBrackets(string expression)
    {
        Regex regex = new Regex(@"\([^()]*\)");
        MatchCollection matches = regex.Matches(expression);

        foreach (Match match in matches)
        {
            if (match.Length > 2)
            {
                return true;
            }
        }

        return false;
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

    private string? _result = "0";
    public string? Result
    {
        get => _result;
        set
        {
            _result = Calculator.Handler(value);

            _result = (_result == "0" || _result.Contains(",") || _result.Contains(".")) ? _result : _result.TrimStart('0');

            _errors[nameof(Result)] = null;

            OnPropertyChanged();
        }
    }

    public RelayCommand CleanCommand { get; }
    public RelayCommand InversionCommand { get; }
    public RelayCommand<string> InputCommand { get; }
    public RelayCommand<string> OperationCommand { get; }
    public RelayCommand CalculationsCommand { get; }
                        

    public virtual RelayCommand<string> AddToMemoryCommand { get; }
    public virtual RelayCommand<string> GetExpression { get; }
    public virtual RelayCommand<string> DeleteToMemoryCommand { get; }


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
