using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private int ClientId;
        public ClientWindow(int clientId)
        {
            InitializeComponent();
            this.ClientId = clientId;
            this.Warning.Visibility = Visibility.Hidden;
            if (this.ClientId != -1)
                FillForms();
        }

        private void FillForms()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/ClientsList.xml");

            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            int i = 0;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (i != this.ClientId)
                {
                    i++;
                    continue;
                }

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Наименование")
                        this.NameBox.Text = childnode.InnerText;

                    if (childnode.Name == "Телефон")
                        this.PhoneBox.Text = childnode.InnerText;

                    if (childnode.Name == "E-mail")
                        this.EmailBox.Text = childnode.InnerText;
                }
            }
        }

        private void EditNode(Client client)
        {
            var xmlDoc = XDocument.Load("Data/ClientsList.xml");

            // Редактируем выбранный элемент
            XElement node = xmlDoc.Root.Elements().ElementAt(this.ClientId);
            node.Element("Наименование").Value = client.Name;
            node.Element("Телефон").Value = client.Phone;
            node.Element("E-mail").Value = client.Email;

            xmlDoc.Save("Data/clientsList.xml");
        }

        private void AddNode(Client client)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Data/ClientsList.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            // создаем новый элемент 
            XmlElement userElem = xDoc.CreateElement("Клиент");
            XmlElement companyElem = xDoc.CreateElement("Наименование");
            XmlElement phoneElem = xDoc.CreateElement("Телефон");
            XmlElement emailElem = xDoc.CreateElement("E-mail");

            // создаем текстовые значения для элементов
            XmlText companyText = xDoc.CreateTextNode(client.Name);
            XmlText phoneText = xDoc.CreateTextNode(client.Phone);
            XmlText emailText = xDoc.CreateTextNode(client.Email);

            //добавляем текст в узлы
            companyElem.AppendChild(companyText);
            phoneElem.AppendChild(phoneText);
            emailElem.AppendChild(emailText);

            // добавляем дочерние узлы
            userElem.AppendChild(companyElem);
            userElem.AppendChild(phoneElem);
            userElem.AppendChild(emailElem);
            xRoot.AppendChild(userElem);
            xDoc.Save("Data/ClientsList.xml");
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var name = this.NameBox.Text;
            var phone = this.PhoneBox.Text;
            var email = this.EmailBox.Text;

            // Проверка заполнения полей
            if (name == "" || phone == "" || email == "")
            {
                this.Warning.Visibility = Visibility.Visible;
                return;
            }

            Client client = new Client(name, phone, email);

            if (this.ClientId != -1)
                EditNode(client);
            else
                AddNode(client);

            this.DialogResult = true;
        }
    }
}
