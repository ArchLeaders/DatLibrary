using CeadLibrary.IO;
using System.Buffers;
using System.Text;

namespace DatLibrary.Core;

public class DatBase
{
    public DatBase(CeadReader reader)
    {
        long hdrOffset = reader.ReadUInt32();

        if ((hdrOffset & 0x80000000) != 0) {
            hdrOffset ^= 0xFFFFFFFF;
            hdrOffset <<= 0x8;
            hdrOffset += 0x100;
        }

        if (reader.BaseStream.Length > uint.MaxValue) {
            hdrOffset = 0x0100000000 ^ hdrOffset;
        }

        using (reader.TemporarySeek(hdrOffset, SeekOrigin.Begin)) {
            int archiveId = reader.ReadInt32();
            uint fileCount = reader.ReadUInt32();

            // Isn't the file count on newer archives?
            if (fileCount != 0x3443432E && fileCount != 0x2e434334 && fileCount != 0x3443432E && fileCount != 0x2E434334) {
                LoadLegacyHdr(reader, archiveId, fileCount);
            }
            else {
                LoadHdr(reader);
            }
        }
    }

    public void LoadHdr(CeadReader reader)
    {

    }

    public static void LoadLegacyHdr(CeadReader reader, int archiveId, uint fileCount)
    {
        var entries = new (uint offset, uint zsize, uint size, uint packed)[fileCount];
        for (int i = 0; i < fileCount; i++) {
            uint offset = reader.ReadUInt32();
            if (archiveId != -1) {
                offset <<= 0x8;
            }

            uint zSize = reader.ReadUInt32();
            uint size = reader.ReadUInt32();
            uint packed = reader.ReadUInt32() & 0x00FFFFFF;

            entries[i] = (offset + (packed >> 24), zSize, size, packed);
        }

        int nameCount = reader.ReadInt32();
        long namesOffset = reader.BaseStream.Position + nameCount * (archiveId <= -5 ? 12 : 8) + 4; // not sure what the last DWord before the strings is
        string[] prevNames = ArrayPool<string>.Shared.Rent(nameCount);
        string[] names = new string[fileCount];

        int index = 0;
        for (int i = 0; i < fileCount; i++) {
            int next = 1;
            StringBuilder sb = new();
            while (next > 0) {
                // >0 means there's another node, but I'm
                // assuming since the value isn't 1/0,
                // that it contains other data
                next = reader.ReadInt16();
                
                // Prepend the last path (probably poorly),
                // clear may be redundant here, I don't
                // think this is ever reached while(sb.Length > 0)
                int prev = reader.ReadInt16();
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
                int offset = reader.ReadInt32();
                if (offset >= 0) {
                    using (reader.TemporarySeek(namesOffset + offset, SeekOrigin.Begin)) {
                        sb.Append(reader.ReadString(StringType.ZeroTerminated));
                    }

                    if (next > 0) {
                        sb.Append(Path.DirectorySeparatorChar);
                    }
                }
                
                if (archiveId <= -5) {
                    reader.Seek(4, SeekOrigin.Current);
                }

                index++;
            }

            names[i] = sb.ToString();
        }

        ArrayPool<string>.Shared.Return(prevNames);
    }
}
