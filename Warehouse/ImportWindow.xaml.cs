using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Serialization;

namespace Warehouse
{
    /// <summary>
    /// Логика взаимодействия для ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private int ImportId;
        private int ChangeType;
        private string filename;
        public ImportWindow(int importId, int changeType)
        {
            InitializeComponent();
            this.ImportId = importId;
            this.ChangeType = changeType;
            this.Warning.Visibility = Visibility.Hidden;

            if (changeType == 1)
            {
                this.Title = "Приход товара";
                this.Partner.Content = "Поставщик";
                this.filename = "Data/ImportList.xml";
            }
            else
            {
                this.Title = "Отгрузка товара";
                this.Partner.Content = "Клиент";
                this.filename = "Data/ExportList.xml";
            }

            // Заполняем списки товаров и клиентов/поставщиков 
            FillComboBox(this.BalanceProducts, 0);
            FillComboBox(this.PartnersBox, changeType);

            if (this.ImportId != -1)
                FillForms();
        }

        private void FillForms()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);

            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            int i = 0;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (i != this.ImportId)
                {
                    i++;
                    continue;
                }

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Наименование")
                        this.BalanceProducts.SelectedItem = childnode.InnerText;

                    if (childnode.Name == "Кол-во")
                        this.AmountBox.Text = childnode.InnerText;
                   
                    if (childnode.Name == "Поставщик" || childnode.Name == "Клиент")
                        this.PartnersBox.SelectedItem = childnode.InnerText;
                }
                break;
            }
        }

        private void EditNode(BalanceChange import)
        {
            var xmlDoc = XDocument.Load(this.filename);

            // Редактируем выбранный элемент
            XElement node = xmlDoc.Root.Elements().ElementAt(this.ImportId);
            node.Element("Наименование").Value = import.Product;
            node.Element("Поставщик").Value = import.Supplier;
            node.Element("Клиент").Value = import.Supplier;
            node.Element("Кол-во").Value = import.Amount.ToString();

            xmlDoc.Save(this.filename);
        }

        private void AddNode(BalanceChange import)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this.filename);
            XmlElement xRoot = xDoc.DocumentElement;

            // создаем новый элемент 
            XmlElement prodElem = xDoc.CreateElement("Товар");
            XmlElement nameElem = xDoc.CreateElement("Наименование");
            XmlElement supplierElem;           
            if (import.Type == 1)
            {
                supplierElem = xDoc.CreateElement("Поставщик");
            }
            else
            {
                supplierElem = xDoc.CreateElement("Клиент");
            }
            XmlElement amountElem = xDoc.CreateElement("Кол-во");

            // создаем текстовые значения для элементов
            XmlText nameText = xDoc.CreateTextNode(import.Product);
            XmlText supplierText = xDoc.CreateTextNode(import.Supplier);
            XmlText amountText = xDoc.CreateTextNode(import.Amount.ToString());

            //добавляем текст в узлы
            nameElem.AppendChild(nameText);
            supplierElem.AppendChild(supplierText);
            amountElem.AppendChild(amountText);

            // добавляем дочерние узлы
            prodElem.AppendChild(nameElem);
            prodElem.AppendChild(supplierElem);
            prodElem.AppendChild(amountElem);
            xRoot.AppendChild(prodElem);
            xDoc.Save(this.filename);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
                       
            // Проверка заполнения полей
            if (BalanceProducts.SelectedIndex == -1 || PartnersBox.SelectedIndex == -1 || this.AmountBox.Text == "")
            {
                this.Warning.Visibility = Visibility.Visible;
                return;
            }

            int amount = Int32.Parse(this.AmountBox.Text);
            var product = BalanceProducts.SelectedItem.ToString();
            var supplier = PartnersBox.SelectedItem.ToString();            

            BalanceChange import = new BalanceChange(product, supplier, amount, this.ChangeType);

            // обновляем остаток на складе при наличии места
            if (!ChangeBalance(import))
                return;

            if (this.ImportId != -1)
                EditNode(import);
            else
                AddNode(import);

            this.DialogResult = true;
        }

        // Только цифры для кол-ва
        private void AmountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void FillComboBox(ComboBox comboBox, int type)
        {
            XDocument xmlDoc = new XDocument();
            if (comboBox.Name == "PartnersBox")
            {
                if (type == 1)
                    xmlDoc = XDocument.Load("Data/SuppliersList.xml");
                else
                    xmlDoc = XDocument.Load("Data/ClientsList.xml");
            }
            else
            {
                xmlDoc = XDocument.Load("Data/BalanceList.xml");
            }
       
            var nodes = xmlDoc.Root.Elements();
            foreach (var node in nodes)
            {
                comboBox.Items.Add(node.Element("Наименование").Value);
            }
        }

        private bool ChangeBalance(BalanceChange change)
        {
            var xmlDoc = XDocument.Load("Data/BalanceList.xml");

            // Редактируем выбранный элемент
            foreach(var node in xmlDoc.Root.Elements())
            {
                if (node.Element("Наименование").Value == change.Product) {
                    int balanceAmount = Int32.Parse(node.Element("Кол-во").Value);
                    int balanceCapacity = Int32.Parse(node.Element("Вместимость").Value);
                    // При загрузке
                    if (change.Type == 1)
                    {
                        if (change.Amount + balanceAmount <= balanceCapacity)
                        {
                            node.Element("Кол-во").Value = (change.Amount + Int32.Parse(node.Element("Кол-во").Value)).ToString();
                        }
                        else
                        {
                            MessageBox.Show("Лимит склада по товару превышен");
                            return false;
                        }
                    }
                    // При отгрузке
                    else
                    {
                        if (balanceAmount - change.Amount >= 0)
                        {
                            node.Element("Кол-во").Value = (Int32.Parse(node.Element("Кол-во").Value) - change.Amount).ToString();
                        }
                        else
                        {
                            MessageBox.Show("На складе недостаточно товара");
                            return false;
                        }
                    }
                    
                }
            }
 
            xmlDoc.Save("Data/BalanceList.xml");
            return true;
        }
    }
}
