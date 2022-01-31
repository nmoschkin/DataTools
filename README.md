# DataTools Utility Libraries #
## Version 7.0.0 Alpha ##


## Updates ##

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

