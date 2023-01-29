using DatLibrary.Extensions;
using DatLibrary.Structs;
using System.Buffers;
using System.Buffers.Binary;

namespace DatLibrary.Core;

public class DatBase
{
    public DatBase(Stream stream)
    {
        Span<byte> buffer32 = stackalloc byte[4];
        stream.Read(buffer32);
        long tableOffset = buffer32.ToUInt32();

        if ((tableOffset & 0x80000000) != 0) {
            tableOffset ^= 0xFFFFFFFF;
            tableOffset <<= 0x8;
            tableOffset += 0x100;
        }

        if (stream.Length > uint.MaxValue) {
            tableOffset = 0x0100000000 ^ tableOffset;
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        
        stream.Read(buffer32);
        int archiveId = BinaryPrimitives.ReadInt32LittleEndian(buffer32);

        stream.Read(buffer32);
        int fileCount = BinaryPrimitives.ReadInt32LittleEndian(buffer32);

        // Isn't the file count on some archives?
        if (fileCount != 0x3443432E && fileCount != 0x2e434334 && fileCount != 0x3443432E && fileCount != 0x2E434334) {
            LoadArchive(stream, archiveId, fileCount);
        }
        else {
            LoadHdr(null!);
        }
    }

    public void LoadHdr(Stream stream)
    {

    }

    public static unsafe void LoadArchive(Stream stream, int archiveId, int entryCount)
    {
        Entry[] entries = new Entry[entryCount];
        stream.FillArray(entries);

        int stringEntryCount = stream.ReadInt32();
        StringEntry[] stringEntries = new StringEntry[stringEntryCount];
        stream.FillArray(stringEntries);

        int stringTableSize = stream.ReadInt32();
        byte[] stringTable = ArrayPool<byte>.Shared.Rent(stringTableSize);

        // Build top-level nodes

        ArrayPool<byte>.Shared.Return(stringTable);
    }
}
