# DataTools Utility Libraries #
## Version 7.0.0-pre-alpha ##


## Updates ##

**December 23, 2022**

So, I have gone several dramatic ways in attempting to determine how best to refactor these projects. 

I have decided I think git submodules are useful in specific scenarios, but not when you use your own libraries everywhere to develop all kinds of apps across a wide variety of platforms.

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
 - Sorting
   - Quick Sort
   - Binary Search
   - Red/Black Collection
 - Memory
   - Memory manipulation with safe classes and unsafe structs
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

So I've realized that I've written all of these projects somewhat dependent on one-another, but there is a hierarchy (everything requires DataTools, for example). But, as the Win32 projects are all separate, right now, untangling could be a bit messy.

But the future is NuGet packages. So, now I have to determine what's too big to go all in the same lib, and what even makes sense to go in the same lib? Display adapter info and processor info are very small compared to the implementation of printers and networking, for example.  Should I make those bigger libraries into one package or make the entire hardware library into one package?

There is also the matter of confusing organization and unused references with regard to the p/Invoke situation.  

So.  All of that's being figured, factored, refactored, and worked out. 

More to come!



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

