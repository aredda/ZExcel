using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zekri_Tool.Components;
using Zekri_Tool.Controllers;
using Zekri_Tool.Models;
using Zekri_Tool.Models.Interfaces;

namespace Zekri_Tool.Windows
{
    public partial class ManageWindow : Window
    {
        private string filePath;
        private FileHandler fileHandler;

        public ObservableCollection<Product> Products = new ObservableCollection<Product>();
        public Description Description
        {
            get { return (Description) cmb_description.SelectedItem; }
        }
        public string Lang
        {
            get { return cmb_lang.SelectedItem.ToString(); }
        }
        public Product SelectedProduct;

        private bool isSaved = false;
        public bool IsSaved
        {
            get { return isSaved; }
            set
            {
                isSaved = value;

                this.btn_save.UpdateBackColor((LinearGradientBrush) TryFindResource(isSaved ? "grd_success" : "grd_danger"));
                this.btn_save.UpdateText(isSaved ? "Save Changes (Saved)" : "Save Changes (Unsaved)");
                this.btn_view.SetEnabled(value);
            }
        }

        private bool isAddedToLogs = false;

        public ManageWindow()
        {
            InitializeComponent();

            // load languanges
            this.cmb_lang.ItemsSource = App.appSettings.Languages;
            this.cmb_lang.SelectedIndex = 0;

            // configure data grid and load products
            this.dg_product.ItemsSource = this.Products;

            // configure events
            this.SizeChanged += (object s, SizeChangedEventArgs args) => {
                int diff = (int) args.NewSize.Height - 768;
                if (diff > 0) txt_description.Height = 135 + diff;  
            };
            this.dg_product.SelectionChanged += DataGridSelectionChanged;
            this.cmb_lang.SelectionChanged += (object s, SelectionChangedEventArgs args) => { FillDescriptions(Lang); };
            this.cmb_description.SelectionChanged += (object s, SelectionChangedEventArgs args) =>
            {
                if (cmb_description.SelectedIndex != -1) 
                    SetRTBText(txt_description, Description.Content);
            };
            this.btn_delete_descr.MouseDown += (object s, MouseButtonEventArgs args) => {
                // Remove selected description
                if (cmb_description.SelectedIndex != -1)
                    App.appSettings.SavedDescriptions.Remove(Description);
                // Reload
                FillDescriptions(Lang);
            };
            this.btn_save_descr.MouseDown += (object s, MouseButtonEventArgs args) => {
                // Save description
                if (GetRTBText(txt_description).Length > 0)
                    App.appSettings.SavedDescriptions.Add(new Description { Language = Lang, Content = GetRTBText(txt_description) });
                // Reload
                FillDescriptions(Lang);
            };
            this.btn_submit.MouseDown += SubmitClick;
            this.btn_delete.MouseDown += DeleteSelectedProduct;
            this.btn_delete.MouseDown += (object s, MouseButtonEventArgs args) => { IsSaved = false; };
            this.btn_cancel.MouseDown += (object s, MouseButtonEventArgs args) => { SelectedProduct = null; SetInsertMode(); };
            this.btn_save.MouseDown += SaveChanges;
            this.btn_view.MouseDown += ViewFile;
            this.txt_keywords.KeyUp += Txt_keywords_KeyDown;

            // initial setup
            this.SetInsertMode();
            this.FillDescriptions(Lang);
            this.IsSaved = false;
            this.isAddedToLogs = false;
        }

        public ManageWindow(string filePath)
            : this()
        {
            this.filePath = filePath;
            this.fileHandler = new FileHandler(filePath, new Product().GetColumns());
            // fill data source
            foreach( List<string> r in this.fileHandler.Read())
            {
                Product product = new Product();
                product.Configure(r);

                Products.Add(product);
            }

            // new conditions
            this.IsSaved = true;
            this.isAddedToLogs = true;
        }

        public void SetInsertMode()
        {
            this.lbl_submit.Content = "Insert a new Product:";
            this.btn_submit.UpdateText("Insert Product");
            this.btn_submit.UpdateIcon(TryFindResource("img_add") as BitmapImage);

            this.btn_cancel.SetEnabled(false);
            this.btn_delete.SetEnabled(false);

            ClearForm();
        }

        public void ClearForm()
        {
            this.txt_title.Text = this.txt_subtitle.Text = "";
            this.txt_description.Document.Blocks.Clear();
            this.txt_keywords.Document.Blocks.Clear();
        }

