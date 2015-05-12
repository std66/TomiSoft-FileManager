using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TomiSoft_FileManager {
	/// <summary>
	/// Információt ad a fájlmásolás állapotáról
	/// </summary>
	struct CopyStatus {
		private long length;
		private long bytesCopied;

		/// <summary>
		/// A másolandó fájl teljes mérete bájtban
		/// </summary>
		public long Length {
			get { return length; }
		}

		/// <summary>
		/// Az átmásolt adatmennyiség bájtban
		/// </summary>
		public long BytesCopied {
			get { return bytesCopied; }
		}

		/// <summary>
		/// Megadja százalékban a másolás állapotát
		/// </summary>
		public double ProgressInPercent {
			get {
				return (double)this.bytesCopied / this.length * 100;
			}
		}

		/// <summary>
		/// Létrehoz egy CopyStatus példányt.
		/// </summary>
		/// <param name="Length">A másolandó adat mérete bájtban</param>
		/// <param name="BytesCopied">Az átmásolt adatmennyiség bájtban</param>
		public CopyStatus(long Length, long BytesCopied) {
			this.length = Length;
			this.bytesCopied = BytesCopied;
		}
	}

	/// <summary>
	/// Ez az osztály az aszinkron fájlmásolást valósítja meg.
	/// </summary>
	class FileCopyManager {
		private string SourceDirectory;
		private string TargetDirectory;
		private string Filename;

		/// <summary>
		/// Létrehoz egy új FileCopyManager példányt.
		/// </summary>
		/// <param name="SourceDirectory">A könyvtár, ami a másolandó fájlt tartalmazza</param>
		/// <param name="TargetDirectory">A könyvtár, ahová a fájlt másolni kell</param>
		/// <param name="Filename">A fájl neve, amit át kell másolni</param>
		public FileCopyManager(string SourceDirectory, string TargetDirectory, string Filename) {
			this.SourceDirectory = SourceDirectory;
			this.TargetDirectory = TargetDirectory;
			this.Filename = Filename;
		}

		/// <summary>
		/// Elindítja a fájl másolását.
		/// </summary>
		/// <param name="ProgressHandler">Egy IProgress-t megvalósító példány, amely segítségével visszajelezhető az állapot</param>
		/// <returns></returns>
		public async Task CopyAsync(IProgress<CopyStatus> ProgressHandler) {
			//Megnyitjuk az olvasandó fájlt és létrehozzuk az írandó fájlt
			using (FileStream Source = File.OpenRead(this.SourceDirectory + this.Filename))
			using (FileStream Target = File.OpenWrite(this.TargetDirectory + this.Filename)) {
				byte[] Buffer = new byte[5000];
				int BytesRead = 0;

				while ((BytesRead = await Source.ReadAsync(Buffer, 0, Buffer.Length)) > 0) {
					await Target.WriteAsync(Buffer, 0, BytesRead);

					if (ProgressHandler != null)
						ProgressHandler.Report(new CopyStatus(Source.Length, Target.Position));
				}
			}
		}
	}
}
