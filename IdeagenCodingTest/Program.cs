using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace IdeagenCodingTest
{
    internal class Program
    {
        const string Add = "+";
        const string Subtract = "-";
        const string Multiply = "*";
        const string Divide = "/";
        const string OpenBracket = "(";
        const string CloseBracket = ")";

        static void Main(string[] args)
        {
            var listOfTestCases = new List<string> 
            {
                "1 + 1",
                "2 * 2",
                "1 + 2 + 3",
                "6 / 2",
                "11 + 23",
                "11.1 + 23",
                "1 + 2 * 3",
                "( 11.5 + 15.4 ) + 10.1",
                "23 - ( 29.3 - 12.5 )",
                "( 1 / 2 ) - 1 + 1",
                "10 - ( 2 + 3 * ( 7 - 5 ) )"
            };

            foreach (var item in listOfTestCases)
            {
                Console.WriteLine("*******************************");
                MyCalculation(item);
                Console.WriteLine("*******************************");
            }
            Console.ReadLine();
        }

        private static double MyCalculation(string sum)
        {
            var listOfValues = new Stack<double>();
            var listOfOperators = new Stack<string>();
            var openBracketIndex = new Stack<int> { };
            var latestCombinedExpression = new Stack<string> { };

            // Split the value by spacing
            var splittedValue = sum.Split(' ');

            for (int i = 0; i < splittedValue.Length; i++)
            {
                // Check if its number or arithmetic sign
                var isNumeric = double.TryParse(splittedValue[i], out double x);

                if (splittedValue[i] == OpenBracket)
                {
                    openBracketIndex.Push(i);
                }
                else if (splittedValue[i] == CloseBracket)
                {
                    var latestOpenBracketIndex = openBracketIndex.Pop();

                    // Get all the value after lat occurence of "(" and last occurance of ")"
                    var lastOpenBracketIndex = splittedValue.Skip(latestOpenBracketIndex + 1).Take(i - latestOpenBracketIndex - 1);

                    var subExpression = string.Join(" ", lastOpenBracketIndex);

                    if (latestCombinedExpression.Count > 0)
                    {
                        // Replace the last bracket solution with the calculated answer from top of stack
                        var combination = latestCombinedExpression.Pop();
                        subExpression = subExpression.Replace(combination, listOfValues.Peek().ToString());
                    }
                    
                    latestCombinedExpression.Push("( " + subExpression+ " )");

                    var combinedSubExpression = subExpression.Split(" ");
                    var numberList = combinedSubExpression.Where(x => double.TryParse(x, out _));
                    var operatorList = combinedSubExpression.Where(x => !double.TryParse(x, out _));

                    var total = MyCalculation(subExpression);

                    foreach (var item in numberList)
                    {
                        listOfValues.Pop();
                    }

                    foreach (var item in operatorList)
                    {
                        listOfOperators.Pop();
                    }
                    listOfValues.Push(total);
                }
                else if (!isNumeric)
                {
                    if (splittedValue[i] == Add || splittedValue[i] == Subtract || splittedValue[i] == Multiply || splittedValue[i] == Divide)
                    {
                        if (listOfOperators.Count > 0 && openBracketIndex.Count <= 0 && GetArithmeticPriority(splittedValue[i]) <= GetArithmeticPriority(listOfOperators.Peek()))
                        {
                            double firstValue = listOfValues.Pop();
                            double secondValue = listOfValues.Pop();
                            var total = Operation(secondValue, firstValue, listOfOperators.Pop());
                            listOfValues.Push(total);
                        }
                        listOfOperators.Push(splittedValue[i]);
                    }
                }
                else
                {
                    listOfValues.Push(x);
                }
            }

            if (listOfOperators.Count > 0)
            {
                foreach (var item in listOfOperators)
                {
                    double firstValue = listOfValues.Pop();
                    double secondValue = listOfValues.Pop();
                    var total = Operation(secondValue, firstValue, item);
                    listOfValues.Push(total);
                }
            }

            Console.WriteLine($"{sum} = {listOfValues.Peek()}");
            return listOfValues.Peek();
        }

        private static int GetArithmeticPriority(string sign)
        {
            if (sign == Add || sign == Subtract)
                return 1;

            if (sign == Multiply || sign == Divide)
                return 2;

            return 0;
        }

        private static double Operation(double x, double y, string sign)
        {
            double response = 0;

            switch (sign)
            {
                case "+":
                    response = x + y;
                    break;
                case "-":
                    response = x - y;
                    break;
                case "*":
                    response = x * y;
                    break;
                case "/":
                    response = x / y;
                    break;
                default:
                    throw new Exception("Invalid Expression");
            }
            return response;
        }
    }
}
