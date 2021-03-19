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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Warehouse
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string currentFile;

        public MainWindow()
        {
            InitializeComponent();
            currentFile = "Data/SuppliersList.xml";
            FillMainTable(currentFile);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile == "Data/SuppliersList.xml")
            {
                var form2 = new SupplierWindow(-1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ClientsList.xml")
            {
                var form2 = new ClientWindow(-1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ImportList.xml")
            {
                var form2 = new ImportWindow(-1, 1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ExportList.xml")
            {
                var form2 = new ImportWindow(-1, -1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/BalanceList.xml")
            {
                var form2 = new ProductWindow(-1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var id = MainTable.SelectedIndex;

            // Если элемент не выбран 
            if (id == -1 || id == MainTable.Items.Count - 1)
                return;

            // Создаем форму для редактирования
            if (currentFile == "Data/SuppliersList.xml")
            {
                var form2 = new SupplierWindow(MainTable.SelectedIndex);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ClientsList.xml")
            {
                var form2 = new ClientWindow(MainTable.SelectedIndex);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ImportList.xml")
            {
                var form2 = new ImportWindow(MainTable.SelectedIndex, 1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/ExportList.xml")
            {
                var form2 = new ImportWindow(MainTable.SelectedIndex, -1);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }

            if (currentFile == "Data/BalanceList.xml")
            {
                var form2 = new ProductWindow(MainTable.SelectedIndex);
                if (form2.ShowDialog() == true)
                    FillMainTable(currentFile);
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var id = MainTable.SelectedIndex;
            
            // Если элемент не выбран 
            if (id == -1 || id == MainTable.Items.Count - 1)
                return;

            // Удаление всех изменений при удалении товара
            if (currentFile == "Data/BalanceList.xml")
                RemoveAllProductChanges(id);

            // Удаление одного изменения и корректировка баланса
            if (currentFile == "Data/ImportList.xml")
            {
                bool isPositive = RemoveSingleProductChange(id, 1);
                if (!isPositive)
                {
                    MessageBox.Show("Недостаточно товара чтобы убрать загруку!");
                    return;
                }
            }


            if (currentFile == "Data/ExportList.xml")
            {
                bool isPositive = RemoveSingleProductChange(id, -1);
                if (!isPositive)
                {
                    MessageBox.Show("Недостаточно товара чтобы убрать отгрузку!");
                    return;
                }
            }


            // Удаляем выбранный элемент
            var xmlDoc = XDocument.Load(currentFile);
            xmlDoc.Root.Nodes().ElementAt(id).Remove();

            xmlDoc.Save(currentFile);
            FillMainTable(currentFile);
        }

        // Переключатели пунктов меню

        private void SupplierTable_Click(object sender, RoutedEventArgs e)
        {
            currentFile = "Data/SuppliersList.xml";
            FillMainTable(currentFile);
        }

        private void ClientsTable_Click(object sender, RoutedEventArgs e)
        {
            currentFile = "Data/ClientsList.xml";
            FillMainTable(currentFile);
        }

        private void ImportTable_Click(object sender, RoutedEventArgs e)
        {
            currentFile = "Data/ImportList.xml";
            FillMainTable(currentFile);
        }

        private void ExportTable_Click(object sender, RoutedEventArgs e)
        {
            currentFile = "Data/ExportList.xml";
            FillMainTable(currentFile);
        }

        private void BalanceTable_Click(object sender, RoutedEventArgs e)
        {
            currentFile = "Data/BalanceList.xml";
            FillMainTable(currentFile);
        }

        // Заполнение таблицы из нужного файла
        private void FillMainTable(string fileName)
        { 
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(fileName);
            if (dataSet.Tables.Count == 0)
                MainTable.ItemsSource = null;
            else
                MainTable.ItemsSource = dataSet.Tables[0].DefaultView;
        }       

        private void RemoveAllProductChanges(int id)
        {
            TextBlock nameCell = MainTable.Columns[0].GetCellContent(MainTable.Items[id]) as TextBlock;
            string productName = nameCell.Text;

            // Удаляем загрузки/отгрузки
            RemoveChangesFromFile("Data/ImportList.xml", productName);
            RemoveChangesFromFile("Data/ExportList.xml", productName);
        }

        private void RemoveChangesFromFile(string fileName, string productName)
        {
            var xmlDoc = XDocument.Load(fileName);
            var newDoc = XDocument.Load(fileName);
            int i = 0;
            foreach (XElement node in xmlDoc.Root.Nodes())
            {
                XElement nameElement = node.Element("Наименование");

                if (nameElement.Value == productName)
                {
                    newDoc.Root.Nodes().ElementAt(i).Remove();
                    i--;
                }
                i++;
            }
            newDoc.Save(fileName);
        }

        private bool RemoveSingleProductChange(int id, int type)
        {
            var xmlDoc = XDocument.Load("Data/BalanceList.xml");

            TextBlock cell = MainTable.Columns[0].GetCellContent(MainTable.Items[id]) as TextBlock;
            string productName = cell.Text;
            cell = MainTable.Columns[2].GetCellContent(MainTable.Items[id]) as TextBlock;
            int value = Int32.Parse(cell.Text);

            foreach (XElement node in xmlDoc.Root.Nodes())
            {
                XElement nameElement = node.Element("Наименование");
                int capacity = Int32.Parse(node.Element("Вместимость").Value);

                if (nameElement.Value == productName)
                {
                    int result = (Int32.Parse(node.Element("Кол-во").Value) - type * value);
                    if (result < 0 && type == 1)
                        return false;
                    
                    if (result > capacity && type == -1)
                        return false;

                    node.Element("Кол-во").Value = result.ToString();
                }
            }
            xmlDoc.Save("Data/BalanceList.xml");
            return true;
        }

    }
}
