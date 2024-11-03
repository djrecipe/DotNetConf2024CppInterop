using ConsoleApp;

// add (using DllImport attribute)
//Console.WriteLine($"Add using DllImport attribute: {BasicInterop.Add(5, 5)}");

// add (using NativeLibrary class)
Interop.Initialize("InteropExample.dll");
Console.WriteLine($"Add using NativeLibrary class: {Interop.Add(5, 5)}");

// invoke mangled name (using DllImport attribute)
//Console.WriteLine($"Invoking mangled name using DllImport attribute: {BasicInterop.MyMangledName()}");