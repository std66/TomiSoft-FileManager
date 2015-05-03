using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApplication2 {
	class FileSystemItem {
		private static string[] Measure = { "B", "KB", "MB", "GB", "TB", "PB" };

		public string Name { get; set; }
		public string Extension { get; set; }
		public string Size { get; set; }
		public string LastModified { get; set; }

		public FileSystemInfo Contents;

		public FileSystemItem(FileInfo fi) {
			Name = fi.Name;
			Extension = fi.Extension;
			Size = this.ConvertBytes(fi.Length);
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

		private string ConvertBytes(long Value) {
			int Index = 0;
			while (Value > 1024) {
				Index++;
				Value /= 1024;
			}

			return String.Format("{0} {1}", Value, FileSystemItem.Measure[Index]);
		}
	}
}
