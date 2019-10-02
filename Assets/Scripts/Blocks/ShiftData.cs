public struct ShiftData
{
    public byte data;

    public static implicit operator ShiftData(byte data) => new ShiftData{ data = data };

    public int maxHeight
    {
        get
        {
            return data & 0b_0111_1111;
        }
        set
        {
            data = (byte)((data & ~0b_0111_1111) | value);
        }
    }
}