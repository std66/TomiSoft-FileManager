using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WpfApplication2
{
    class DirectoryHandler
    {
        private string rootPath;

		public readonly ObservableCollection<FileSystemItem> Contents = new ObservableCollection<FileSystemItem>();

        public string RootPath
        {
            get
            {
                return rootPath;
            }
        }

        public bool IsRoot
        {
            get
            {
                return this.rootPath.Split('\\').Length == 1;
            }
        }

        public DirectoryHandler(string RootPath)
        {
            if (!Directory.Exists(RootPath))
                throw new IOException("Ilyen könyvtár nem létezik");

            this.rootPath = RootPath;
        }

        public void Update()
        {
			this.Contents.Clear();

			DirectoryInfo Info = new DirectoryInfo(this.rootPath);

			foreach (DirectoryInfo i in Info.GetDirectories().OrderBy((x) => x.Name))
				this.Contents.Add(new FileSystemItem(i));

			foreach (FileInfo i in Info.GetFiles().OrderBy((x) => x.Name))
				this.Contents.Add(new FileSystemItem(i));
        }
    }
}
