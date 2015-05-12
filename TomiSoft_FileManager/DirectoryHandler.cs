using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TomiSoft_FileManager {
	class DirectoryHandler {
		/// <summary>
		/// A jelenlegi teljes útvonalat tárolja. Vigyázni kell arra, hogy mindig
		/// "\" jelre végződjön.
		/// </summary>
		private string rootPath;

		/// <summary>
		/// A könyvtár teljes tartalmát tartalmazza
		/// </summary>
		public readonly ObservableCollection<FileSystemItem> Contents = new ObservableCollection<FileSystemItem>();

		/// <summary>
		/// Ez az esemény akkor váltódik ki, ha az aktuális útvonal megváltozik
		/// </summary>
		public event Action<string> RootPathChanged;

		/// <summary>
		/// A jelenlegi teljes elérési útvonalat adja meg vagy állítja be
		/// </summary>
		public string RootPath {
			get {
				return this.rootPath;
			}

			set {
				if (!Directory.Exists(value))
					throw new IOException("Ilyen könyvtár nem létezik");

				this.rootPath = value;

				if (this.RootPathChanged != null)
					this.RootPathChanged(this.rootPath);

				this.Update();
			}
		}

		/// <summary>
		/// Meghatározza, hogy a gyökérkönyvtárban vagyok-e
		/// </summary>
		public bool IsRoot {
			get {
				string[] Parts = this.rootPath.Split(@"\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				return Parts.Length == 1;
			}
		}

		/// <summary>
		/// Létrehoz egy DirectoryHandler példányt a megadott útvonallal.
		/// </summary>
		/// <param name="RootPath">A kiinduló útvonal</param>
		public DirectoryHandler(string RootPath) {
			if (!Directory.Exists(RootPath))
				throw new IOException("Ilyen könyvtár nem létezik");

			this.rootPath = RootPath;
		}

		/// <summary>
		/// Belép a megadott könyvtárba
		/// </summary>
		/// <param name="Dirname">A könyvtár, amibe be akarunk lépni</param>
		public void OpenDir(string Dirname) {
			this.rootPath += Dirname + "\\";

			if (this.RootPathChanged != null)
				this.RootPathChanged(this.rootPath);

			this.Update();
		}

		/// <summary>
		/// Az előző könyvtárba lép vissza
		/// </summary>
		public void OpenUpDir() {
			if (this.IsRoot)
				return;

			string[] Parts = this.rootPath.Split(@"\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			this.rootPath = String.Join("\\", Parts, 0, Parts.Length - 1) + "\\";

			if (this.RootPathChanged != null)
				this.RootPathChanged(this.rootPath);

			this.Update();
		}

		/// <summary>
		/// Kiolvassa az aktuális könyvtár tartalmát
		/// </summary>
		public void Update() {
			this.Contents.Clear();

			if (!this.IsRoot) {
				this.Contents.Add(new FileSystemItem("[..]", true));
			}

			DirectoryInfo Info = new DirectoryInfo(this.rootPath);

			foreach (DirectoryInfo i in Info.GetDirectories().OrderBy((x) => x.Name))
				this.Contents.Add(new FileSystemItem(i));

			foreach (FileInfo i in Info.GetFiles().OrderBy((x) => x.Name))
				this.Contents.Add(new FileSystemItem(i));
		}
	}
}
