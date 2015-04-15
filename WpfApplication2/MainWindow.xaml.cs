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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DirectoryHandler LeftDirectoryHandler;
        private readonly DirectoryHandler RightDirectoryHandler;

        public MainWindow()
        {
            InitializeComponent();

            foreach (var CurrentDrive in DriveInfo.GetDrives())
            {
                if (CurrentDrive.IsReady)
                {
                    cbDriveLeft.Items.Add(String.Format("{0} [{1}]", CurrentDrive.Name, CurrentDrive.VolumeLabel));
                    cbDriveRight.Items.Add(String.Format("{0} [{1}]", CurrentDrive.Name, CurrentDrive.VolumeLabel));
                }
            }

            if (cbDriveLeft.Items.Count == 0)
            {
                MessageBox.Show("Nincs meghajtó a rendszerben.");
                Environment.Exit(1);
            }

            cbDriveLeft.SelectedIndex = 0;
            cbDriveRight.SelectedIndex = 0;

            this.LeftDirectoryHandler = new DirectoryHandler(DriveInfo.GetDrives()[0].RootDirectory.FullName);
            this.RightDirectoryHandler = new DirectoryHandler(DriveInfo.GetDrives()[0].RootDirectory.FullName);

            this.LeftDirectoryHandler.PathChanged += (o, e) => this.UpdateWindow(e, lwLeftWindow);
            this.RightDirectoryHandler.PathChanged += (o, e) => this.UpdateWindow(e, lwRightWindow);

            this.LeftDirectoryHandler.Update();
            this.RightDirectoryHandler.Update();
        }

        private void UpdateWindow(List<FileSystemInfo> Contents, ListView Target)
        {
            Target.Items.Clear();

            foreach (var Current in Contents)
            {
                if (Current is DirectoryInfo)
                {
                    var x = new
                    {
                        Name = Current.Name,
                        Ext = "",
                        Size = "<DIR>",
                        LastAccess = Current.LastAccessTime.ToString()
                    };

                    Target.Items.Add(x);
                }
            }
        }
    }
}
