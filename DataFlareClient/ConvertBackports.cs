using System;
using System.Collections.Generic;
using System.Text;

namespace DataFlareClient
{
    //
    //This file contains functions backported from .net5, should be removed if library runtime is updated
    //
    public static class ConvertBackports
    {
        /// <summary>
        /// Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <code>null</code>.</exception>
        /// <exception cref="FormatException">The length of <paramref name="s"/>, is not zero or a multiple of 2.</exception>
        /// <exception cref="FormatException">The format of <paramref name="s"/> is invalid. <paramref name="s"/> contains a non-hex character.</exception>
        public static byte[] FromHexString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            return FromHexString(s.AsSpan());
        }

        /// <summary>
        /// Converts the span, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="chars">The span to convert.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="chars"/>.</returns>
        /// <exception cref="FormatException">The length of <paramref name="chars"/>, is not zero or a multiple of 2.</exception>
        /// <exception cref="FormatException">The format of <paramref name="chars"/> is invalid. <paramref name="chars"/> contains a non-hex character.</exception>
        public static byte[] FromHexString(ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return Array.Empty<byte>();
            if ((uint)chars.Length % 2 != 0)
                throw new FormatException();

            byte[] result = new byte[chars.Length >> 1];

            if (!HexConverter.TryDecodeFromUtf16(chars, result))
                throw new FormatException();

            return result;
        }


        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with uppercase hex characters.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <returns>The string representation in hex of the elements in <paramref name="inArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="inArray"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inArray"/> is too large to be encoded.</exception>
        public static string ToHexString(byte[] inArray)
        {
            if (inArray == null)
                throw new ArgumentNullException(nameof(inArray));

            return ToHexString(new ReadOnlySpan<byte>(inArray));
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with uppercase hex characters.
        /// Parameters specify the subset as an offset in the input array and the number of elements in the array to convert.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="offset">An offset in <paramref name="inArray"/>.</param>
        /// <param name="length">The number of elements of <paramref name="inArray"/> to convert.</param>
        /// <returns>The string representation in hex of <paramref name="length"/> elements of <paramref name="inArray"/>, starting at position <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="inArray"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="length"/> is negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> plus <paramref name="length"/> is greater than the length of <paramref name="inArray"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inArray"/> is too large to be encoded.</exception>
        public static string ToHexString(byte[] inArray, int offset, int length)
        {
            if (inArray == null)
                throw new ArgumentNullException(nameof(inArray));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset > (inArray.Length - length))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return ToHexString(new ReadOnlySpan<byte>(inArray, offset, length));
        }

        /// <summary>
        /// Converts a span of 8-bit unsigned integers to its equivalent string representation that is encoded with uppercase hex characters.
        /// </summary>
        /// <param name="bytes">A span of 8-bit unsigned integers.</param>
        /// <returns>The string representation in hex of the elements in <paramref name="bytes"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/> is too large to be encoded.</exception>
        public static string ToHexString(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length == 0)
                return string.Empty;
            if (bytes.Length > int.MaxValue / 2)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            return HexConverter.ToString(bytes, HexConverter.Casing.Upper);
        }
    }
}
