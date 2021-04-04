using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse
{
    // Паттерн Компоновщик - древовидная структура подразделений организации
    // с начальником и подчиненными
    class Organization
    {
        private List<Head> departments;
        public Organization()
        {
            var mainDepartment = new Head("Иванов В.П.", "Руководитель", 3000);
            mainDepartment.Add(new Worker("Петров С.А.", "Старший разработчик", 2000));
            mainDepartment.Add(new Worker("Смирнов Т.З.", "Младший разработчик", 1500));

            this.departments = new List<Head>();
            this.departments.Add(mainDepartment);
        }

        public void PrintHead()
        {
            foreach (var department in departments)
                department.Print();
        }
    }

    abstract class Employee
    {
        protected string name;
        protected string position;
        protected int salary;

        public Employee(string name, string position, int salary)
        {
            this.name = name;
            this.position = position;
            this.salary = salary;
        }

        public virtual void Add(Employee employee) { }

        public virtual void Remove(Employee employee) { }

        public virtual void Print()
        {
            Console.WriteLine(name);
        }
    }

    class Head : Employee
    {
        private List<Employee> Employees = new List<Employee>();

        public Head(string name, string position, int salary) : base(name, position, salary)
        {
        }

        public override void Add(Employee Employee)
        {
            Employees.Add(Employee);
        }

        public override void Remove(Employee Employee)
        {
            Employees.Remove(Employee);
        }

        public override void Print()
        {
            Console.WriteLine("Начальник: " + name);
            Console.WriteLine("Подчиненные: ");
            for (int i = 0; i < Employees.Count; i++)
            {
                Employees[i].Print();
            }
            Console.WriteLine("");
        }
    }

    class Worker : Employee
    {
        public Worker(string name, string position, int salary) : base(name, position, salary)
        { }
    }
}
