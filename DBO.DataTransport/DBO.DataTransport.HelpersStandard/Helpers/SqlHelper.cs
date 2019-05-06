using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace DBO.DataTransport.HelpersStandard.Helpers
{
    public static class SqlHelper
    {
        public static T GetValue<T>(this SqlXml sqlXml)
        {
            T value;

            // using System.Xml;
            using (XmlReader xmlReader = sqlXml.CreateReader())
            {
                // using System.Xml.Serialization;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                value = (T)xmlSerializer.Deserialize(xmlReader);
            }

            return value;
        }

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
