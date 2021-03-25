using System;

namespace SomeWebApp.Core.Entities
{
    public class FileModel
    {
        public FileModel() { }
        public FileModel(string name, string machineName, string path, string extension, int size)
        {
            Name = name;
            MachineName = machineName;
            Path = path;
            Extension = extension;
            Size = size;
            UploadTimestamp = DateTimeOffset.UtcNow;
        }
        public UInt64 ID { get; set; }
        public string Name { get; set; }
        public string MachineName { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public int Size { get; set; }
        public DateTimeOffset UploadTimestamp { get; set; }
    }
}
