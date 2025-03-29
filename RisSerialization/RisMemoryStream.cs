namespace RisSerialization;

public class RisMemoryStream
{
    private byte[] _data;
    private int _position;

    public RisMemoryStream()
    {
        _data = Array.Empty<byte>();
        _position = 0;
    }

    public RisMemoryStream(byte[] value)
    {
        _data = value;
        _position = 0;
    }

    public byte[] ToArray()
    {
        return _data.ToArray();
    }

    public int Seek(int offset, SeekFrom seekFrom)
    {
        switch (seekFrom)
        {
            case SeekFrom.Begin:
                _position = offset;
                break;
            case SeekFrom.Current:
                _position += offset;
                break;
            case SeekFrom.End:
                _position = _data.Length + offset;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(seekFrom), seekFrom, null);
        }

        // clamp the position, in case it falls out of range
        if (_position < 0)
        {
            _position = 0;
        }

        if (_position > _data.Length)
        {
            _position = _data.Length;
        }

        return _position;
    }

    public byte[] Read(int count)
    {
        // clamp count, such that only the remaining bytes are read
        var bytesLeftToRead = _data.Length - _position;
        if (count > bytesLeftToRead)
        {
            count = bytesLeftToRead;
        }

        // read the bytes, by copying them to a new array
        var bytes = new byte[count];
        Array.Copy(
            _data,
            _position,
            bytes,
            0,
            count
        );

        // advance the cursor
        _position += count;
        return bytes;
    }

    public void Write(byte[] value)
    {
        // ensure the capacity is big enough
        var requiredCapacity = _position + value.Length;
        if (_data.Length < requiredCapacity)
        {
            // capacity is not big enough
            // create an array that is big enough and copy the old into the new one
            var newDataArray = new byte[requiredCapacity];
            Array.Copy(
                _data,
                newDataArray,
                _data.Length
            );
            _data = newDataArray;
        }

        // write by copying the values into the array
        Array.Copy(
            value,
            0,
            _data,
            _position,
            value.Length
        );

        // advance the cursor
        _position += value.Length;
    }
}