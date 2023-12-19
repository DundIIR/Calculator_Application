using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace Calculator_App
{
    class MemoryLog
    {
        private readonly IMemory _logger;

        public MemoryLog(IMemory logger)
        {
            _logger = logger;
        }
    }

    internal interface IMemory
    {
        void Add(string expression);
        string[] GetAllExpressions();


        void Delete(string expression);
    }

    class RamMemory : IMemory
    {
        private List<string> expressions = new List<string>();


        public void Delete(string expression)
        {
            if (expressions.Contains(expression))
            {
                expressions.Remove(expression);
            }
        }

        public string[] GetAllExpressions()
        {
            expressions.Add("Привет");
            return expressions.ToArray();
        }



        public void Add(string? expression)
        {
            if (expression != null)
                expressions.Add(expression);
        }
    }

    class FileMemory : IMemory
    {
        
        //private string filePath = "History.txt";
        private string filePath = "D:\\VS_Project\\Calculator_Application\\Calculator_App\\Calculator_App\\Resources\\History.txt";
        public void Delete(string expression)
        {
            // Чтение всех выражений из файла
            string[] expressions = GetAllExpressions();

            // Удаление указанного выражения из списка
            List<string> expressionList = new List<string>(expressions);
            expressionList.Remove(expression);

            // Запись обновленного списка выражений обратно в файл
            File.WriteAllLines(filePath, expressionList);
        }

        public string[] GetAllExpressions()
        {
            // Если файл существует, считываем все строки из него
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath);
            }

            return new string[0]; // Если файла нет, возвращаем пустой массив
        }

        public void Add(string expression)
        {
            // Добавляем новое выражение в файл
            File.AppendAllLines(filePath, new[] { expression });
        }
    }

    class DBMemory : IMemory
    {
        public void Delete(string expression)
        {
            throw new NotImplementedException();
        }

        public string[] GetAllExpressions()
        {
            throw new NotImplementedException();
        }

        public void Add(string expression)
        {
            throw new NotImplementedException();
        }
    }
}
