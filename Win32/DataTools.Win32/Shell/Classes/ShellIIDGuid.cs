// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeShell
//         Wrappers for native and COM shell interfaces.
//
// Some enum documentation copied from the MSDN (and in some cases, updated).
// Some classes and interfaces were ported from the WindowsAPICodePack.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

//using DataTools.Hardware.MessageResources;
//using DataTools.Hardware;
//using DataTools.Hardware.Native;
using System;

namespace DataTools.Shell.Native
{
    //<HideModuleName>
    //Public Module NativeShell

    // This code is mostly translated from the Windows API Code Pack. I added some IIDs for Windows 8.1

    /// <summary>
    /// Native shell UUID (<see cref="Guid"/>) constants.
    /// </summary>
    public sealed class ShellIIDGuid
    {
        #region Text UUIDs

        /// <summary>
        /// IModalWindow - B4DB1657-70D7-485E-8E3E-6FCB5A5C1802
        /// </summary>
        public const string IModalWindow = "B4DB1657-70D7-485E-8E3E-6FCB5A5C1802";

        /// <summary>
        /// IFileDialog - 42F85136-DB7E-439C-85F1-E4075D135FC8
        /// </summary>
        public const string IFileDialog = "42F85136-DB7E-439C-85F1-E4075D135FC8";

        /// <summary>
        /// IFileOpenDialog - D57C7288-D4AD-4768-BE02-9D969532D960
        /// </summary>
        public const string IFileOpenDialog = "D57C7288-D4AD-4768-BE02-9D969532D960";

        /// <summary>
        /// IFileSaveDialog - 84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB
        /// </summary>
        public const string IFileSaveDialog = "84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB";

        /// <summary>
        /// IFileDialogEvents - 973510DB-7D7F-452B-8975-74A85828D354
        /// </summary>
        public const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";

        /// <summary>
        /// IFileDialogControlEvents - 36116642-D713-4B97-9B83-7484A9D00433
        /// </summary>
        public const string IFileDialogControlEvents = "36116642-D713-4B97-9B83-7484A9D00433";

        /// <summary>
        /// IFileDialogCustomize - E6FDD21A-163F-4975-9C8C-A69F1BA37034
        /// </summary>
        public const string IFileDialogCustomize = "E6FDD21A-163F-4975-9C8C-A69F1BA37034";

        /// <summary>
        /// IShellItem - 43826D1E-E718-42EE-BC55-A1E261C37BFE
        /// </summary>
        public const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";

        /// <summary>
        /// IShellItem2 - 7E9FB0D3-919F-4307-AB2E-9B1860310C93
        /// </summary>
        public const string IShellItem2 = "7E9FB0D3-919F-4307-AB2E-9B1860310C93";

        /// <summary>
        /// IShellItemArray - B63EA76D-1F85-456F-A19C-48159EFA858B
        /// </summary>
        public const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";

        /// <summary>
        /// IShellLibrary - 11A66EFA-382E-451A-9234-1E0E12EF3085
        /// </summary>
        public const string IShellLibrary = "11A66EFA-382E-451A-9234-1E0E12EF3085";

        /// <summary>
        /// IThumbnailCache - F676C15D-596A-4ce2-8234-33996F445DB1
        /// </summary>
        public const string IThumbnailCache = "F676C15D-596A-4ce2-8234-33996F445DB1";

        /// <summary>
        /// ISharedBitmap - 091162a4-bc96-411f-aae8-c5122cd03363
        /// </summary>
        public const string ISharedBitmap = "091162a4-bc96-411f-aae8-c5122cd03363";

        /// <summary>
        /// IShellFolder - 000214E6-0000-0000-C000-000000000046
        /// </summary>
        public const string IShellFolder = "000214E6-0000-0000-C000-000000000046";

        /// <summary>
        /// IShellFolder2 - 93F2F68C-1D1B-11D3-A30E-00C04F79ABD1
        /// </summary>
        public const string IShellFolder2 = "93F2F68C-1D1B-11D3-A30E-00C04F79ABD1";

