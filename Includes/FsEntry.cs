using System.IO;

namespace SymLinker.Includes
{
    public class FsEntry
    {
        public string Path { get; set; }
        public FsEntryType Type { get; set; }
        public bool Exists { get; set; }
        public bool IsSymLink { get; set; }


        public FsEntry(string path)
        {
            Path = path;
            if (Directory.Exists(path) || File.Exists(path))
            {
                FileAttributes attr = File.GetAttributes(path);
                Type = ((attr & FileAttributes.Directory) == FileAttributes.Directory) 
                     ? FsEntryType.Directory : FsEntryType.File;
                Exists = true;
                IsSymLink = ((attr & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);
            }
            else
            {
                Type = 0;
                Exists = false;
                IsSymLink = false;
            }
        }


        public static implicit operator FsEntry(string path) => new(path);


        public override string ToString()
        {
            string link = IsSymLink ? "*" : "";
            if (Type == FsEntryType.File) return $"[file{link}] {Path}";
            if (Type == FsEntryType.Directory) return $"[dir{link}] {Path}";
            return $"[???] {Path}";
        }
    }


    public enum FsEntryType
    {
        NotFound = 0,
        File = 1,
        Directory = 2
    }
}