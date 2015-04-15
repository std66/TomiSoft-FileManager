using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    class DirectoryHandler
    {
        private string rootPath;

        public event EventHandler<List<FileSystemInfo>> PathChanged;

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

            this.GetContents();
        }

        public void Update()
        {
            this.GetContents();
        }

        private void GetContents()
        {
            List<FileSystemInfo> Contents = new List<FileSystemInfo>();

            DirectoryInfo Info = new DirectoryInfo(this.rootPath);

            Contents.AddRange(Info.GetDirectories().OrderBy((x) => x.Name));
            Contents.AddRange(Info.GetFiles().OrderBy((x) => x.Name));

            if (this.PathChanged != null)
                this.PathChanged(this, Contents);
        }
    }
}
