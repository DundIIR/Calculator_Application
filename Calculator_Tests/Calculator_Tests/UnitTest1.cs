using Xunit;
using FluentAssertions;
using System.ComponentModel;
using FluentAssertions.Equivalency;
using EquationProcessing;
using Calculator_Parser;

namespace Calculator_Tests
{
    public class Tests
    {
        private Calculator _calculator;
        private Parser _parser;

        public Tests()
        {
            _calculator = new Calculator();
            _parser = new Parser();
        }

        // Calculation
        [Fact]
        public void Calculation_ShouldReturnNumberZero_IfSourceEmpty()
        {
            var result = Calculator.Calculation(string.Empty);
            result.Should().Be("0");
        }


        // Handler
        [Fact]
        public void Handler_ShouldReturnNotSpace_IfSourceСontainsSpaces()
        {
            var result = Calculator.Handler("   123 , 123 + ( 123 )  ");
            result.Should().Be("123,123+(123)");
        }

        [Fact]
        public void Handler_ShouldReturnNotDots_IfSourceСontainsDots()
        {
            var result = Calculator.Handler("123.123");
            result.Should().Be("123,123");
        }

        [Fact]
        public void Handler_ShouldReturnOneDotInNumber_IfSourceСontainsMultipleDotsInNumber()
        {
            var result = Calculator.Handler("123.123.456 + 123");
            result.Should().Be("123,123456+123");
        }

        [Fact]
        public void Handler_ShouldReturnNotLetters_IfSourceСontainsLetters()
        {
            var result = Calculator.Handler("123.1asd23 asd+ 123asd");
            result.Should().Be("123,123+123");
        }



        // Parser
        [Fact]
        public void Parser_ShouldReturnZero_IfSourceContainsEmptyString()
        {
            var result = _parser.StartParsing("");
            result.Should().Be("0");
        }

        [Fact]
        public void Parser_ShouldReturnCurrentNumber_IfSourceContainsOneNumber()
        {
            var result = _parser.StartParsing("1");
            result.Should().Be("1");
        }

        [Theory]
        [InlineData("2+", "4")]
        [InlineData("2-", "0")]
        [InlineData("2*", "4")]
        [InlineData("2/", "1")]
        [InlineData("+", "0")]
        [InlineData("*", "0")]
        [InlineData(" -  *", "0")]
        [InlineData("/", "Infinity")]
        [InlineData("1000000000000000000000000000000", "1E+30")]
        [InlineData("1E+20+1E+20", "2E+20")]



        public void Parser_ShouldReturnDivisionOnItself_IfTheSourceCodeContainsAnUnfinishedOperation(string input, string expected)
        {
            var result = _parser.StartParsing(input);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1/0", "Infinity")]
        [InlineData("0/0", "Infinity")]
        [InlineData("0/(10-10)", "Infinity")]
        [InlineData("10/(10*0)", "Infinity")]

        public void Parser_ShouldReturnInfinityString_IfSourceContainsDivisionByZero(string input, string expected)
        {
            var result = _parser.StartParsing(input);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("5+5", "10")]
        [InlineData("(5+5)*2", "20")]
        [InlineData("5+5*2", "15")]
        [InlineData("1", "1")]
        

        public void Parser_ShouldReturnResult_IfSourceContainsValidExpression(string input, string expected)
        {
            var result = _parser.StartParsing(input);
            result.Should().Be(expected);
        }


        [Theory]
        [InlineData("2+(0.5)", "2,5")]
        [InlineData("2*2.6", "5,2")]
        [InlineData("123.123+1.1", "124,223")]
        [InlineData("2.2+1.1", "3,3")]
        [InlineData("-0.5+-1*2/0.22*100+1", "-908,59090909")]
        [InlineData("0.5+0.5", "1")]
        [InlineData("0.+0.5", "0,5")]
        [InlineData("(2+2*(3+5))*2", "36")]


        public void Parser_ShouldReturnDoubleNumber_IfSourceWritingDot(string left, string right)
        {
            var result = _parser.StartParsing(left);
            result.Should().Be(right);

        }

        [Theory]
        [InlineData("()", "Error")]
        [InlineData("2+()", "Error")]
        [InlineData("О+1.0", "Error")]
        [InlineData("2*()", "Error")]
        [InlineData("2+(3*5", "Error")]
        [InlineData("2+3*5)", "Error")]
        [InlineData("2+2*(3*5))*2", "Error")]
        public void Parser_ShouldReturnError_If(string left, string right)
        {
            var result = _parser.StartParsing(left);
            result.Should().Be(right);

        }

        [Fact]
        public void Parser_ShouldReturnMaxValue_IfSourceContainsSumMaxValue()
        {
            var m1 = int.MaxValue;
            var m2 = int.MaxValue;
            var result = _parser.StartParsing($"{m1} + {m2}");
            result.Should().Be(m1.ToString());
        }
    }
}
