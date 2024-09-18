using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Wallofregist.Models;
using Wallofregist.Pages;
using Page = System.Windows.Controls.Page;
using Microsoft.Office.Interop.Excel;
using System.IO;
namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// административная часть приложения
    /// </summary>
    public partial class Admin : Page
    {
        public ObservableCollection<Sotrydniki> Sotrydnikis { get; set; }
        public ObservableCollection<Roli> Roles { get; set; }

        public Admin()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;

        }

        private void LoadData()//получение списка сотрдуников в карточки в виде листа из базы данных
        {
            using (var context = new BazaDan())
            {
                Sotrydnikis = new ObservableCollection<Sotrydniki>(context.Sotrydniki.ToList());
                Roles = new ObservableCollection<Roli>(context.Roli.ToList());

            }
        }

        private void txtSearch_TextChanged_1(object sender, TextChangedEventArgs e)//функция поиска
        {
            string searchText = txtSearch.Text.ToLower();

            ICollectionView view = CollectionViewSource.GetDefaultView(LViewProduct.ItemsSource);

            if (view != null)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    view.Filter = null;
                }
                else
                {
                   
                    view.Filter = item =>
                    {
                        Sotrydniki dataItem = item as Sotrydniki;

                        if (dataItem != null)
                        {
                            string itemName = dataItem.Imya.ToLower();//по имени
                            return itemName.Contains(searchText);
                        }

                        return false;
                    };
                }
            }
            }

        private void ApplySorting()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Sotrydnikis);

            if (view != null)
            {
                ComboBoxItem selectedSortItem = cmbSorting.SelectedItem as ComboBoxItem;

                if (selectedSortItem != null && selectedSortItem.Tag != null)
                {
                    string[] sortParams = selectedSortItem.Tag.ToString().Split(',');
                    string sortProperty = sortParams[0];
                    ListSortDirection sortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), sortParams[1]);

                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(sortProperty, sortDirection));
                }
            }
        }

        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySorting();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddSotr addSotr = new AddSotr();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadData(); 
            }
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                EditWindow editWindow = new EditWindow();
                editWindow.DataContext = LViewProduct.SelectedItem;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadData();
                }
            }
        }
        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Admin newAdminPage = new Admin();
            NavigationService.Navigate(newAdminPage);
        }

        private void PrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument flowDoc = Doc.Document as FlowDocument;
                IDocumentPaginatorSource idp = flowDoc;
                pd.PrintDocument(idp.DocumentPaginator, "Title");
            }
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = excel.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)excel.ActiveSheet;
            ws.Cells[1, 1] = "Имя";
            ws.Cells[1, 2] = "Фамилия";
            ws.Cells[1, 3] = "Отчество";
            ws.Cells[1, 4] = "Номер телефона";
            ws.Cells[1, 5] = "Почта";
            ws.Cells[1, 6] = "Серия паспорта";
            ws.Cells[1, 7] = "Номер паспорта";
            ws.Cells[1, 8] = "Назввание роли";
            int row = 2;
            foreach (var sotrydnik in Sotrydnikis)
            {
                var role = Roles.FirstOrDefault(r => r.IDRoli == sotrydnik.IDRoli);
                if (role != null)
                {
                    ws.Cells[row, 1] = sotrydnik.Imya;
                    ws.Cells[row, 2] = sotrydnik.Familia;
                    ws.Cells[row, 3] = sotrydnik.Otchestvo;
                    ws.Cells[row, 4] = sotrydnik.NumberPhone;
                    ws.Cells[row, 5] = sotrydnik.Pochta;
                    ws.Cells[row, 6] = sotrydnik.SeriaPasporta;
                    ws.Cells[row, 7] = sotrydnik.NomerPasporta;
                    ws.Cells[row, 8] = role.NazvanieRoli;  
                    row++;
                }
            }
            try
            {
                string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Sotrydniki.xlsx");
                wb.SaveAs(fileName);
                excel.Quit();
                MessageBox.Show("Данные экспортированы в Excel.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла Excel: " + ex.Message);
            }
        }
    }
}
