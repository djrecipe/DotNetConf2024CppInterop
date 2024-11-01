#include "pch.h"
#include "Interop.h"

#include <stdexcept>

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
    return -4;
}
}
}
