#pragma once
#include <vector>
namespace DotNetConf
{
namespace Interop
{

// avoid name mangling
extern "C"
{

// example struct
struct MyData
{
    int Value1;
    double Value2;
    int ArrayCount;
    int* ArrayValues;
};

// these methods will be exported because they have the export attribute
__declspec(dllexport) int Add(int left, int right);
__declspec(dllexport) int  __cdecl MYcdecl();
__declspec(dllexport)
int  __stdcall MYstdcall();

// these methods will not be exported, unless declared in a .def file
void ThrowUnhandledException();
int MarshalStruct(MyData* data);

} // end of "extern c"

// C++ export will result in a mangled export name
__declspec(dllexport) int MyMangledName();

}
}