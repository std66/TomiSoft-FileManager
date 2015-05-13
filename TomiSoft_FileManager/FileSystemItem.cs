using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TomiSoft_FileManager {
	/// <summary>
	/// Egy fájlrendszerbeli elemet ír le
	/// </summary>
	class FileSystemItem {
		/// <summary>
		/// Néhány mértékegység a méret megjelenítéséhez
		/// </summary>
		private static string[] Measure = { "B", "KB", "MB", "GB", "TB", "PB" };

		/// <summary>
		/// Az elem neve
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Az elem kiterjesztése
		/// </summary>
		public string Extension { get; private set; }

		/// <summary>
		/// Az elem mérete
		/// </summary>
		public string Size { get; private set; }

		/// <summary>
		/// Az utolsó hozzáférés ideje
		/// </summary>
		public string LastModified { get; private set; }

		/// <summary>
		/// Könyvtár-e az aktuális elem?
		/// </summary>
		public bool IsDir { get; private set; }

		/// <summary>
		/// Az elemhez tartozó FileSystemInfo példány
		/// </summary>
		public readonly FileSystemInfo Contents;

		/// <summary>
		/// Létrehoz egy FileSystemItem példányt egy meglévő FileInfo példány adatait felhasználva.
		/// </summary>
		/// <param name="fi">A FileInfo példány, amelyből az adatokat nyerjük</param>
		public FileSystemItem(FileInfo fi) {
			Name = fi.Name;

			if (fi.Extension.Length > 1)
				Extension = fi.Extension.Substring(1, fi.Extension.Length - 1).ToLower();
			else
				Extension = String.Empty;

			Size = this.ConvertBytes(fi.Length);
			LastModified = fi.LastAccessTime.ToString();
			Contents = fi;
			IsDir = false;
		}

		/// <summary>
		/// Létrehoz egy FileSystemItem példányt egy meglévő DirectoryInfo példány adatait felhasználva.
		/// </summary>
		/// <param name="fi">A DirectoryInfo példány, amelyből az adatokat nyerjük</param>
		public FileSystemItem(DirectoryInfo di) {
			Name = di.Name;
			Extension = String.Empty;
			Size = "<DIR>";
			LastModified = di.LastAccessTime.ToString();
			Contents = di;
			IsDir = true;
		}

		/// <summary>
		/// Létrehoz egy egyedi FileSystemItem példányt egy megadott név alapján.
		/// Ez semmi más információt nem fog tartalmazni az elemről a neven kívül
		/// </summary>
		/// <param name="Name">Az elem neve</param>
		/// <param name="IsDir">Könyvtárként legyen-e használva?</param>
		public FileSystemItem(string Name, bool IsDir = true) {
			this.Name = Name;
			this.IsDir = IsDir;
		}

		/// <summary>
		/// Byte-okban megadott méretet vált át más mértékegységbe
		/// </summary>
		/// <param name="Value">Az átváltandó méret byte-okban</param>
		/// <returns>Az átváltott adat</returns>
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
