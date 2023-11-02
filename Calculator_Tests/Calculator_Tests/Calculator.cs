using Calculator_Parser;
using Newtonsoft.Json.Linq;

namespace EquationProcessing
{
    public class Calculator
    {
        public static string Calculation(string? input)
        {
            Parser _parser;
            _parser = new Parser();
            var result = input;
            if (string.IsNullOrWhiteSpace(result) || result == "0")
            {
                return "0";
            }
            result = _parser.StartParsing(result);

            return result;
        }

        public static string Handler(string? input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.All(c => c == '0'))
            {
                return "0";
            }
            else if(input == "Infinity" || input == "Error")
            {
                return input;
            }
            var handlerInput = input;
            var result = "";
            bool isDot = false;
            List<char> allowedСharacters = new() { '-', '+', '*', '/', '(', ')', 'E' };
            foreach (var symbol in handlerInput)
            {
                if (symbol == '.' || symbol == ',')
                {
                    if (!isDot)
                    {
                        isDot = true;
                        result += ',';
                    }
                }
                else if (char.IsDigit(symbol))
                {
                        result += symbol;
                }
                else if (allowedСharacters.Contains(symbol))
                {
                    isDot = false;
                    result += symbol;
                }
                else
                {
                    isDot = false;
                }
            }
            return result;
        }
    }

}