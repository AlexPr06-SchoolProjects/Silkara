using Microsoft.IO;

namespace UtpTypes.Managers;

public static class UtpMemoryManager
{
    public static readonly RecyclableMemoryStreamManager Pool = new RecyclableMemoryStreamManager();
}
