/* *************************************************
 * DataTools C# Utility Library
 * Copyright (C) 2011-2023 Nathaniel Moschkin
 * All Rights Reserved
 *
 * Licensed Under the Apache 2.0 License
 * *************************************************/

using System.ComponentModel;

namespace DataTools.Desktop
{
    /// <summary>
    /// Represents a Windows Shell/Windows Explorer object
    /// </summary>
    /// <remarks>
    /// A <see cref="IShellObject"/> may be a local or remote file, folder, or other shell entity.<br /><br />
    /// Implementations of this interface can edit file timestamps. Use with caution.<br /><br />
    /// Unless otherwise noted, setting a property on an instance of this interface will change the underlying file, folder, or shell item with immediate effect.
    /// </remarks>
    public interface IShellObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the attributes of the filesystem object
        /// </summary>
        FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets the file creation time.
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the display name of the object.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets the icon image for this shell object.
        /// </summary>
        System.Drawing.Bitmap IconImage { get; }

        /// <summary>
        /// Gets or sets the current requested icon image size.
        /// </summary>
        StandardIcons IconSize { get; set; }

        /// <summary>
        /// Return true if the object is a special folder.
        /// </summary>
        bool IsFolder { get; }

        /// <summary>
        /// Return true if the object is a special item.
        /// </summary>
        bool IsSpecial { get; }

        /// <summary>
        /// Gets or sets the last access time of the file.
        /// </summary>
        DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets the last write time of the file.
        /// </summary>
        DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        IShellFolderObject Parent { get; }

        /// <summary>
        /// Gets the canonical name of a special file.
        /// </summary>
        /// <returns></returns>
        string CanonicalName { get; }

        /// <summary>
        /// Gets the shell parsing name which may be different from the display name.
        /// </summary>
        /// <remarks>
        /// <see cref="ParsingName"/> is used by Windows Explorer, internally, to reference special objects.
        /// </remarks>
        string ParsingName { get; }

        /// <summary>
        /// Gets the size of the object if it is a file.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Try to move or rename the object.
        /// </summary>
        /// <param name="newName">The new absolute location for the file.</param>
        /// <returns></returns>
        /// <remarks>
        /// You must provide this method with an absolute path.
        /// </remarks>
        bool TryMoveObject(string newName);

        /// <summary>
        /// Return true if the object can be moved or renamed.
        /// </summary>
        bool CanMoveObject { get; }

        /// <summary>
        /// Refresh the information in the object instance with live data from Explorer.
        /// </summary>
        /// <param name="iconSize"></param>
        void Refresh(StandardIcons? iconSize = default);
    }


    /// <summary>
    /// Represents a Windows Shell/Windows Explorer folder object
    /// </summary>
    /// <remarks>
    /// A <see cref="IShellFolderObject"/> may be a local or remote folder, or other shell entity that behaves as a folder.<br /><br />
    /// Implementations of this interface can edit file timestamps. Use with caution.<br /><br />
    /// Unless otherwise noted, setting a property on an instance of this interface will change the underlying file, folder, or shell item with immediate effect.
    /// </remarks>
    public interface IShellFolderObject : IShellObject, ICollection<IShellObject>
    {
        /// <summary>
        /// If the object is a folder, gets all children that qualify as folders.
        /// </summary>
        /// <exception cref="NotImplementedException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <remarks>
        /// If the object is not a folder, either a <see cref="NotImplementedException"/> or <see cref="DirectoryNotFoundException"/> could be thrown.
        /// </remarks>
        ICollection<IShellFolderObject> Folders { get; }

        ///// <summary>
        ///// If the object is a folder, gets all children that qualify as files.
        ///// </summary>
        ///// <exception cref="NotImplementedException" />
        ///// <exception cref="DirectoryNotFoundException" />
        ///// <remarks>
        ///// If the object is not a folder, either a <see cref="NotImplementedException"/> or <see cref="DirectoryNotFoundException"/> could be thrown.
        ///// </remarks>
        //IEnumerable<IShellObject> Files { get; }

        /// <summary>
        /// If the object is a folder, gets all children.
        /// </summary>
        /// <exception cref="NotImplementedException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <remarks>
        /// If the object is not a folder, either a <see cref="NotImplementedException"/> or <see cref="DirectoryNotFoundException"/> could be thrown.
        /// </remarks>
        ICollection<IShellObject> Children { get; }

        /// <summary>
        /// Gets or sets a value that determines whether the child items are loaded only upon enumeration (and not upon initialization)
        /// </summary>
        bool IsLazyLoad { get; set; }
    }
}