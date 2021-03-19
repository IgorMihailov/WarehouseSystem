using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Warehouse
{
    /// <summary>
    /// Логика взаимодействия для Product.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private int ProductId;
        public ProductWindow(int productId)
        {
            InitializeComponent();
            this.ProductId = productId;
            this.Warning.Visibility = Visibility.Hidden;
            if (this.ProductId != -1)
                FillForms();
        }

        private void FillForms()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/BalanceList.xml");

            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            int i = 0;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (i != this.ProductId)
                {
                    i++;
                    continue;
                }

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Наименование")
                        this.NameBox.Text = childnode.InnerText;

                    if (childnode.Name == "Вместимость")
                        this.CapacityBox.Text = childnode.InnerText;
                }
                break;
            }
        }

        private void EditNode(Product product)
        {
            var xmlDoc = XDocument.Load("Data/BalanceList.xml");

            // Редактируем выбранный элемент
            XElement node = xmlDoc.Root.Elements().ElementAt(this.ProductId);
            node.Element("Наименование").Value = product.Name;
            node.Element("Вместимость").Value = product.Capacity.ToString();

            xmlDoc.Save("Data/BalanceList.xml");
        }

        private void AddNode(Product product)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/BalanceList.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            // создаем новый элемент 
            XmlElement prodElem = xDoc.CreateElement("Товар");
            XmlElement nameElem = xDoc.CreateElement("Наименование");
            XmlElement amountElem = xDoc.CreateElement("Кол-во");
            XmlElement capacityElem = xDoc.CreateElement("Вместимость");

            // создаем текстовые значения для элементов
            XmlText nameText = xDoc.CreateTextNode(product.Name);
            XmlText amountText = xDoc.CreateTextNode(product.Amount.ToString());
            XmlText capacityText = xDoc.CreateTextNode(product.Capacity.ToString());

            //добавляем текст в узлы
            nameElem.AppendChild(nameText);
            amountElem.AppendChild(amountText);
            capacityElem.AppendChild(capacityText);

            // добавляем дочерние узлы
            prodElem.AppendChild(nameElem);
            prodElem.AppendChild(amountElem);
            prodElem.AppendChild(capacityElem);
            xRoot.AppendChild(prodElem);
            xDoc.Save("Data/BalanceList.xml");
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (this.NameBox.Text == "" || this.CapacityBox.Text == "")
            {
                this.Warning.Visibility = Visibility.Visible;
                return;
            }

            var name = this.NameBox.Text;
            var capacity = Int32.Parse(this.CapacityBox.Text);
            Product product = new Product(name, 0, capacity);

            if (this.ProductId != -1)
                EditNode(product);
            else
                AddNode(product);

            this.DialogResult = true;
        }

        private void CapacityBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }
    }
}
