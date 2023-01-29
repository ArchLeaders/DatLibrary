using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace DatLibrary.Extensions;

public static class SpanExtension
{
    public static unsafe T ToStruct<T>(this Span<byte> data) where T : struct
    {
        if (data.Length < Marshal.SizeOf<T>()) {
            throw new ArgumentException($"The data was too short, expected {Marshal.SizeOf<T>()} and got {data.Length}", nameof(data));
        }

        fixed (byte* ptr = data) {
            return Marshal.PtrToStructure<T>((IntPtr)ptr);
        }
    }

    public static short ToInt16(this Span<byte> data, bool bigEndian = false)
    {
        if (bigEndian) {
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }
        else {
            return BinaryPrimitives.ReadInt16LittleEndian(data);
        }
    }

    public static ushort ToUInt16(this Span<byte> data, bool bigEndian = false)
    {
        if (bigEndian) {
            return BinaryPrimitives.ReadUInt16BigEndian(data);
        }
        else {
            return BinaryPrimitives.ReadUInt16LittleEndian(data);
        }
    }

    public static int ToInt32(this Span<byte> data, bool bigEndian = false)
    {
        if (bigEndian) {
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }
        else {
            return BinaryPrimitives.ReadInt32LittleEndian(data);
        }
    }

    public static uint ToUInt32(this Span<byte> data, bool bigEndian = false)
    {
        if (bigEndian) {
            return BinaryPrimitives.ReadUInt32BigEndian(data);
        }
        else {
            return BinaryPrimitives.ReadUInt32LittleEndian(data);
        }
    }
}