        /// <summary>
        /// IEnumIDList - 000214F2-0000-0000-C000-000000000046
        /// </summary>
        public const string IEnumIDList = "000214F2-0000-0000-C000-000000000046";

        /// <summary>
        /// IShellLinkW - 000214F9-0000-0000-C000-000000000046
        /// </summary>
        public const string IShellLinkW = "000214F9-0000-0000-C000-000000000046";

        /// <summary>
        /// CShellLink - 00021401-0000-0000-C000-000000000046
        /// </summary>
        public const string CShellLink = "00021401-0000-0000-C000-000000000046";

        /// <summary>
        /// IPropertyStore - 886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99
        /// </summary>
        public const string IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";

        /// <summary>
        /// IPropertyStoreCache - 3017056d-9a91-4e90-937d-746c72abbf4f
        /// </summary>
        public const string IPropertyStoreCache = "3017056d-9a91-4e90-937d-746c72abbf4f";

        /// <summary>
        /// IPropertyDescription - 6F79D558-3E96-4549-A1D1-7D75D2288814
        /// </summary>
        public const string IPropertyDescription = "6F79D558-3E96-4549-A1D1-7D75D2288814";

        /// <summary>
        /// IPropertyDescription2 - 57D2EDED-5062-400E-B107-5DAE79FE57A6
        /// </summary>
        public const string IPropertyDescription2 = "57D2EDED-5062-400E-B107-5DAE79FE57A6";

        /// <summary>
        /// IPropertyDescriptionList - 1F9FC1D0-C39B-4B26-817F-011967D3440E
        /// </summary>
        public const string IPropertyDescriptionList = "1F9FC1D0-C39B-4B26-817F-011967D3440E";

        /// <summary>
        /// IPropertyEnumType - 11E1FBF9-2D56-4A6B-8DB3-7CD193A471F2
        /// </summary>
        public const string IPropertyEnumType = "11E1FBF9-2D56-4A6B-8DB3-7CD193A471F2";

        /// <summary>
        /// IPropertyEnumType2 - 9B6E051C-5DDD-4321-9070-FE2ACB55E794
        /// </summary>
        public const string IPropertyEnumType2 = "9B6E051C-5DDD-4321-9070-FE2ACB55E794";

        /// <summary>
        /// IPropertyEnumTypeList - A99400F4-3D84-4557-94BA-1242FB2CC9A6
        /// </summary>
        public const string IPropertyEnumTypeList = "A99400F4-3D84-4557-94BA-1242FB2CC9A6";

        /// <summary>
        /// IPropertyStoreCapabilities - c8e2d566-186e-4d49-bf41-6909ead56acc
        /// </summary>
        public const string IPropertyStoreCapabilities = "c8e2d566-186e-4d49-bf41-6909ead56acc";

        /// <summary>
        /// ICondition - 0FC988D4-C935-4b97-A973-46282EA175C8
        /// </summary>
        public const string ICondition = "0FC988D4-C935-4b97-A973-46282EA175C8";

        /// <summary>
        /// ISearchFolderItemFactory - a0ffbc28-5482-4366-be27-3e81e78e06c2
        /// </summary>
        public const string ISearchFolderItemFactory = "a0ffbc28-5482-4366-be27-3e81e78e06c2";

        /// <summary>
        /// IConditionFactory - A5EFE073-B16F-474f-9F3E-9F8B497A3E08
        /// </summary>
        public const string IConditionFactory = "A5EFE073-B16F-474f-9F3E-9F8B497A3E08";

        /// <summary>
        /// IRichChunk - 4FDEF69C-DBC9-454e-9910-B34F3C64B510
        /// </summary>
        public const string IRichChunk = "4FDEF69C-DBC9-454e-9910-B34F3C64B510";

        /// <summary>
        /// IPersistStream - 00000109-0000-0000-C000-000000000046
        /// </summary>
        public const string IPersistStream = "00000109-0000-0000-C000-000000000046";

