using CeadLibrary.IO;
using DatLibrary.Core;
using System.Diagnostics;

Stopwatch watch = Stopwatch.StartNew();

FileStream fs = File.OpenRead("D:\\Games\\LegoDimensions\\Game\\content\\GAME.DAT");
CeadReader reader = new(fs);
DatBase _base = new(reader);

watch.Stop();

Console.WriteLine($"Elapsed Milliseconds: {watch.ElapsedMilliseconds}");