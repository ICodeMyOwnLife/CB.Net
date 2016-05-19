using System.IO;


namespace CB.Net.Socket
{
    public class NetFileInfo
    {
        #region  Constructors & Destructor
        public NetFileInfo() { }

        public NetFileInfo(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            FileName = fileInfo.Name;
            FileSize = fileInfo.Length;
        }
        #endregion


        #region  Properties & Indexers
        public string FileName { get; set; }
        public long FileSize { get; set; }
        #endregion
    }
}