        /// <summary>
        /// IPersist - 0000010c-0000-0000-C000-000000000046
        /// </summary>
        public const string IPersist = "0000010c-0000-0000-C000-000000000046";

        /// <summary>
        /// IEnumUnknown - 00000100-0000-0000-C000-000000000046
        /// </summary>
        public const string IEnumUnknown = "00000100-0000-0000-C000-000000000046";

        /// <summary>
        /// IQuerySolution - D6EBC66B-8921-4193-AFDD-A1789FB7FF57
        /// </summary>
        public const string IQuerySolution = "D6EBC66B-8921-4193-AFDD-A1789FB7FF57";

        /// <summary>
        /// IQueryParser - 2EBDEE67-3505-43f8-9946-EA44ABC8E5B0
        /// </summary>
        public const string IQueryParser = "2EBDEE67-3505-43f8-9946-EA44ABC8E5B0";

        /// <summary>
        /// IQueryParserManager - A879E3C4-AF77-44fb-8F37-EBD1487CF920
        /// </summary>
        public const string IQueryParserManager = "A879E3C4-AF77-44fb-8F37-EBD1487CF920";

        /// <summary>
        /// INotinheritableBitmap - 091162a4-bc96-411f-aae8-c5122cd03363
        /// </summary>
        public const string INotinheritableBitmap = "091162a4-bc96-411f-aae8-c5122cd03363";

        /// <summary>
        /// IShellItemImageFactory - bcc18b79-ba16-442f-80c4-8a59c30c463b
        /// </summary>
        public const string IShellItemImageFactory = "bcc18b79-ba16-442f-80c4-8a59c30c463b";

        /// <summary>
        /// IContextMenu - 000214e4-0000-0000-c000-000000000046
        /// </summary>
        public const string IContextMenu = "000214e4-0000-0000-c000-000000000046";

        /// <summary>
        /// IContextMenu2 - 000214f4-0000-0000-c000-000000000046
        /// </summary>
        public const string IContextMenu2 = "000214f4-0000-0000-c000-000000000046";

        /// <summary>
        /// IContextMenu3 - BCFCE0A0-EC17-11D0-8D10-00A0C90F2719
        /// </summary>
        public const string IContextMenu3 = "BCFCE0A0-EC17-11D0-8D10-00A0C90F2719";

        /// <summary>
        /// IImageList - 46EB5926-582E-4017-9FDF-E8998DAA0950
        /// </summary>
        public const string IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";

        #endregion Text UUIDs

        #region Guid structs

        /// <summary>
        /// IModalWindow - B4DB1657-70D7-485E-8E3E-6FCB5A5C1802
        /// </summary>
        public static readonly Guid IModalWindowUuid = Guid.Parse(IModalWindow);

        /// <summary>
        /// IFileDialog - 42F85136-DB7E-439C-85F1-E4075D135FC8
        /// </summary>
        public static readonly Guid IFileDialogUuid = Guid.Parse(IFileDialog);

        /// <summary>
        /// IFileOpenDialog - D57C7288-D4AD-4768-BE02-9D969532D960
        /// </summary>
        public static readonly Guid IFileOpenDialogUuid = Guid.Parse(IFileOpenDialog);

        /// <summary>
        /// IFileSaveDialog - 84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB
        /// </summary>
        public static readonly Guid IFileSaveDialogUuid = Guid.Parse(IFileSaveDialog);

        /// <summary>
        /// IFileDialogEvents - 973510DB-7D7F-452B-8975-74A85828D354
        /// </summary>
        public static readonly Guid IFileDialogEventsUuid = Guid.Parse(IFileDialogEvents);

        /// <summary>
        /// IFileDialogControlEvents - 36116642-D713-4B97-9B83-7484A9D00433
        /// </summary>
        public static readonly Guid IFileDialogControlEventsUuid = Guid.Parse(IFileDialogControlEvents);

        /// <summary>
        /// IFileDialogCustomize - E6FDD21A-163F-4975-9C8C-A69F1BA37034
        /// </summary>
        public static readonly Guid IFileDialogCustomizeUuid = Guid.Parse(IFileDialogCustomize);

