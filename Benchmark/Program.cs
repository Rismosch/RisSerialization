using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Benchmark;
using Newtonsoft.Json;

// the data to be serialized
var data = new Data();
data.MyNumber = 13.45f;
data.ABunchOfBools = new List<bool> { true, false, true, true };
data.MyName = "Rismosch";
// generated via https://www.randomnumbergenerator.com/
data.Payload = new byte[]
{
    248,
    188,
    93,
    51,
    115,
    254,
    255,
    92,
    185,
    245,
    163,
    158,
    114,
    165,
    34,
    163,
};

// compression utils
byte[] Compress(byte[] value)
{
    using var outputStream = new MemoryStream();
    using (var deflateStream = new DeflateStream(outputStream, CompressionMode.Compress, true))
    {
        deflateStream.Write(value, 0, value.Length);
    }

    return outputStream.ToArray();
}

byte[] Decompress(byte[] value)
{
    using var input = new MemoryStream(value);
    using var output = new MemoryStream();
    using var s = new DeflateStream(input, CompressionMode.Decompress);
    s.CopyTo(output);
    return output.ToArray();
}

// size
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("size");
Console.WriteLine();

// custom
var customBytes = data.Serialize();
var customSize = customBytes.Length;
var customCompressedBytes  = Compress(customBytes);
var customCompressedSize = customCompressedBytes.Length;
Console.WriteLine($"custom size: {customSize}");
Console.WriteLine($"custom compressed size: {customCompressedSize}");

// newtonsoft
var newtonsoftJson = JsonConvert.SerializeObject(data);
var newtonsoftBytes = Encoding.UTF8.GetBytes(newtonsoftJson);
var newtonsoftCompressedBytes = Compress(newtonsoftBytes);
var newtonsoftSize = newtonsoftBytes.Length;
var newtonsoftCompressedSize = newtonsoftCompressedBytes.Length;
Console.WriteLine($"newtonsoft size: {newtonsoftSize}");
Console.WriteLine($"newtonsoft compressed size: {newtonsoftCompressedSize}");

// benchmark
Stopwatch stopwatch;
const int runs = 1_000_000;
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("speed");
Console.WriteLine();
Console.WriteLine($"runs: {runs}");

[MethodImpl(MethodImplOptions.NoInlining)]
void BlackBox(object? value)
{
    // prevent dead code elimination
}

// custom serialization
Console.WriteLine("benchmarking custom...");
stopwatch = Stopwatch.StartNew();
for (var i = 0; i < runs; i++)
{
    var bytes = data.Serialize();
    var copy = Data.Deserialize(bytes);
    BlackBox(copy);
}

Console.WriteLine($"custom: {stopwatch.Elapsed}");

// custom with compression
Console.WriteLine("benchmarking custom with compression...");
stopwatch = Stopwatch.StartNew();
for (var i = 0; i < runs; i++)
{
    var bytes = data.Serialize();
    var compressed = Compress(bytes);
    var decompressed = Decompress(compressed);
    var copy = Data.Deserialize(decompressed);
    BlackBox(copy);
}

Console.WriteLine($"custom with compression: {stopwatch.Elapsed}");

// newtonsoft json
Console.WriteLine("benchmarking newtonsoft...");
stopwatch = Stopwatch.StartNew();
for (var i = 0; i < runs; i++)
{
    var json = JsonConvert.SerializeObject(data);
    var copy = JsonConvert.DeserializeObject<Data>(json);
    BlackBox(copy);
}

Console.WriteLine($"newtonsoft: {stopwatch.Elapsed}");

// newtonsoft json with compression
Console.WriteLine("benchmarking newtonsoft with compression...");
stopwatch = Stopwatch.StartNew();
for (var i = 0; i < runs; i++)
{
    var json = JsonConvert.SerializeObject(data);
    var bytes = Encoding.UTF8.GetBytes(json);
    var compressed = Compress(bytes);
    var decompressed = Decompress(compressed);
    var jsonCopy = Encoding.UTF8.GetString(decompressed);
    var copy = JsonConvert.DeserializeObject<Data>(jsonCopy);
    BlackBox(copy);
}

Console.WriteLine($"newtonsoft with compression: {stopwatch.Elapsed}");