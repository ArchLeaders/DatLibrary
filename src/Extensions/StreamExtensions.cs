using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace DatLibrary.Extensions;

public static class StreamExtensions
{
    public static unsafe void FillArray<T>(this Stream stream, T[] values) where T : struct
    {
        int structSize = Marshal.SizeOf<T>();
        int allocSize = values.Length * structSize;

        if (allocSize < 0x100000) {
            Span<byte> buffer = stackalloc byte[allocSize];
            stream.Read(buffer);
            for (int i = 0; i < values.Length; i++) {
                values[i] = buffer[(i * structSize)..(i * structSize + structSize)].ToStruct<T>();
            }
        }
        else {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(allocSize);
            stream.Read(buffer, 0, allocSize);
            for (int i = 0; i < values.Length; i++) {
                values[i] = buffer.AsSpan()[(i * structSize)..(i * structSize + structSize)].ToStruct<T>();
            }

            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static short ReadInt16(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        stream.Read(buffer);
        return BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static ushort ReadUInt16(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        stream.Read(buffer);
        return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }
    
    public static int ReadInt32(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        stream.Read(buffer);
        return BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static uint ReadUInt32(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        stream.Read(buffer);
        return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }
}
