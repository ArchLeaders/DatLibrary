using System.Buffers.Binary;

namespace DatLibrary.Extensions;

public static class SpanExtension
{
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
