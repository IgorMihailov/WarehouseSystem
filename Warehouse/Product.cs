using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Warehouse
{
    public class Product
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Capacity { get; set; }

        public Product(string name, int amount, int capacity)
        {
            this.Name = name;
            this.Amount = amount;
            this.Capacity = capacity;
        }

        // Возвращает кол-во всех товаров на складе
        public static int GetProductsNum()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/BalanceList.xml");

            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            return xRoot.ChildNodes.Count;
        }
    }
}
