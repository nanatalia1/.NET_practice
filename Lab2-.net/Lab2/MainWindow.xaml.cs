using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Window = System.Windows.Window;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tree.Items.Add("Treeview");
        }
        public void Open(object sender, RoutedEventArgs e)
        {
           var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
           dlg.ShowDialog();
            DirectoryInfo dir = new DirectoryInfo(dlg.SelectedPath);
            var root = ProcessDirectory(dir);
            tree.Items.Add(root);
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void About(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Lab2 - Natalia Czapla", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        TreeViewItem ProcessFile(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                Tag = file.FullName
            };
            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            item.Selected += new RoutedEventHandler(StatusBarUpdate);
            var menuItem1 = new System.Windows.Controls.MenuItem { Header = "Open" };
            var menuItem2 = new System.Windows.Controls.MenuItem { Header = "Delete" };
            menuItem1.Click += new RoutedEventHandler(fileOpen);
            menuItem2.Click += new RoutedEventHandler(fileDelete);
            item.ContextMenu.Items.Add(menuItem1);
            item.ContextMenu.Items.Add(menuItem2);
          

            return item;
        }

        TreeViewItem ProcessDirectory(DirectoryInfo directory)
        {
            var root = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };

            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                root.Items.Add(ProcessDirectory(subdir));
            }
            foreach (FileInfo file in directory.GetFiles())
            {
                root.Items.Add(ProcessFile(file));
            }

            root.ContextMenu = new System.Windows.Controls.ContextMenu();
            root.Selected += new RoutedEventHandler(StatusBarUpdate);
            var menuItem1 = new System.Windows.Controls.MenuItem { Header = "Create" };
            var menuItem2 = new System.Windows.Controls.MenuItem { Header = "Delete" };
            menuItem1.Click += new RoutedEventHandler(CreateNew);
            menuItem2.Click += new RoutedEventHandler(DeleteDirectory);
            root.ContextMenu.Items.Add(menuItem1);
            root.ContextMenu.Items.Add(menuItem2);
            



            return root;
        }
        
        void DeleteDirectory(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            string path = item.Tag.ToString();
            Directory.Delete(path, true);
            //System.Windows.Controls.MenuItem menuItem=e.Source as System.Windows.Controls.MenuItem;
            //Directory.Delete(item.Tag.ToString());
            TreeViewItem parent = (TreeViewItem)item.Parent;
            parent.Items.Remove(item);

        }

        
        void fileOpen(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            string content = File.ReadAllText((string)item.Tag);
            scroll.Content = new TextBlock() { Text = content };
        }

        void fileDelete(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            //string path = (string)item.Tag;
            File.Delete(item.Tag.ToString());
            TreeViewItem parent = (TreeViewItem)item.Parent;
            parent.Items.Remove(item);


        }
        private void CreateNew(object sender, RoutedEventArgs e)
        {
            TreeViewItem folder = (TreeViewItem)tree.SelectedItem;
            string path = (string)folder.Tag;
            Window1 dialog = new Window1(path);
            dialog.ShowDialog();
            if (dialog.Succeeded())
            {
                if (File.Exists(dialog.GetPath()))
                {
                    FileInfo file = new FileInfo(dialog.GetPath());
                    folder.Items.Add(ProcessFile(file));
                }
                else if (Directory.Exists(dialog.GetPath()))
                {
                    DirectoryInfo dir = new DirectoryInfo(dialog.GetPath());
                    folder.Items.Add(ProcessDirectory(dir));
                }
            }
        }

        private void StatusBarUpdate(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            FileAttributes attributes = File.GetAttributes((string)item.Tag);
            statusDOS.Text = "";
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                statusDOS.Text += 'r';
            }
            else
            {
                statusDOS.Text += '-';
            }
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                statusDOS.Text += 'a';
            }
            else
            {
                statusDOS.Text += '-';
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                statusDOS.Text += 'h';
            }
            else
            {
                statusDOS.Text += '-';
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                statusDOS.Text += 's';
            }
            else
            {
                statusDOS.Text += '-';
            }
        }

    }
    
}
