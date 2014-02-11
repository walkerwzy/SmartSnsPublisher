using SmartSnsPublisher.Utility;
using System;
using System.IO;

namespace SmartSnsPublisher.Entity
{
    /// <summary>
    /// 文件元数据。
    /// 可以使用以下几种构造方法：
    /// 本地路径：new FileItem("C:/temp.jpg");
    /// 本地文件：new FileItem(new FileInfo("C:/temp.jpg"));
    /// 字节流：new FileItem("abc.jpg", bytes);
    /// </summary>
    public class FileItem
    {
        private string _fileName;
        private string _mimeType;
        private byte[] _content;
        private readonly FileInfo _fileInfo;

        /// <summary>
        /// 基于本地文件的构造器。
        /// </summary>
        /// <param name="fileInfo">本地文件</param>
        public FileItem(FileInfo fileInfo)
        {
            if (fileInfo == null || !fileInfo.Exists)
            {
                throw new ArgumentException("fileInfo is null or not exists!");
            }
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// 基于本地文件全路径的构造器。
        /// </summary>
        /// <param name="filePath">本地文件全路径</param>
        public FileItem(string filePath)
            : this(new FileInfo(filePath))
        { }

        /// <summary>
        /// 基于文件名和字节流的构造器。
        /// </summary>
        /// <param name="fileName">文件名称（服务端持久化字节流到磁盘时的文件名）</param>
        /// <param name="content">文件字节流</param>
        public FileItem(string fileName, byte[] content)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            if (content == null || content.Length == 0) throw new ArgumentNullException("content");

            _fileName = fileName;
            _content = content;
        }

        /// <summary>
        /// 基于文件名、字节流和媒体类型的构造器。
        /// </summary>
        /// <param name="fileName">文件名（服务端持久化字节流到磁盘时的文件名）</param>
        /// <param name="content">文件字节流</param>
        /// <param name="mimeType">媒体类型</param>
        public FileItem(String fileName, byte[] content, String mimeType)
            : this(fileName, content)
        {
            if (string.IsNullOrEmpty(mimeType)) throw new ArgumentNullException("mimeType");
            _mimeType = mimeType;
        }

        public string GetFileName()
        {
            if (_fileName == null && _fileInfo != null && _fileInfo.Exists)
            {
                _fileName = _fileInfo.FullName;
            }
            return _fileName;
        }

        public string GetMimeType()
        {
            return _mimeType ?? (_mimeType = HelperFileInfo.GetMimeType(GetContent()));
        }

        public byte[] GetContent()
        {
            if (_content == null && _fileInfo != null && _fileInfo.Exists)
            {
                using (Stream fileStream = _fileInfo.OpenRead())
                {
                    _content = new byte[fileStream.Length];
                    fileStream.Read(_content, 0, _content.Length);
                }
            }

            return _content;
        }
    }
}
