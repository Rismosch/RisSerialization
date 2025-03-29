using RisSerialization;

namespace Benchmark;

public class Data
{
    private static readonly byte[] Magic = { 1, 2, 3, 4, 5 };
    private const int Version = 42;

    public float MyNumber;
    public List<bool> ABunchOfBools;
    public string MyName;
    public byte[] Payload;

    public byte[] Serialize()
    {
        var s = new RisMemoryStream();

        // magic
        RisIO.Write(s, Magic);
        // version
        RisIO.WriteInt(s, Version);

        // serialize
        RisIO.WriteFloat(s, MyNumber);
        RisIO.WriteInt(s, ABunchOfBools.Count);
        foreach (var b in ABunchOfBools)
        {
            RisIO.WriteBool(s, b);
        }

        RisIO.WriteString(s, MyName);
        var ptrAddress = RisIO.Seek(s, 0, SeekFrom.Current);
        RisIO.WriteFatPtr(s, new FatPtr());
        var ptr = RisIO.Write(s, Payload);
        RisIO.Seek(s, ptrAddress, SeekFrom.Begin);
        RisIO.WriteFatPtr(s, ptr);

        var result = s.ToArray();
        return result;
    }

    public static Data Deserialize(byte[] value)
    {
        var s = new RisMemoryStream(value);

        // magic
        var magic = RisIO.Read(s, Magic.Length);
        for (int i = 0; i < Magic.Length; i++)
        {
            var left = Magic[i];
            var right = magic[i];

            if (left != right)
            {
                throw new FormatException("magic does not match");
            }
        }

        // version
        var version = RisIO.ReadInt(s);
        if (version != Version)
        {
            throw new FormatException("version does not match");
        }

        // deserialize
        var result = new Data();
        result.MyNumber = RisIO.ReadFloat(s);
        result.ABunchOfBools = new List<bool>();
        var count = RisIO.ReadInt(s);
        for (int i = 0; i < count; i++)
        {
            var b = RisIO.ReadBool(s);
            result.ABunchOfBools.Add(b);
        }

        result.MyName = RisIO.ReadString(s);
        var ptr = RisIO.ReadFatPtr(s);
        result.Payload = RisIO.ReadAt(s, ptr);
        ;

        return result;
    }
}