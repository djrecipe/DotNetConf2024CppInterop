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
    MyData() : ArrayValues(nullptr) {};
    ~MyData() { delete[] ArrayValues; }
};

// these methods will be exported because they have the export attribute
__declspec(dllexport) int Add(int left, int right);
__declspec(dllexport)
int ConcatStrings(const char* left, const char* right,
    void*& output);
__declspec(dllexport) int ConcatWideStrings(const char* left, const char* right, void*& output);
__declspec(dllexport) int AddStructValues(void* data);
__declspec(dllexport) int DeleteArray(void* ptr);
__declspec(dllexport) int DeleteStruct(void* ptr);
__declspec(dllexport) int  __cdecl MYcdecl();
__declspec(dllexport) int  __stdcall MYstdcall();
__declspec(dllexport) void ThrowUnhandledException();
__declspec(dllexport) int ThrowCaughtException();

} // end of "extern c"

// C++ export will result in a mangled export name
__declspec(dllexport) int MyMangledName();

}
}