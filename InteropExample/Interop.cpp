#include "pch.h"
#include "Interop.h"

#include <codecvt>
#include <locale>
#include <stdexcept>
#include <sstream>

namespace DotNetConf
{
namespace Interop
{
extern "C"
{
int Add(int left, int right)
{
    return left + right;
}
int ConcatStrings(char* left, char* right, void*& output)
{
    std::ostringstream stream;
    stream << left << right;
    std::string concat = stream.str();
    auto length = strlen(concat.c_str());
    output = new char[length + 1];
    strcpy((char*)output, concat.c_str());
    ((char*)output)[length] = '\0';
    return length + 1;
}
int ConcatWideStrings(char* left, char* right, void*& output)
{
    std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

    // decode raw bytes into platform-dependent unicode encoding (utf-16 for windows, utf-8 for unix)
    std::wstring left_wstr = converter.from_bytes(left);
    std::wstring right_wstr = converter.from_bytes(right);

    std::wstringstream stream;
    stream << left << right;
    std::wstring concat = stream.str();
    auto length = wcslen(concat.c_str());
    output = new wchar_t[length + 1];
    wcscpy((wchar_t*)output, concat.c_str());
    ((wchar_t*)output)[length] = L'\0';
    return length + 1;
}
int DeleteArray(void* ptr)
{
    delete[] ptr;
    return 1;
}
void ThrowUnhandledException()
{
    throw std::runtime_error("unhandled exception!");
}
int MarshalStruct(MyData* data)
{
    return sizeof(data);
}
int MYcdecl()
{
    return 57;
}
int MYstdcall()
{
    return 66;
}
}
int MyMangledName()
{
    return 1;
}
}
}
