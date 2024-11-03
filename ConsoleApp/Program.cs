using System.Text;
using ConsoleApp;

// add (using DllImport attribute)
//Console.WriteLine($"Add using DllImport attribute: {BasicInterop.Add(5, 5)}");
// concat strings (using DllImport attribute)

// add (using NativeLibrary class)
// load library
Interop.Initialize("InteropExample.dll");
// invoke add method
Console.WriteLine($"Add 5 + 5: {Interop.Add(5, 5)}");
// invoke string concatenation
Console.WriteLine($"Concatenate 'foo' and 'bar': {Interop.ConcatStrings("foo", "bar")}");
// invoke unicode string concatenation
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine($"Concatenate 'ดดดด' and 'ๆๆๆๆ': {Interop.ConcatWideStrings("ดดดด", "ๆๆๆๆ")}");
// invoke mangled method
Console.WriteLine($"Invoking mangled method name: {Interop.MyMangledName()}");