        /// <summary>
        /// IShellItem - 43826D1E-E718-42EE-BC55-A1E261C37BFE
        /// </summary>
        public static readonly Guid IShellItemUuid = Guid.Parse(IShellItem);

        /// <summary>
        /// IShellItem2 - 7E9FB0D3-919F-4307-AB2E-9B1860310C93
        /// </summary>
        public static readonly Guid IShellItem2Uuid = Guid.Parse(IShellItem2);

        /// <summary>
        /// IShellItemArray - B63EA76D-1F85-456F-A19C-48159EFA858B
        /// </summary>
        public static readonly Guid IShellItemArrayUuid = Guid.Parse(IShellItemArray);

        /// <summary>
        /// IShellLibrary - 11A66EFA-382E-451A-9234-1E0E12EF3085
        /// </summary>
        public static readonly Guid IShellLibraryUuid = Guid.Parse(IShellLibrary);

        /// <summary>
        /// IThumbnailCache - F676C15D-596A-4ce2-8234-33996F445DB1
        /// </summary>
        public static readonly Guid IThumbnailCacheUuid = Guid.Parse(IThumbnailCache);

        /// <summary>
        /// ISharedBitmap - 091162a4-bc96-411f-aae8-c5122cd03363
        /// </summary>
        public static readonly Guid ISharedBitmapUuid = Guid.Parse(ISharedBitmap);

        /// <summary>
        /// IShellFolder - 000214E6-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IShellFolderUuid = Guid.Parse(IShellFolder);

        /// <summary>
        /// IShellFolder2 - 93F2F68C-1D1B-11D3-A30E-00C04F79ABD1
        /// </summary>
        public static readonly Guid IShellFolder2Uuid = Guid.Parse(IShellFolder2);

        /// <summary>
        /// IEnumIDList - 000214F2-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IEnumIDListUuid = Guid.Parse(IEnumIDList);

        /// <summary>
        /// IShellLinkW - 000214F9-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IShellLinkWUuid = Guid.Parse(IShellLinkW);

        /// <summary>
        /// CShellLink - 00021401-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid CShellLinkUuid = Guid.Parse(CShellLink);

        /// <summary>
        /// IPropertyStore - 886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99
        /// </summary>
        public static readonly Guid IPropertyStoreUuid = Guid.Parse(IPropertyStore);

        /// <summary>
        /// IPropertyStoreCache - 3017056d-9a91-4e90-937d-746c72abbf4f
        /// </summary>
        public static readonly Guid IPropertyStoreCacheUuid = Guid.Parse(IPropertyStoreCache);

        /// <summary>
        /// IPropertyDescription - 6F79D558-3E96-4549-A1D1-7D75D2288814
        /// </summary>
        public static readonly Guid IPropertyDescriptionUuid = Guid.Parse(IPropertyDescription);

        /// <summary>
        /// IPropertyDescription2 - 57D2EDED-5062-400E-B107-5DAE79FE57A6
        /// </summary>
        public static readonly Guid IPropertyDescription2Uuid = Guid.Parse(IPropertyDescription2);

        /// <summary>
        /// IPropertyDescriptionList - 1F9FC1D0-C39B-4B26-817F-011967D3440E
        /// </summary>
        public static readonly Guid IPropertyDescriptionListUuid = Guid.Parse(IPropertyDescriptionList);

        /// <summary>
        /// IPropertyEnumType - 11E1FBF9-2D56-4A6B-8DB3-7CD193A471F2
        /// </summary>
        public static readonly Guid IPropertyEnumTypeUuid = Guid.Parse(IPropertyEnumType);

        /// <summary>
        /// IPropertyEnumType2 - 9B6E051C-5DDD-4321-9070-FE2ACB55E794
        /// </summary>
        public static readonly Guid IPropertyEnumType2Uuid = Guid.Parse(IPropertyEnumType2);

