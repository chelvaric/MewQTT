using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.HelperClasses
{
    public static class ArrayExtension
    {
        public static T[] RemoveAt<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (length < 0)
            {
                startIndex += 1 + length;
                length = -length;
            }

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException("length");

            T[] newArray = new T[array.Length - length];

            Array.Copy(array, 0, newArray, 0, startIndex);
            Array.Copy(array, startIndex + length, newArray, startIndex, array.Length - startIndex - length);

            return newArray;
        }
    }
}
