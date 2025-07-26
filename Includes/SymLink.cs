using System.ComponentModel;
using System.IO;

namespace SymLinker.Includes
{
    public class SymLink
    {
        public FsEntry Source { get; set; }
        public FsEntry Target { get; set; }
        public SymLinkType Type { get; set; }
        public SymValidatedLinkState ValidatedState { get; private set; } = 0;


        public SymLink() : this("", "", 0) { }

        public SymLink(string source, string target, SymLinkType type = 0)
        {
            Source = source;
            Target = target;
            Type = type;
            Validate();
        }


        public SymValidatedLinkState Validate()
        {
            if (Target.Exists == false) return ValidatedState = SymValidatedLinkState.InvalidDestination;
            if (Source.Path == Target.Path) return ValidatedState = SymValidatedLinkState.SamePath;

            bool sourceReal = Source.Exists && Source.IsSymLink == false;
            bool targetReal = Target.Exists && Target.IsSymLink == false;
            if (sourceReal && targetReal) return ValidatedState = SymValidatedLinkState.NotLinkable;

            if (TryResolveSymlink(Source) is FileSystemInfo fsi)
            {
                if (fsi.FullName == Target.Path)
                {
                    return ValidatedState = SymValidatedLinkState.Exists;
                }
                else
                {
                    return ValidatedState = SymValidatedLinkState.WrongDestination;
                }
            }
           
            // New means destination exists, 
            return ValidatedState = SymValidatedLinkState.New;
        }


        static FileSystemInfo? TryResolveSymlink(FsEntry entry)
        {
            FileSystemInfo? fsi = null;
            try
            {
                if (entry.Type == FsEntryType.Directory)
                {
                    fsi = Directory.ResolveLinkTarget(entry.Path, true);
                }
                else
                {
                    fsi = File.ResolveLinkTarget(entry.Path, true);
                }
            }
            catch (Exception ex)
            {
                _ = ex;
            }
            return fsi;
        }



        /// <summary>
        /// Try parse a symlink from just the link path. Returns null if not a symlink.
        /// </summary>
        public static SymLink? TryParse(FsEntry entry)
        {
            if (entry.IsSymLink == false) return null;

            FileSystemInfo? fsi = TryResolveSymlink(entry);
            if (fsi == null) return null;

            string target = fsi.FullName;
            return new SymLink() { Source = entry, Target = target };
        }
    }


    public enum SymLinkType 
    {
        Soft = 0,
        Hard = 1,
        Junction = 2
    }


    public enum SymValidatedLinkState : int
    {
        /// <summary>The state has not yet been tested.</summary>
        [Description("The state has not yet been tested.")]
        Untested = 0,

        /// <summary>This state occurs if the wrong destination is specified.</summary>
        [Description("The specified destination is incorrect for the source link.")]
        WrongDestination = 1,

        /// <summary>The symlink now exists.</summary>
        [Description("The symlink exists.")]
        Exists = 2,

        /// <summary>The path combination can not be a symlink (most commonly because both are real!).</summary>
        [Description("The source combination can not be a symlink (most commonly because both are real!).")]
        NotLinkable = 3,

        /// <summary>The source and destination are the same, no link can be created.</summary>
        [Description("The source and destination are the same, no link can be created.")]
        SamePath = 4,

        /// <summary>The destination is an invalid link target.</summary>
        [Description("The destination is an invalid link target.")]
        InvalidDestination = 5,

        /// <summary>The symlink is new, but can be created.</summary>
        [Description("The symlink is new, but can be created.")]
        New = 6,
    }
}