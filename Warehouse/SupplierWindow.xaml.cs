using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для SupplierWindow.xaml
    /// </summary>
    public partial class SupplierWindow : Window
    {
        private int SupplierId;
        public SupplierWindow(int supplierId)
        {
            InitializeComponent();
            this.SupplierId = supplierId;
            this.Warning.Visibility = Visibility.Hidden;
            if (this.SupplierId != -1)
                FillForms();
        }

        private void FillForms()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/SuppliersList.xml");
            
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            int i = 0;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (i != this.SupplierId)
                {
                    i++;
                    continue;
                }

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Наименование")
                        this.NameBox.Text = childnode.InnerText;

                    if (childnode.Name == "Продукция")
                        this.TypeBox.Text = childnode.InnerText;
                    
                    if (childnode.Name == "E-mail")
                        this.EmailBox.Text = childnode.InnerText;
                }
                break;
            }
        }

        private void EditNode(Supplier supplier)
        {
            var xmlDoc = XDocument.Load("Data/SuppliersList.xml");

            // Редактируем выбранный элемент
            XElement node = xmlDoc.Root.Elements().ElementAt(this.SupplierId);
            node.Element("Наименование").Value = supplier.Name;
            node.Element("Продукция").Value = supplier.ProdType;
            node.Element("E-mail").Value = supplier.Email;

            xmlDoc.Save("Data/SuppliersList.xml");
        }

        private void AddNode(Supplier supplier)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/SuppliersList.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            // создаем новый элемент 
            XmlElement userElem = xDoc.CreateElement("Поставщик");
            XmlElement companyElem = xDoc.CreateElement("Наименование");
            XmlElement typeElem = xDoc.CreateElement("Продукция");
            XmlElement emailElem = xDoc.CreateElement("E-mail");

            // создаем текстовые значения для элементов
            XmlText companyText = xDoc.CreateTextNode(supplier.Name);
            XmlText typeText = xDoc.CreateTextNode(supplier.ProdType);
            XmlText emailText = xDoc.CreateTextNode(supplier.Email);

            //добавляем текст в узлы
            companyElem.AppendChild(companyText);
            typeElem.AppendChild(typeText);
            emailElem.AppendChild(emailText);

            // добавляем дочерние узлы
            userElem.AppendChild(companyElem);
            userElem.AppendChild(typeElem);
            userElem.AppendChild(emailElem);
            xRoot.AppendChild(userElem);
            xDoc.Save("Data/SuppliersList.xml");
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var name = this.NameBox.Text;
            var type = this.TypeBox.Text;
            var email = this.EmailBox.Text;            

            // Проверка заполнения полей
            if (name == "" || type == "" || email == "")
            {
                this.Warning.Visibility = Visibility.Visible;
                return;
            }

            Supplier supplier = new Supplier(name, type, email);

            if (this.SupplierId != -1)
                EditNode(supplier);
            else
                AddNode(supplier);

            this.DialogResult = true;
        }
    }
}
