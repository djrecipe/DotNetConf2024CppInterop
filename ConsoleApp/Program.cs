using ConsoleApp;

// add (using DllImport attribute)
//Console.WriteLine($"Add using DllImport attribute: {BasicInterop.Add(5, 5)}");
// concat strings (using DllImport attribute)

// add (using NativeLibrary class)
Interop.Initialize("InteropExample.dll");
Console.WriteLine($"Add using NativeLibrary class: {Interop.Add(5, 5)}");
Console.WriteLine($"Concatenate 'foo' and 'bar' using NativeLibrary class: {Interop.ConcatStrings("foo", "bar")}");

// invoke mangled name (using DllImport attribute)
//Console.WriteLine($"Invoking mangled name using DllImport attribute: {BasicInterop.MyMangledName()}");