using System.Diagnostics.CodeAnalysis;

namespace RisSerialization;

public struct FatPtr : IEquatable<FatPtr>
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
        return Equals(new FatPtr());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FatPtr other)
        {
            return false;
        }

        return Equals(other);
    }

    public bool Equals(FatPtr other)
    {
        return Address == other.Address && Length == other.Length;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Address, Length);
    }
}