        public void FillDescriptions(string language)
        {
            language = language.ToLower().Trim();

            cmb_description.ItemsSource = App.appSettings.SavedDescriptions.Where(sd => sd.Language.ToLower().Trim().CompareTo(language) == 0);
            cmb_description.DisplayMemberPath = "PreviewContent";
            cmb_description.SelectedValuePath = "Content";
            cmb_description.SelectedIndex = 0;
        }

        public string GetRTBText(RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text.Trim();
        }

        public void SetRTBText(RichTextBox rtb, string value)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(value);

            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(p);
        }

        public bool IsEmpty(TextBox textBox)
        {
            return textBox.Text.Length == 0;
        }

        public bool IsEmpty(RichTextBox richTextBox)
        {
            return GetRTBText(richTextBox).Length == 0;
        }

        public void SubmitClick(object s, MouseButtonEventArgs args)
        {
            try
            {
                if (IsEmpty(txt_title) || IsEmpty(txt_subtitle) || IsEmpty(txt_description) || IsEmpty(txt_keywords))
                    throw new Exception("All fields are required!");

                if (GetRTBText(txt_keywords).Split(',').Length == 0)
                    throw new Exception("A product should have at least 1 keyword!");

                Product demo = new Product();
                demo.Title = txt_title.Text;
                demo.SubTitle = txt_subtitle.Text;
                demo.Description = Description;
                demo.Keywords = GetRTBText(txt_keywords);

                if (SelectedProduct == null)
                    Products.Add(demo);
                else
                {
                    Products[Products.IndexOf(SelectedProduct)] = demo;

                    SelectedProduct = demo;
                }

                IsSaved = false;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Submitting Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DataGridSelectionChanged(object s, SelectionChangedEventArgs args)
        {
            if (dg_product.SelectedItem != null)
            {
                SelectedProduct = dg_product.SelectedItem as Product;

                this.lbl_submit.Content = "Update a product:";
                this.btn_submit.UpdateText("Update Product");
                this.btn_submit.UpdateIcon(TryFindResource("img_edit") as BitmapImage);

                txt_title.Text = SelectedProduct.Title;
                txt_subtitle.Text = SelectedProduct.SubTitle;
                SetRTBText(txt_description, SelectedProduct.Description.Content);
                SetRTBText(txt_keywords, SelectedProduct.Keywords);

                this.UpdateKeywordCounter();

                this.btn_delete.SetEnabled(true);
                this.btn_cancel.SetEnabled(true);
            }
        }

        public void DeleteSelectedProduct(object s, MouseButtonEventArgs args)
        {
            if (SelectedProduct != null && MessageBox.Show("Are you sure to delete '" + SelectedProduct.Title + "'?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // remove selected
                Products.Remove(SelectedProduct);
                // clean up
                SelectedProduct = null;
                this.SetInsertMode();
            }
        }

        public void SaveChanges(object s, MouseButtonEventArgs args)
        {
            try
            {
                if (filePath == null)
                {
                    // select file
                    SaveFileDialog fd = new SaveFileDialog();
                    fd.Filter = "Excel Files (*.xlsx) | *.xlsx";
                    fd.ShowDialog();
                    // change filepath
                    this.filePath = fd.FileName;
                }
                // initializes the file handler and save changes
                if (this.fileHandler == null)
                    this.fileHandler = new FileHandler(this.filePath, Products.ToList<IParsable>());
                // change data source
                this.fileHandler.Rows = Products.ToList<IParsable>();
                // save
                this.fileHandler.Save();
                // mark as saved
                IsSaved = true;
                // add to logs
                if (!isAddedToLogs)
                {
                    App.appSettings.Logs.Add(new Log { FullPath = this.filePath, Time = DateTime.Now });

                    isAddedToLogs = true;
                }
            }
            catch (Exception x)
            {
                // reset
                this.filePath = null;

                MessageBox.Show(x.Message, "Saving Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ViewFile(object s, MouseButtonEventArgs args)
        {
            if (File.Exists(this.filePath))
                this.fileHandler.Show();
        }

        public void UpdateKeywordCounter()
        {
            string[] keywords = GetRTBText(txt_keywords).Split(',');

            int count = keywords.Length;

            if (keywords[count - 1] == "")
                count--;

            lbl_kwcount.Content = "Keyword Count: " + (GetRTBText(txt_keywords).Length == 0 ? 0 : count);
            lbl_kwcount.Foreground = TryFindResource(GetRTBText(txt_keywords).Length == 0 ? "clr_danger" : "clr_main") as SolidColorBrush;
        }

        private void Txt_keywords_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateKeywordCounter();
        }
    }
}
