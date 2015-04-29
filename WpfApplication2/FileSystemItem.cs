using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApplication2 {
	class FileSystemItem {
		public string Name;
		public string Extension;
		public string Size;
		public string LastModified;

		public FileSystemInfo Contents;

		public FileSystemItem(FileInfo fi) {
			Name = fi.Name;
			Extension = fi.Extension;
			Size = fi.Length.ToString();
			LastModified = fi.LastAccessTime.ToString();
			Contents = fi;
		}

		public FileSystemItem(DirectoryInfo di) {
			Name = di.Name;
			Extension = String.Empty;
			Size = "<DIR>";
			LastModified = di.LastAccessTime.ToString();
			Contents = di;
		}
	}
}
