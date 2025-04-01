namespace RisSerialization;

public struct FatPtr
{
    public int Address;
    public int Length;

    public FatPtr()
    {
        Address = 0;
        Length = 0;
    }

    public static FatPtr WithLength(int address, int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }

        var result = new FatPtr();
        result.Address = address;
        result.Length = length;
        return result;
    }

    public static FatPtr WithEnd(int begin, int end)
    {
        if (begin > end)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, null);
        }

        var result = new FatPtr();
        result.Address = begin;
        result.Length = end - begin;
        return result;
    }

    public int End()
    {
        return Address + Length;
    }

    public bool IsNull()
    {
        return Address == 0 && Length == 0;
    }
}