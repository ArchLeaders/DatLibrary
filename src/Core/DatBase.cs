using CeadLibrary.IO;
using DatLibrary.Extensions;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

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

        // Isn't the file count on newer archives?
        if (fileCount != 0x3443432E && fileCount != 0x2e434334 && fileCount != 0x3443432E && fileCount != 0x2E434334) {
            LoadLegacyHdr(stream, archiveId, fileCount);
        }
        else {
            LoadHdr(null!);
        }
    }

    public void LoadHdr(Stream stream)
    {

    }

    public static void LoadLegacyHdr(Stream stream, int archiveId, int fileCount)
    {
        // Only allocate buffers larger than 1mb
        int entryBufferSize = fileCount * 16;
        Span<byte> entriesBufffer = entryBufferSize < 0x100000 ? stackalloc byte[entryBufferSize] : new byte[entryBufferSize];
        stream.Read(entriesBufffer);

        Span<byte> buffer = stackalloc byte[16];
        var entries = new (string path, uint offset, uint zsize, uint size, uint packed)[fileCount];
        for (int i = 0; i < fileCount; i++) {
            buffer = entriesBufffer[(i * 16)..(i * 16 + 16)];

            uint offset = buffer[..4].ToUInt32();
            if (archiveId != -1) {
                offset <<= 0x8;
            }

            uint zSize = entriesBufffer[4..8].ToUInt32();
            uint size = entriesBufffer[12..16].ToUInt32();
            uint packed = entriesBufffer[16..].ToUInt32() & 0x00FFFFFF;

            entries[i] = (null!, offset + (packed >> 24), zSize, size, packed);
        }

        // Get string info entry count
        Span<byte> buffer32 = stackalloc byte[4];
        stream.Read(buffer32);
        int nameCount = buffer32.ToInt32();

        // Only allocate buffers larger than 1mb
        int nodeSize = archiveId <= -5 ? 12 : 8;
        int strInfoBufferSize = nameCount * nodeSize;
        Span<byte> strInfoBuffer = strInfoBufferSize < 0x100000 ? stackalloc byte[strInfoBufferSize] : new byte[strInfoBufferSize];
        stream.Read(strInfoBuffer);

        // not sure what the last DWord before the strings is
        long namesOffset = stream.Position + strInfoBufferSize + 4;
        string[] prevNames = ArrayPool<string>.Shared.Rent(nameCount);

        Stack<string> root = new();
        buffer = stackalloc byte[8];

        int index = 0;
        for (int i = 0; i < fileCount; i++) {
            StringBuilder sb = new();

            int next = 1;
            while (next > 0) {
                buffer = strInfoBuffer[(index * 12)..(index * 12 + 12)];

                // >0 means there's another node, but I'm
                // assuming since the value isn't 1/0,
                // that it contains other data
                next = buffer[..2].ToInt16();

                // Prepend the last path (probably poorly),
                // clear may be redundant here, I don't
                // think this is ever reached while(sb.Length > 0)
                int prev = buffer[2..4].ToInt16();
                if (prev != 0) {
                    sb.Clear();
                    sb.Append(prevNames[prev]);
                }

                // Seems to allocate unessissary duplicates,
                // but they are indexed by prev in some
                // instances
                if (sb.Length > 0) {
                    prevNames[index] = sb.ToString();
                }

                // Read the string from an offset relative
                // to the start of the string table
                int offset = buffer[4..8].ToInt32();
                if (offset >= 0) {
                    sb.Append("__entry__");

                    if (next > 0) {
                        sb.Append(Path.DirectorySeparatorChar);
                    }
                }

                index++;
            }

            entries[i].path = sb.ToString();
        }
    }
}
