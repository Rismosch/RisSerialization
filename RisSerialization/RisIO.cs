using System.Text;

namespace RisSerialization;

public static class RisIO
{
    // wrappers
    public static int Seek(RisMemoryStream s, int offset, SeekFrom seekFrom)
    {
        return s.Seek(offset, seekFrom);
    }

    public static byte[] ReadUnchecked(RisMemoryStream s, int count)
    {
        return s.Read(count);
    }

    public static byte[] Read(RisMemoryStream s, int count)
    {
        var result = ReadUnchecked(s, count);
        if (result.Length != count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, null);
        }

        return result;
    }

    public static byte[] ReadAt(RisMemoryStream s, FatPtr fatPtr)
    {
        Seek(s, fatPtr.Address, SeekFrom.Begin);
        var result = Read(s, fatPtr.Length);
        return result;
    }

    public static FatPtr Write(RisMemoryStream s, byte[] value)
    {
        var begin = Seek(s, 0, SeekFrom.Current);
        s.Write(value);
        var end = Seek(s, 0, SeekFrom.Current);
        var ptr = FatPtr.WithEnd(begin, end);
        return ptr;
    }

    // read
    public static int ReadInt(RisMemoryStream s)
    {
        var bytes = Read(s, 4);
        FixEndianness(bytes);
        var result = BitConverter.ToInt32(bytes);
        return result;
    }

    public static float ReadFloat(RisMemoryStream s)
    {
        var bytes = Read(s, 4);
        FixEndianness(bytes);
        var result = BitConverter.ToSingle(bytes);
        return result;
    }

    public static T ReadEnum<T>(RisMemoryStream s) where T : Enum
    {
        var i = ReadInt(s);
        if (!Enum.IsDefined(typeof(T), i))
        {
            throw new FormatException($"{i} is not defined for enum {typeof(T)}");
        }

        var result = (T)Enum.ToObject(typeof(T), i);
        return result;
    }

    public static bool ReadBool(RisMemoryStream s)
    {
        var bytes = Read(s, 1);
        var b = bytes[0];
        switch (b)
        {
            case 1:
                return true;
            case 0:
                return false;
            default:
                throw new FormatException($"{b} is not a valid bool");
        }
    }

    public static string ReadString(RisMemoryStream s)
    {
        var length = ReadInt(s);
        var bytes = Read(s, length);
        var result = Encoding.UTF8.GetString(bytes);
        return result;
    }

    public static FatPtr ReadFatPtr(RisMemoryStream s)
    {
        var address = ReadInt(s);
        var length = ReadInt(s);
        var result = FatPtr.WithLength(address, length);
        return result;
    }

    // write
    public static void WriteInt(RisMemoryStream s, int value)
    {
        var bytes = BitConverter.GetBytes(value);
        FixEndianness(bytes);
        Write(s, bytes);
    }

    public static void WriteFloat(RisMemoryStream s, float value)
    {
        var bytes = BitConverter.GetBytes(value);
        FixEndianness(bytes);
        Write(s, bytes);
    }

    public static void WriteEnum(RisMemoryStream s, Enum value)
    {
        var i = Convert.ToInt32(value);
        WriteInt(s, i);
    }

    public static void WriteBool(RisMemoryStream s, bool value)
    {
        if (value)
        {
            Write(s, new byte[] { 1 });
        }
        else
        {
            Write(s, new byte[] { 0 });
        }
    }

    public static void WriteString(RisMemoryStream s, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteInt(s, bytes.Length);
        Write(s, bytes);
    }

    public static void WriteFatPtr(RisMemoryStream s, FatPtr value)
    {
        WriteInt(s, value.Address);
        WriteInt(s, value.Length);
    }

    // util
    public static void FixEndianness(byte[] value)
    {
        // we are using little-endian. thus, if our cpu is not little-endian, we need to flip the bytes
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(value);
        }
    }
}