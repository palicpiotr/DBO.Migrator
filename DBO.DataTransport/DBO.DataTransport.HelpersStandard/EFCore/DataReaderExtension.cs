using System;
using System.Collections.Generic;
using System.Data;

namespace DBO.DataTransport.HelpersStandard.EFCore
{
    public static class DataReaderExtension
    {
        public class EnumeratorWrapper<T>
        {
            private readonly Func<bool> _moveNext;
            private readonly Func<T> _current;

            public EnumeratorWrapper(Func<bool> moveNext, Func<T> current)
            {
                _moveNext = moveNext;
                _current = current;
            }

            public EnumeratorWrapper<T> GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                return _moveNext();
            }

            public T Current => _current();
        }

        private static IEnumerable<T> BuildEnumerable<T>(
            Func<bool> moveNext, Func<T> current)
        {
            var po = new EnumeratorWrapper<T>(moveNext, current);
            foreach (var s in po)
                yield return s;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T source) where T : IDataReader
        {
            return BuildEnumerable(source.Read, () => source);
        }

        public static T GetValueOrDefault<T>(this IDataRecord row, string fieldName)
        {
            int ordinal = row.GetOrdinal(fieldName);
            return row.GetValueOrDefault<T>(ordinal);
        }

        public static T GetValueOrDefault<T>(this IDataRecord row, int ordinal)
        {
            if (row.IsDBNull(ordinal)) return default(T);
            var t = typeof(T);
            var value = row.GetValue(ordinal);
            if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Nullable<>))
                return (T)Convert.ChangeType(value, t);

            if (value == null)
                return default(T);

            t = Nullable.GetUnderlyingType(t);
            return (T)Convert.ChangeType(value, t);
        }
    }
}