        /// <summary>
        /// IPropertyEnumTypeList - A99400F4-3D84-4557-94BA-1242FB2CC9A6
        /// </summary>
        public static readonly Guid IPropertyEnumTypeListUuid = Guid.Parse(IPropertyEnumTypeList);

        /// <summary>
        /// IPropertyStoreCapabilities - c8e2d566-186e-4d49-bf41-6909ead56acc
        /// </summary>
        public static readonly Guid IPropertyStoreCapabilitiesUuid = Guid.Parse(IPropertyStoreCapabilities);

        /// <summary>
        /// ICondition - 0FC988D4-C935-4b97-A973-46282EA175C8
        /// </summary>
        public static readonly Guid IConditionUuid = Guid.Parse(ICondition);

        /// <summary>
        /// ISearchFolderItemFactory - a0ffbc28-5482-4366-be27-3e81e78e06c2
        /// </summary>
        public static readonly Guid ISearchFolderItemFactoryUuid = Guid.Parse(ISearchFolderItemFactory);

        /// <summary>
        /// IConditionFactory - A5EFE073-B16F-474f-9F3E-9F8B497A3E08
        /// </summary>
        public static readonly Guid IConditionFactoryUuid = Guid.Parse(IConditionFactory);

        /// <summary>
        /// IRichChunk - 4FDEF69C-DBC9-454e-9910-B34F3C64B510
        /// </summary>
        public static readonly Guid IRichChunkUuid = Guid.Parse(IRichChunk);

        /// <summary>
        /// IPersistStream - 00000109-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IPersistStreamUuid = Guid.Parse(IPersistStream);

        /// <summary>
        /// IPersist - 0000010c-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IPersistUuid = Guid.Parse(IPersist);

        /// <summary>
        /// IEnumUnknown - 00000100-0000-0000-C000-000000000046
        /// </summary>
        public static readonly Guid IEnumUnknownUuid = Guid.Parse(IEnumUnknown);

        /// <summary>
        /// IQuerySolution - D6EBC66B-8921-4193-AFDD-A1789FB7FF57
        /// </summary>
        public static readonly Guid IQuerySolutionUuid = Guid.Parse(IQuerySolution);

        /// <summary>
        /// IQueryParser - 2EBDEE67-3505-43f8-9946-EA44ABC8E5B0
        /// </summary>
        public static readonly Guid IQueryParserUuid = Guid.Parse(IQueryParser);

        /// <summary>
        /// IQueryParserManager - A879E3C4-AF77-44fb-8F37-EBD1487CF920
        /// </summary>
        public static readonly Guid IQueryParserManagerUuid = Guid.Parse(IQueryParserManager);

        /// <summary>
        /// INotinheritableBitmap - 091162a4-bc96-411f-aae8-c5122cd03363
        /// </summary>
        public static readonly Guid INotinheritableBitmapUuid = Guid.Parse(INotinheritableBitmap);

        /// <summary>
        /// IShellItemImageFactory - bcc18b79-ba16-442f-80c4-8a59c30c463b
        /// </summary>
        public static readonly Guid IShellItemImageFactoryUuid = Guid.Parse(IShellItemImageFactory);

        /// <summary>
        /// IContextMenu - 000214e4-0000-0000-c000-000000000046
        /// </summary>
        public static readonly Guid IContextMenuUuid = Guid.Parse(IContextMenu);

        /// <summary>
        /// IContextMenu2 - 000214f4-0000-0000-c000-000000000046
        /// </summary>
        public static readonly Guid IContextMenu2Uuid = Guid.Parse(IContextMenu2);

        /// <summary>
        /// IContextMenu3 - BCFCE0A0-EC17-11D0-8D10-00A0C90F2719
        /// </summary>
        public static readonly Guid IContextMenu3Uuid = Guid.Parse(IContextMenu3);

        /// <summary>
        /// IImageList - 46EB5926-582E-4017-9FDF-E8998DAA0950
        /// </summary>
        public static readonly Guid IImageListUuid = Guid.Parse(IImageList);

        #endregion Guid structs
    }
}