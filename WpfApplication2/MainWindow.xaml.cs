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
		/// <summary>
		/// A bal oldali ablak kezelője
		/// </summary>
        private readonly DirectoryHandler LeftDirectoryHandler;

		/// <summary>
		/// A jobb oldali ablak kezelője
		/// </summary>
        private readonly DirectoryHandler RightDirectoryHandler;

		/// <summary>
		/// Meghatározza, hogy a jobb vagy a bal ablak van-e kiválasztva.
		/// Igaz, ha a bal oldali, hamis, ha a jobb oldali.
		/// </summary>
		private bool isLeftWindowActive = true;

		/// <summary>
		/// Megadja, hogy van-e művelet folyamatban.
		/// </summary>
		private bool operationInProgress = false;

		/// <summary>
		/// Megadja az aktív könyvtárkezelőt
		/// </summary>
		private DirectoryHandler ActiveDirectoryHandler {
			get {
				return (this.IsLeftWindowActive) ? this.LeftDirectoryHandler : this.RightDirectoryHandler;
			}
		}

		/// <summary>
		/// Megadja az inaktív könyvtárkezelőt
		/// </summary>
		private DirectoryHandler InactiveDirectoryHandler {
			get {
				return (!this.IsLeftWindowActive) ? this.LeftDirectoryHandler : this.RightDirectoryHandler;
			}
		}

		/// <summary>
		/// Megadja az aktív ListView-t.
		/// </summary>
		private ListView ActiveFileWindow {
			get {
				return (this.IsLeftWindowActive) ? this.lwLeftWindow : this.lwRightWindow;
			}
		}

		/// <summary>
		/// Meghatározza, hogy a jobb vagy a bal ablak van-e kiválasztva.
		/// Igaz, ha a bal oldali, hamis, ha a jobb oldali.
		/// </summary>
		private bool IsLeftWindowActive {
			get {
				return this.isLeftWindowActive;
			}
			set {
				this.isLeftWindowActive = value;

				lLeftPath.Foreground = (IsLeftWindowActive) ? Brushes.Red : Brushes.Black;
				lLeftPath.Background = (IsLeftWindowActive) ? Brushes.PapayaWhip : Brushes.Transparent;

				lRightPath.Foreground = (!IsLeftWindowActive) ? Brushes.Red : Brushes.Black;
				lRightPath.Background = (!IsLeftWindowActive) ? Brushes.PapayaWhip : Brushes.Transparent;
			}
		}

		/// <summary>
		/// A program főképernyőjét hozza létre és a programot kezdőállapotra állítja
		/// </summary>
        public MainWindow()
        {
            InitializeComponent();

			#region Meghajtók betöltése
			//Lekérdezzük az összes meghajtót és beállítjuk a meghajtó-kiválasztó ComboBox-okat
            foreach (var CurrentDrive in DriveInfo.GetDrives())
            {
                if (CurrentDrive.IsReady)
                {
                    cbDriveLeft.Items.Add(String.Format("{0} [{1}]", CurrentDrive.Name, CurrentDrive.VolumeLabel));
                    cbDriveRight.Items.Add(String.Format("{0} [{1}]", CurrentDrive.Name, CurrentDrive.VolumeLabel));
                }
            }

			//Ha nem találtunk meghajtót vagy logikai partíciót...
            if (cbDriveLeft.Items.Count == 0)
            {
				this.ErrorMessage("Nincs meghajtó", "A program nem indítható el, mivel nem található meghajtó vagy\nlogikai partíció a rendszerben.");
                Environment.Exit(1);
            }

			//A legelső meghajtót választjuk ki kiindulásnak
            cbDriveLeft.SelectedIndex = 0;
            cbDriveRight.SelectedIndex = 0;
			#endregion

			//Létrehozzuk a két ablak könyvtárkezelőjét
			this.LeftDirectoryHandler = new DirectoryHandler(DriveInfo.GetDrives()[0].RootDirectory.FullName);
            this.RightDirectoryHandler = new DirectoryHandler(DriveInfo.GetDrives()[0].RootDirectory.FullName);

			//Hozzáadunk némi eseményt, ami frissíti az útvonalat megjelenítő label-eket
			this.LeftDirectoryHandler.RootPathChanged += (RootPath) => this.lLeftPath.Content = RootPath;
			this.RightDirectoryHandler.RootPathChanged += (RootPath) => this.lRightPath.Content = RootPath;

			//Beállítjuk a két ListView adatforrását
			this.lwLeftWindow.DataContext = this.LeftDirectoryHandler.Contents;
			this.lwRightWindow.DataContext = this.RightDirectoryHandler.Contents;

			//Hogy legyen is valami az ablakban, frissítjük a tartalmát
			this.LeftDirectoryHandler.Update();
			this.RightDirectoryHandler.Update();
        }

		/// <summary>
		/// Ez hajtódik végre, ha egyszer kattintottak valamelyik ListView egy elemére
		/// </summary>
		/// <param name="sender">A ListView, amin az esemény kiváltódott</param>
		/// <param name="e">Az esemény paraméterei</param>
		private void ListViewClick(object sender, MouseButtonEventArgs e) {
			ListView lw = sender as ListView;

			this.IsLeftWindowActive = lw.Name == "lwLeftWindow";
		}

		/// <summary>
		/// Ez hajtódik végre, ha valamelyik ablakban duplán kattintottak egy fájlra vagy könyvtárra.
		/// </summary>
		/// <param name="sender">A ListView, amin az esemény kiváltódott</param>
		/// <param name="e">Az esemény paraméterei</param>
		private void ListViewItemDoubleClicked(object sender, MouseButtonEventArgs e) {
			//Megszerezzük az eseményt kiváltó ListView-t és a hozzá tartozó könyvtárkezelőt
			ListView lw = sender as ListView;

			#region Ellenőrzés
			//Ha nem a ListView küldte az eseményt...
			if (lw == null)
				return;

			//Ha nincs fájl kiválasztva...
			if (lw.SelectedItems.Count != 1)
				return;

			//Ha a ListView kiválasztott eleme nem FileSystemItem
			FileSystemItem item = lw.SelectedItem as FileSystemItem;
			if (item == null)
				return;
			#endregion

			//Ha könyvtár lett kiválasztva...
			if (item.IsDir) {
				try {
					//Ami mellesleg a .. könyvtár...
					if (item.Name == "[..]")
						//... akkor egy szintet feljebb lépünk ...
						this.ActiveDirectoryHandler.OpenUpDir();
					else
						//... különben megnyitjuk a könyvtárat
						this.ActiveDirectoryHandler.OpenDir(item.Name);
				}
				catch (UnauthorizedAccessException ex) {
					//Hiba esetén visszalépünk az előző szintre
					this.ActiveDirectoryHandler.OpenUpDir();
					this.ErrorMessage("Jogosultság hiba", String.Format("Nem sikerült megnyitni a következő könyvtárat:\n{0}\n\nRészletes leírás:\n{1}", item.Name, ex.Message));
				}
			}
			else {
				System.Diagnostics.Process.Start(this.ActiveDirectoryHandler.RootPath + item.Name);
			}
		}

		/// <summary>
		/// Akkor váltódik ki, ha a felhasználó egy másik meghajtót választ ki
		/// </summary>
		/// <param name="sender">A ComboBox példány, amelyen az esemény kiváltódott</param>
		/// <param name="e">Az esemény paraméterei</param>
		private void DriveChanged(object sender, SelectionChangedEventArgs e) {
			ComboBox cb = sender as ComboBox;

			#region Ellenőrzés
			//Ha a sender nem ComboBox példány vagy az aktuális könyvtárkezelő még nem létezik
			if (cb == null || this.ActiveDirectoryHandler == null)
				return;

			//Ha a ComboBox-ban nincs kiválasztott elem
			if (cb.SelectedIndex < 0)
				return;
			#endregion

			string item = cb.SelectedItem as string;
			if (item == null)
				return;

			this.IsLeftWindowActive = cb.Name == "cbDriveLeft";

			try {
				this.ActiveDirectoryHandler.RootPath = item.Substring(0, 3);
			}
			catch (IOException ex) {
				this.ErrorMessage("Hiba meghajtóváltás során", ex.Message);
			}
		}

		/// <summary>
		/// Dob egy hibaüzenetet
		/// </summary>
		/// <param name="Title">Az ablak címe</param>
		/// <param name="Message">Az ablak szövege</param>
		private void ErrorMessage(string Title, string Message) {
			MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		/// <summary>
		/// Ez a metódus hívódik meg, ha a júzer fájlt akar másolni
		/// </summary>
		/// <param name="sender">Az eseményt kiváltó nyomógomb</param>
		/// <param name="e">Az esemény paraméterei</param>
		private async void CopyClicked(object sender, RoutedEventArgs e) {
			#region Ellenőrzés
			if (this.operationInProgress) {
				this.ErrorMessage("Hiba", "Nem indíthat új műveletet, amíg az előző nem ért véget.");
				return;
			}

			if (this.ActiveFileWindow.SelectedItems.Count != 1)
				return;

			FileSystemItem item = this.ActiveFileWindow.SelectedItem as FileSystemItem;
			if (item == null)
				return;
			#endregion

			if (item.IsDir) {
				this.ErrorMessage("Lusta voltam", "Bocsi, de ezt nem volt kedvem leprogramozni.");
				return;
			}

			string SourceDirectory = this.ActiveDirectoryHandler.RootPath;
			string TargetDirectory = this.InactiveDirectoryHandler.RootPath;

			FileCopyManager CopyMgr = new FileCopyManager(SourceDirectory, TargetDirectory, item.Name);
			Progress<CopyStatus> Progress = new Progress<CopyStatus>((x) => {
				pbProgress.Maximum = x.Length;
				pbProgress.Value = x.BytesCopied;
				lProgress.Content = String.Format("{0}%", (int)x.ProgressInPercent);
			});

			this.operationInProgress = true;
			await CopyMgr.CopyAsync(Progress);
			this.operationInProgress = false;

			pbProgress.Value = 0;
			lProgress.Content = "0%";
		}
    }
}
