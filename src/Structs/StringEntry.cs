using System.Runtime.InteropServices;

namespace DatLibrary.Structs;

[StructLayout(LayoutKind.Explicit, Size = 0x0C, Pack = 1)]
public struct StringEntry
{
    [FieldOffset(0x00)]
    public short BlobIndex;

    [FieldOffset(0x02)]
    public short NodeIndex;

    [FieldOffset(0x04)]
    public uint Offset;

    [FieldOffset(0x08)]
    public uint Unknown;
}
