using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TomiSoft_FileManager.Dialogs {
	/// <summary>
	/// Ez a dialog a felhasználótól bekér egy string-et.
	/// </summary>
	public partial class StringInputDialog : Window {
		/// <summary>
		/// Megadja a felhasználó által beírt szöveget
		/// </summary>
		public string Value {
			get {
				return this.tbInput.Text;
			}
		}

		/// <summary>
		/// Létrehoz egy új StringInputDialog példányt. Ez a túlterhelés alapértelmezetten üressé teszi a beviteli mezőt
		/// és nem engedi meg az üres inputot.
		/// </summary>
		/// <param name="Title">Az ablak címe</param>
		/// <param name="Message">Az ablak üzenete</param>
		public StringInputDialog(string Title, string Message) : this(Title, Message, "", false) {}

		/// <summary>
		/// Létrehoz egy új StringInputDialog példányt. Ez a túlterhelés nem engedi meg az üres inputot.
		/// </summary>
		/// <param name="Title">Az ablak címe</param>
		/// <param name="Message">Az ablak üzenete</param>
		/// <param name="DefaultValue">A beviteli mező alapértelmezett értéke</param>
		public StringInputDialog(string Title, string Message, string DefaultValue) : this(Title, Message, DefaultValue, false) {}

		/// <summary>
		/// Létrehoz egy új StringInputDialog példányt. Ez a túlterhelés alapértelmezetten üressé teszi a beviteli mezőt.
		/// </summary>
		/// <param name="Title">Az ablak címe</param>
		/// <param name="Message">Az ablak üzenete</param>
		/// <param name="AllowEmpty">Megengedett-e üres input</param>
		public StringInputDialog(string Title, string Message, bool AllowEmpty) : this(Title, Message, "", AllowEmpty) {}

		/// <summary>
		/// Létrehoz egy új StringInputDialog példányt.
		/// </summary>
		/// <param name="Title">Az ablak címe</param>
		/// <param name="Message">Az ablak üzenete</param>
		/// <param name="DefaultValue">A beviteli mező alapértelmezett értéke</param>
		/// <param name="AllowEmpty">Megengedett-e üres input</param>
		public StringInputDialog(string Title, string Message, string DefaultValue, bool AllowEmpty) {
			InitializeComponent();
			this.ResizeMode = System.Windows.ResizeMode.NoResize;
			this.ShowInTaskbar = false;

			this.Title = Title;
			this.lText.Content = Message;
			this.tbInput.Text = DefaultValue;

			this.tbInput.Focus();
			this.tbInput.SelectAll();

			//Ezt csak azért, hogy ha a júzer entert üt, az okézza le az ablakot.
			this.tbInput.PreviewKeyDown += (o, e) => {
				if (e.Key == Key.Enter) {
					ButtonAutomationPeer peer = new ButtonAutomationPeer(this.btnOk);
					IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

					if (invokeProv != null)
						invokeProv.Invoke();
				}
					
			};

			this.btnOk.Click += (o, e) => {
				if (!AllowEmpty && String.IsNullOrWhiteSpace(this.tbInput.Text)) {
					MessageBox.Show("A mező nem lehet üres", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				this.DialogResult = true;
				this.Close();
			};

			this.btnCancel.Click += (o, e) => {
				this.DialogResult = false;
				this.Close();
			};
		}
	}
}
