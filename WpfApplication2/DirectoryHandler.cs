using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WpfApplication2 {
	class DirectoryHandler {
		private string rootPath;

		public readonly ObservableCollection<FileSystemInfo> Contents = new ObservableCollection<FileSystemInfo>();

		public string RootPath {
			get {
				return rootPath;
			}
		}

		public bool IsRoot {
			get {
				return this.rootPath.Split('\\').Length == 1;
			}
		}

		public DirectoryHandler(string RootPath) {
			if (!Directory.Exists(RootPath))
				throw new IOException("Ilyen könyvtár nem létezik");

			this.rootPath = RootPath;

			this.GetContents();
		}

		public void Update() {
			this.GetContents();
		}

		private void GetContents() {
			this.Contents.Clear();

			DirectoryInfo Info = new DirectoryInfo(this.rootPath);

			foreach (var item in Info.GetDirectories().OrderBy((x) => x.Name)) {
				this.Contents.Add(item);
			}

			foreach (var item in Info.GetFiles().OrderBy((x) => x.Name)) {
				this.Contents.Add(item);
			}
		}
	}
}
