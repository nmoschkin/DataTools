# DataTools Utility Libraries #
## Version 8.0 Development ##

__Note__: Active development is occurring in the __dev-8.0__ branch. It is merged to main from time to time.

## Updates ##

**April 29, 2023**

Finally the heavy refactoring is starting to pay off. There is a new series of utilities in __DataTools.Win32.Disk__ that will read raw partition structure from the disk without having to hit the Operating System. This is part of the first of a series of native interop libraries I plan to attempt to port to *NIX/iOS systems.

I have been going through and trying to write detailed documentation for all public types and members. Now that the disk library refactoring is done (that was a true mess), the remaining refactors for the Win32 subsystem should be completed in relatively less time, and I will be ready to publish 8.0.

**January 2, 2023**

Change of plans. I have decided to go multi-platform with the core libraries. 

I have split up the main DataTools library and grouped classes by function:

  - __DataTools.Text__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
      <br>_Text tools_
  - __DataTools.MathTools__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
     <br>_Miscellaneous Math Tools_
  - __DataTools.Essentials__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
     <br>_DataTools App Essentials_
  - __DataTools.Memory__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
     <br>_Cross-platform memory tools_
  - __DataTools.Graphics__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
     <br>_Miscellaneous graphics tools_
  - __DataTools.Console__ _(.NET Standard 2.0, .NET 7.0, .NET Framework 4.8)_
     <br>_Color Console_
    
Additionally, I have multi-targeted the Windows and Win-32 specific libraries, as well, to .NET 7.0 and .NET Framework 4.8.
 
**December 23, 2022**

__I have decided that I'm going to skip what would have been version 7.0, because version 7.0 would have been based on the old project organization scheme, and I am reorganizing the project in dramatic ways (including some major core deprecations.) I realized I could not sensibly call this project version 7.0, anymore, and so welcome to Version 8.0 Development Phase 1!__

_Migrated to .NET 7.0, preparing to create NuGet packages._
_All references to __IntPtr__ have been replaced with __nint__ except in the .NET Standard 2.0 projects._


### Going Forward

__The .NET Standard 2.0 core libraries of DataTools, DataTools.Graphics, DataTools.Extras and the PluginFramework are being refactored into .NET 7.0+.__

The old .NET Standard 2.0 libraries have been put in a folder called Standard. I will try to keep some core aspects in sync with their new .NET 7.0+ counterparts, but as Xamarin.Forms becomes more legacy, the usefulness of keeping these libraries around evaporates in tandem with that eventuality.

All projects in _this_ solution (save the Xamarin projects) will be using the .NET 7.0 version of the libraries. Also, all of my other public repositories that depend on projects in this repository will be transitioned to .NET 7.0 libraries.

I have decided I think git submodules are useful in specific scenarios, but not when you use your own libraries everywhere to develop all kinds of apps across a wide variety of platforms. Facing this situation, head-on, I've decided that I need to build NuGet packages, at least locally, for myself. I will probably also get a code signing certificate, soon, too.  But this project is not ready for prime-time, yet, so it's not a priority.

__That being said.  Let's have a look at what this project provides, and where I want it to go:__

 - Text Manipulation
   - Basic text cleanup and parsing
   - Byte Order Mark
   - Native-type strings
   - CSV
   - Advanced
     - Expression Parsing
 - Math
   - Radians <-> Cartesean
 - Graphics
   - Color Math
     - CMYK
     - RGB
     - HSV
     - Grayscale
   - Color Wheel
   - Screen Coordinate Conversions
 - Expressions
   - Expression Parsing
   - Unit Conversion to/from SI units
 - Plug-In Framework
 - Sorting
   - Quick Sort
   - Binary Search
   - Red/Black Collection
 - Memory
   - Memory manipulation with safe classes and unsafe structs
 - Cross Platform
   - Color Controls
   - Xamarin
   - MAUI
   - _???_
 - Windows
   - Win32 Memory Objects
     - _Very Swiss-Army-Knife-ish_
     - MemPtr (struct)
     - SafePtrBase (based on SafeHandle)
       - SafePtr
       - CoTaskMemPtr
       - VirtualMemPtr
       - NetworkMemPtr
   - Win32 Desktop
     - Icons
     - Fonts
     - Shell Abstractions
   - Win32 Hardware
     - Device Information
      - Bluetooth
      - Display Adapters
      - Disks
        - Low Level Read/Write
        - GPT Partitions
        - MBR Partitions
        - Virtual Disks and ISO Images
          - Mounting
          - Creating
          - Unmounting
      - Security and Networking
        - In-depth Adapter Information
        - Network Neighborhood
        - Local and Remote principal and machine enumeration
      - Printers
        - Paper Types
        - Printer Queue
        - Print
      - Processor Info
      - USB
        - HID Device Probing
    
Etc, etc.. 

This is basically a collection of things I find repetitively useful in the course of development work.  I recognize it's basically a hodge-podge of "slightly-more-organized-than-code-snippets."  So I have to think about which pieces really belong together, and which pieces belong in wholly different projects.

But, I've realized that, over the (many) years, I've written all of these projects to be somewhat dependent on one-another in various intricate ways. There is a hierarchy (everything requires DataTools, for example), but especially with regard to the Win32 projects, the dependencies are all over the place.

So, my goal is to work all that out, and make this project useful to the public.

The most difficult re-organization will be with the Win32 libraries. 

More to come!


## Version 7.2.0-pre-alpha ##

## Updates ##

**April 29, 2022**

Finally got around to moving all of the related projects into their respective real folders (as opposed to just being virtually separated with solution folders.)


Continuing to refactor and bug-fix.

**ADDED MAUI PROJECTS**

The MAUI ColorControls port will be finished pending a complete re-installation of .NET 6.0 Preview and Visual Studio 2022 Preview (due to difficulties.)

**January 30, 2022**

So, I've finally started programming regularly in WinUI 3/Project Reunion and by extension the UWP namespaces.  I will be cleaning this code up and organizing it into sections based on target type.  I will also be cleaning up and 'universalizing' all of the universal code and the Win32 code will become better organized. I'm going to merge code from separate DLL's and break out new ones.  

The organizational structure of this project is going to be changed so much that the changes will be breaking.  

Therefore, I am going to be starting work on **DataTools 7.0** which will be a major reorganization (but not an entire rewrite).  

Please, if you have questions or comments, do not hesitate to use the issues board to comment or just reach out. 

## Notes ##

The project is finally completely refactored into C# and .NET 6.0, and taking on a final form.  

The hardware components have been separated out into the various types of hardware so you don't have to include a massive hardware library to get at one specific thing.

Also, I have moved a lot of what used to be open-to-the-public guts code to the backend.  I am figuring if there is a need to expose deeper functions in the future, I can abstract them to a layer more consanguine for 3rd-part consumption.

Also very different is all references to WPF and WinForms libraries have been removed from the DataTools.Win32.* projects, as has anything that feels even remotely 'ViewModelly'.  Going forward, layers that interface with WPF, WinUI, or WinForms will have their own intermediate libraries (or you can write your own!)

Most of the code is well-documented.

I will be using SandCastle Help Compiler to create a new Wiki for this very different project.

**Important**

 - Some features in the hardware libraries depend on compiling native to your platform to function properly (compiling to x64 on 64-bit systems.)
 - Many features require elevated administrative access.

__Hardware disk information features require the platform to be correct (especially for using the virtual disk system), and for elevated access to be present.__

The Demo Project is the **SysInfoTool** project.  

