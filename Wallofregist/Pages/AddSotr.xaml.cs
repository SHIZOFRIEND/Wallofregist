using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
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
using Wallofregist.Models;
using Microsoft.Office.Interop.Word;
using Window = System.Windows.Window;
using System.IO;
using System.Net;
namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddSotr.xaml
    /// код отвечает за добавление сотрудников в базу данных
    /// включает в себя функции отчистки полей, добавления фото, а также валидации
    /// </summary>
    public partial class AddSotr : Window
    {
        private int? IDRoli;
        private string SelectedRoleName;

        public AddSotr()
        {
            InitializeComponent();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)//проверка обязательных полей
        {
         if (string.IsNullOrWhiteSpace(txtImya.Text) ||
         string.IsNullOrWhiteSpace(txtfamilia.Text) ||
         string.IsNullOrWhiteSpace(txtNumberPhone.Text) ||
         string.IsNullOrWhiteSpace(txtseria.Text) ||
         string.IsNullOrWhiteSpace(txtnomerpasporta.Text) ||
         string.IsNullOrWhiteSpace(txtPochta.Text))
            {
                MessageBox.Show("Заполните все обязательные поля перед сохранением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int? idPolzovatelia = null;
            try//блок занесения в базу данных полей TextBox(ов)
            {
                using (var context = new BazaDan())
                {
                    Sotrydniki newSotrydnik = new Sotrydniki
                    {
                        Imya = txtImya.Text,
                        Familia = txtfamilia.Text,
                        Otchestvo = txtotchestvo.Text,
                        NumberPhone = txtNumberPhone.Text,
                        Pochta = txtPochta.Text,
                        IDRoli = (int)IDRoli,
                        IDPolzovateliaDlyaAvtorizacii = idPolzovatelia,
                        SeriaPasporta = txtseria.Text,
                        NomerPasporta = txtnomerpasporta.Text

                    };
                    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();//внедрение валидатора, который выводит сообщения в MessageBox  информацию о неккоректных данных
                    if (!Validator.TryValidateObject(newSotrydnik, new ValidationContext(newSotrydnik), validationResults, true))
                    {
                        StringBuilder errorMessage = new StringBuilder("Введены некорректные данные. Пожалуйста, проверьте введенные значения:\n");

                        foreach (var validationResult in validationResults)
                        {
                            errorMessage.AppendLine(validationResult.ErrorMessage/*дополнения строки ошибки о характере ошибки*/);
                        }

                        MessageBox.Show(errorMessage.ToString(), "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    context.Sotrydniki.Add(newSotrydnik);
                    context.SaveChanges();
                    string seriaPasporta = txtseria.Text;
                    string familia = txtfamilia.Text;
                    string imya = txtImya.Text;
                    string otchestvo = txtotchestvo.Text;
                    string nomerpasporta = txtnomerpasporta.Text;
                    Dogovor(seriaPasporta, familia, imya, otchestvo, nomerpasporta, SelectedRoleName);
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CleanButton_Click(object sender, RoutedEventArgs e)//отчистка полей
        {
            txtImya.Text = string.Empty;
            txtfamilia.Text = string.Empty;
            txtotchestvo.Text = string.Empty;
            txtNumberPhone.Text = string.Empty;
            txtPochta.Text = string.Empty;
            txtnomerpasporta.Text = string.Empty;
            txtseria.Text = string.Empty;

            imgPhoto.Source = null;
        }
        private void Dogovor(string seriaPasporta, string familia, string imya, string otchestvo, string nomerpasporta, string roleName)
        {
            var items = new Dictionary<string, string>()
            {
                {"<gorod>", "Новосибирск"},
                {"<currentdate>", DateTime.Now.ToString("dd.MM.yyyy")},
                {"<olo>", "ООО"},
                {"<number>", "1"},
                {"<comp>", "Egorich"},
                {"<dir>", "Антонов Е.А"},
                {"<sotr>", "Антонов Е.А"},
                {"<address>", "Ипподромская 30"},
                {"<kpp>", "123456789"},
                {"<zap>", "40000 рублей (40 тысяч рублей)" },
                {"<data>", "12"},
                {"<mvd>", "ГУ МВД России по Новосибирску"},
                {"<seria>", seriaPasporta},
                {"<imya>", imya},
                {"<fam>", familia},
                {"<otchwestvo>", otchestvo},
                {"<nomerpasp>", nomerpasporta},
                {"<role>", roleName}
            };
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document wordDoc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                object missing = System.Reflection.Missing.Value;
                string fileName = @"C:\Users\Egor\Desktop\учеба\Wallofregist\Wallofregist\Files\blank.docx";
                if (!File.Exists(fileName))
                {
                    MessageBox.Show("Файл не найден: " + fileName);
                    return;
                }
                wordDoc = wordApp.Documents.Open(fileName, ReadOnly: false, Visible: true);
                foreach (var item in items)
                {
                    object findText = item.Key;
                    object replaceText = item.Value;
                    Range myRange = wordDoc.Content;
                    myRange.Find.ClearFormatting();
                    myRange.Find.Execute(FindText: findText, ReplaceWith: replaceText, Replace: WdReplace.wdReplaceAll);
                }
                string newFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "trudovogo-dogovora.docx");
                wordDoc.SaveAs2(newFilePath);
                MessageBox.Show("Документ успешно сохранен: " + newFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                wordDoc?.Close();
                wordApp?.Quit();
            }
        }
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)//добавление фотографии в разеных форматах
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));// Установка изображения
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb.SelectedItem;
                SelectedRoleName = selectedComboBoxItem.Content.ToString();  
                int roleId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out roleId))
                {
                    IDRoli = roleId;  
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}