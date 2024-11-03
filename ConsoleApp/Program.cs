using ConsoleApp;

// add (using DllImport attribute)
Console.WriteLine($"Basic interop result: {BasicInterop.Add(5, 5)}");

// invoke mangled name (using DllImport attribute)
Console.WriteLine($"Invoking mangled name using DllImport attribute: {BasicInterop.MyMangledName()}");