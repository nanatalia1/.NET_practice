using System;
using System.Collections.Generic;
using System.IO;
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

namespace Lab2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        string path;
        string name;
        bool success;
        public Window1(string path)
        {
            InitializeComponent();
            this.path = path;
            success = false;
        }

        void close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        void ok(object sender, RoutedEventArgs e)
        {
            bool isFile = (bool)file.IsChecked;
            bool isDirectory = (bool)directory.IsChecked;
            if (isFile && !Regex.IsMatch(newName.Text, "^[a-zA-Z0-9_~-]{1,8}\\.(php|txt|html)$"))
            {
                System.Windows.MessageBox.Show("Wrong name", "Create", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                name=newName.Text;
                path = path + "\\" + name;
                FileAttributes attributes = FileAttributes.Normal;
                if ((bool)r.IsChecked)
                {
                    attributes |= FileAttributes.ReadOnly;
                }
                if ((bool)a.IsChecked)
                {
                    attributes |= FileAttributes.Archive;
                }
                if ((bool)h.IsChecked)
                {
                    attributes |= FileAttributes.Hidden;
                }
                if ((bool)s.IsChecked)
                {
                    attributes |= FileAttributes.System;
                }
                if (isFile)
                {
                    File.Create(path);
                }
                else if (isDirectory)
                {
                    Directory.CreateDirectory(path);
                }
                File.SetAttributes(path, attributes);
                success = true;
            }
            Close();
        }
        public bool Succeeded()
        {
            return success;
        }

        public string GetPath()
        {
            return path;
        }
    }
}
