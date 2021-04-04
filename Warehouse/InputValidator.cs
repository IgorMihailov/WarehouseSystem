using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Warehouse
{
    // S - класс решает только одну задачу

    //Паттерн Стратегия - под каждый вид проверяего поля свой алгоритм проверки
    public class InputValidator
    {
        public bool CheckInput(string userInput, IText inputType)
        {
            return inputType.Check(userInput);
        }
    }

    // O - интерфейс позволяет при добавлении новых видов проверяемых полей
    // расширять существующий функционал, а не изменять его
    
    // L - производный интерфейс можно использовать как базовый; IEmail не ограничивает IText

    // I - интерфейсы разделены, классы реализуют только необходимые
    
    // D - благодаря использованию интерфейса, нам не важны детали проверки на валидность. 
    // Важно лишь то, что она есть => абстракция не зависит от конкретной реализации
    public interface IText
    {
        bool Check(string userInput);
    }

    public interface IEmail : IText
    {
        bool IsPostServerSupports(string email);
    }

    class NameCheck : IText
    {
        public bool Check(string userInput)
        {
            // Проверка на соответствие шаблону имени
            // Создаем шаблон для слова без цифр 
            string pattern = @"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$";        
            bool result = Regex.Match(userInput, pattern).Success;
         
            return result;
        }
    }

    class PhoneCheck : IText
    {
        public bool Check(string userInput)
        {
            // Проверка на соответсвие номеру
            // Создаем шаблон для слова без цифр 
            string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";
            bool result = Regex.Match(userInput, pattern).Success;

            return result;
        }
    }

    class EmailCheck : IEmail
    {
        private string[] supportList = new string[] { "yandex.ru", "gmail.com" };

        public bool Check(string userInput)
        {
            // Проверка на соответсвие email
            // Создаем шаблон для слова без цифр 
            string pattern = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$";
            bool result = Regex.Match(userInput, pattern).Success;

            if (result)
                return IsPostServerSupports(userInput);
            else
                return false;
        }

        public bool IsPostServerSupports(string email)
        {            
            string postServer = email.Substring(email.IndexOf('@') + 1);

            foreach (var server in supportList)
            {
                if (String.Compare(server, postServer) == 0)
                    return true;
            }
            
            return false;
        }
    }
}
