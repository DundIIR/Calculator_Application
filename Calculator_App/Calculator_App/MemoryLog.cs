using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Data.SQLite;
using Dapper;

namespace Calculator_App
{
    internal interface IMemory
    {
        bool Add(string expression);
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

        public bool Add(string? expression)
        {
            if (expression != null && !expressions.Contains(expression))
            {
                expressions.Add(expression);
                return true;
            }
            return false;
        }
    }

    class FileMemory : IMemory
    {
        private readonly string filePath;
        public FileMemory()
        {
            string spath = typeof(DBMemory).Assembly.Location.ToString();
            int index = spath.IndexOf("Calculator_App") + "Calculator_App".Length;
            filePath = spath.Substring(0, index) + "\\Calculator_App\\Resources\\HistoryOperations.txt";
        }

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

        public bool Add(string expression)
        {
            string[] expressions = GetAllExpressions();

            if (!expressions.Contains(expression))
            {
                File.AppendAllLines(filePath, new[] { expression });
                return true;
            }
            return false;
        }
    }

    class DBMemory : IMemory
    {
        private readonly SQLiteConnection connection;
        public DBMemory()
        {

            string spath = typeof(DBMemory).Assembly.Location.ToString();
            int index = spath.IndexOf("Calculator_App") + "Calculator_App".Length;
            string connectionPath = "Data Source=" + spath.Substring(0, index) + "\\db_operation.db;";

            connection = new System.Data.SQLite.SQLiteConnection(connectionPath);
            connection.Open();
            
            var result = connection.Query<Expression>("select * from operations");
        }

        public void Delete(string expression)
        { 
            var result = connection.Execute("DELETE FROM operations WHERE expression = :expression", new { expression });
        }

        public string[] GetAllExpressions()
        {
            var result = connection.Query<Expression>("select * from operations");
            var expressions = result.Select(expr => expr.expression).ToList();
            return expressions.ToArray();
        }

        public bool Add(string expression)
        {
            var existingExpressions = GetAllExpressions();
            if (!existingExpressions.Contains(expression))
            {
                var result = connection.Execute("INSERT INTO operations (expression) VALUES (:expression)", new { expression });
                return true;
            }
            return false;
        }

        ~ DBMemory()
        {
            connection.Close();
        }
    }

    class Expression
    {
        public long Id { get; set; }
        public string expression { get; set; }
    }
}
