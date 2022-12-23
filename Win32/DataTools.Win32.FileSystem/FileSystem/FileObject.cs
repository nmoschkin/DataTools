using DataTools.FileSystem;
using DataTools.Shell.Native;
using DataTools.Win32;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Desktop
{
    public class FileObject : ShellObject
    {
        private SystemFileType fileType;
        public FileObject(string parsingName) : base(parsingName, parsingName.Contains("::"), true, StandardIcons.Icon48)
        {
        }

        public FileObject(string parsingName, bool initialize) : base(parsingName, parsingName.Contains("::"), initialize, StandardIcons.Icon48)
        {
        }

        public FileObject(string parsingName, bool special, bool initialize, StandardIcons iconSize) : base(parsingName, special, initialize, iconSize)
        {
        }

        public override bool IsFolder => false;

        public override long Size => FileTools.GetFileSize(ParsingName);

        public string Directory => Path.GetDirectoryName(ParsingName);


        /// <summary>
        /// Get the full ParsingName of the file.
        /// </summary>
        /// <returns></returns>
        public string Filename
        {
            get
            {
                return ParsingName;
            }

            internal set
            {
                if (ParsingName is object)
                {
                    if (!FileTools.MoveFile(ParsingName, value))
                    {
                        throw new AccessViolationException("Unable to rename/move file.");
                    }
                }
                else if (!File.Exists(value))
                {
                    throw new FileNotFoundException("File Not Found: " + Filename);
                }

                ParsingName = value;
                Refresh();
            }
        }

        /// <summary>
        /// Returns the file type description
        /// </summary>
        /// <returns></returns>
        public string FileType
        {
            get
            {
                if (fileType is null) return "Unknown";
                return fileType.Description;
            }
        }

        /// <summary>
        /// Returns the file type icon
        /// </summary>
        /// <returns></returns>
        public Icon FileTypeIcon
        {
            get
            {
                if (fileType == null) return null;
                return fileType.DefaultIcon;
            }
        }

        /// <summary>
        /// Returns the WPF-compatible file type icon image
        /// </summary>
        /// <returns></returns>
        public Bitmap FileTypeIconImage
        {
            get
            {
                if (fileType == null) return null;
                return fileType.DefaultImage;
            }
        }

        /// <summary>
        /// Returns a Windows Forms compatible icon for the file
        /// </summary>
        /// <returns></returns>
        public override Icon Icon
        {
            get
            {
                if (IsSpecial || (Parent != null && Parent.IsSpecial))
                {
                    if (base.Icon != null)
                    {
                        int? argiIndex = null;
                        base.Icon = Resources.GetFileIcon(ParsingName, StandardToSystem(IconSize), iIndex: ref argiIndex);
                    }
                }

                if (base.Icon != null)
                {
                    return base.Icon;
                }
                else
                {
                    return FileTypeIcon;
                }
            }
            protected set
            {
                base.Icon = value;
            }
        }

        /// <summary>
        /// Returns an image for the file
        /// </summary>
        /// <returns></returns>
        public override Bitmap IconImage
        {
            get
            {
                if (IsSpecial || (Parent != null && Parent.IsSpecial))
                {
                    if (base.IconImage != null)
                    {
                        int? idx = 0;
                        base.IconImage = Resources.IconToTransparentBitmap(Resources.GetFileIcon(ParsingName, StandardToSystem(IconSize), ref idx));
                    }
                }

                if (base.IconImage != null)
                {
                    return base.IconImage;
                }
                else
                {
                    return FileTypeIconImage;
                }
            }
            protected set
            {
                base.IconImage = value;
            }
        }

        /// <summary>
        /// Return the file type object.
        /// </summary>
        /// <returns></returns>
        public SystemFileType TypeObject
        {
            get
            {
                return fileType;
            }
        }

        public override void Refresh(StandardIcons? iconSize = null)
        {
            base.Refresh(iconSize);

            fileType = SystemFileType.FromExtension(Path.GetExtension(ParsingName), size: IconSize);

            OnPropertyChanged(nameof(Filename));
            OnPropertyChanged(nameof(Directory));
            OnPropertyChanged(nameof(FileType));
            OnPropertyChanged(nameof(FileTypeIcon));
            OnPropertyChanged(nameof(FileTypeIconImage));
            OnPropertyChanged(nameof(TypeObject));
            OnPropertyChanged(nameof(Size));
        }

        public override string ToString()
        {
            return DisplayName ?? Filename;
        }
    }
}
