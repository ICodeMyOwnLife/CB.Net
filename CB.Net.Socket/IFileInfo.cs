namespace CB.Net.Socket
{
    public interface IFileInfo
    {
        #region Abstract
        string FileName { get; set; }
        long FileSize { get; set; }
        #endregion
    }
}