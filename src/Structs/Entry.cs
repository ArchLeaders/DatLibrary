using System.Runtime.InteropServices;

namespace DatLibrary.Structs;

[StructLayout(LayoutKind.Explicit, Size = 0x10, Pack = 1)]
public struct Entry
{
	[FieldOffset(0x00)]
	public uint Offset;

	[FieldOffset(0x04)]
	public uint Size;

	[FieldOffset(0x08)]
	public uint UncompressedSize;

	[FieldOffset(0x0C)]
	public uint CompressFlag;
}
