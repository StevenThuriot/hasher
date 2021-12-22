using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleHasher.Internals;

#if NETSTANDARD2_1
internal static class BitOperations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint value, int offset)
    {
        return (value << offset) | (value >> 32 - offset);
    }
}
#else 
using System.Numerics;
#endif

internal struct SimpleHashCode
{
    private const uint Seed = 1309273434;

    private uint _v1;

    private uint _v2;

    private uint _v3;

    private uint _v4;

    private uint _queue1;

    private uint _queue2;

    private uint _queue3;

    private uint _length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input) => BitOperations.RotateLeft(hash + (uint)((int)input * -2048144777), 13) * 2654435761u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue) => BitOperations.RotateLeft(hash + (uint)((int)queuedValue * -1028477379), 17) * 668265263;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4) => BitOperations.RotateLeft(v1, 1) + BitOperations.RotateLeft(v2, 7) + BitOperations.RotateLeft(v3, 12) + BitOperations.RotateLeft(v4, 18);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        hash ^= hash >> 15;
        hash *= 2246822519u;
        hash ^= hash >> 13;
        hash *= 3266489917u;
        hash ^= hash >> 16;
        return hash;
    }

    public void Add(object? value)
    {
        switch (value)
        {
            case null:
                Add(0);
                break;

            case string str:
                Add(str);
                break;

            case char character:
                Add(character);
                break;

            default:
                Add(value.GetHashCode());
                break;
        }
    }

    public void Add(char value) => Add((int)(value | ((uint)value << 16)));

    public void Add(string value)
    {
        if (value is not null)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Add(value[i]);
            }

            Add('\0');
        }
        else
        {
            Add(0);
        }
    }

    public void Add(int value)
    {
        uint num = _length++;
        switch (num % 4u)
        {
            case 0u:
                _queue1 = (uint)value;
                return;
            case 1u:
                _queue2 = (uint)value;
                return;
            case 2u:
                _queue3 = (uint)value;
                return;
        }

        _v1 = Round(_v1, _queue1);
        _v2 = Round(_v2, _queue2);
        _v3 = Round(_v3, _queue3);
        _v4 = Round(_v4, (uint)value);
    }

    public int ToHashCode()
    {
        uint length = _length;
        uint num = length % 4u;
        uint num2 = ((length < 4) ? (Seed + 374761393) : MixState(_v1, _v2, _v3, _v4)) + (length * 4);

        if (num != 0)
        {
            num2 = QueueRound(num2, _queue1);
            if (num > 1)
            {
                num2 = QueueRound(num2, _queue2);
                if (num > 2)
                {
                    num2 = QueueRound(num2, _queue3);
                }
            }
        }

        return (int)MixFinal(num2);
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException("HashCodeNotSupported");

    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException("HashCode_EqualityNotSupported");
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}