using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using Wallofregist.Models;

namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
       
        

        public ObservableCollection<Sotrydniki> Sotrydniki { get; set; }

        public EditWindow()
        {
            InitializeComponent();
            txtHiddenID.Visibility = Visibility.Hidden;
            LoadRoles();
            DataContext = this;
           


        }
         
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtImya.Text = string.Empty;
            txtfamilia.Text = string.Empty;
            txtotchestvo.Text = string.Empty;
            txtNumberPhone.Text = string.Empty;
            txtPochta.Text = string.Empty;
           
            imgPhoto.Source = null;
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
       
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImya.Text) ||
        string.IsNullOrWhiteSpace(txtfamilia.Text) ||
        string.IsNullOrWhiteSpace(txtNumberPhone.Text) ||
        string.IsNullOrWhiteSpace(txtPochta.Text))
            {
                MessageBox.Show("Заполните все обязательные поля перед сохранением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new BazaDan())
                {
                    int idSotrydnika;
                    if (!int.TryParse(txtHiddenID.Text, out idSotrydnika))
                    {
                        MessageBox.Show("Невозможно получить идентификатор сотрудника.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    
                    Sotrydniki existingSotrydnik = context.Sotrydniki.FirstOrDefault(s => s.IDSotrydnika == idSotrydnika);

                    if (existingSotrydnik == null)
                    {
                        MessageBox.Show("Сотрудник не найден.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Изменяем свойства существующего объекта Sotrydniki
                    existingSotrydnik.Imya = txtImya.Text;
                    existingSotrydnik.Familia = txtfamilia.Text;
                    existingSotrydnik.Otchestvo = txtotchestvo.Text;
                    existingSotrydnik.NumberPhone = txtNumberPhone.Text;
                    existingSotrydnik.Pochta = txtPochta.Text;
                    existingSotrydnik.IDRoli = (int)cbRoles.SelectedValue;
                    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                    if (!Validator.TryValidateObject(existingSotrydnik, new ValidationContext(existingSotrydnik), validationResults, true))
                    {
                        StringBuilder errorMessage = new StringBuilder("Введены некорректные данные. Пожалуйста, проверьте введенные значения:\n");

                        foreach (var validationResult in validationResults)
                        {
                            errorMessage.AppendLine(validationResult.ErrorMessage);
                        }

                        MessageBox.Show(errorMessage.ToString(), "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    context.SaveChanges();
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new BazaDan())
                    {
                        Sotrydniki selectedSotrydnik = context.Sotrydniki.Find(((Sotrydniki)DataContext).IDSotrydnika);

                        if (selectedSotrydnik != null)
                        {
                            context.Sotrydniki.Remove(selectedSotrydnik);
                            context.SaveChanges();
                        }
                    }

                    MessageBox.Show("Карточка удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                    DialogResult = true;
                }
            }
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

         

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
        }
        private void LoadRoles()
        {
            try
            {
                using (var context = new BazaDan())
                {
                    var roles = context.Roli.ToList();  
                    cbRoles.ItemsSource = roles;  
                    cbRoles.DisplayMemberPath = "NazvanieRoli";  
                    cbRoles.SelectedValuePath = "IDRoli";  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtPochta_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
    

