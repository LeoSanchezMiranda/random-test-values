﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

namespace RandomTestValues
{
    public static class RandomValue
    {
        private static Dictionary<Type, Func<Type, object>> SupportedTypes =>
            new Dictionary<Type, Func<Type, object>>
            {
                {typeof(int), type => Int()},
                {typeof(string), type => String()},
                {typeof(decimal), type => Decimal()},
                {typeof(double), type => Double()},
                {typeof(bool), type => Bool()},
                {typeof(byte), type => Byte()},
                {typeof(char), type => Char()},
                {typeof(float), type => Float()},
                {typeof(long), type => Long()},
                {typeof(sbyte), type => SByte()},
                {typeof(short), type => Short()},
                {typeof(uint), type => UInt()},
                {typeof(ulong), type => ULong()},
                {typeof(ushort), type => UShort()},
                {typeof(Guid), type => Guid()},
                {typeof(DateTime), type => DateTime()},
                {typeof(TimeSpan), type => TimeSpan() }
            };

        private static readonly Random _Random = new Random();

        /// <summary>
        /// Use for getting a random string for your unit tests.  This is basically a Guid.ToString() so it will
        /// not have any formatting and it will have "-"
        /// </summary>
        /// <returns>A random string the length of a Guid</returns>
        public static string String()
        {
            return Guid().ToString();
        }

        /// <summary>
        /// Use for getting a random string of a specific length for your unit tests.
        /// </summary>
        /// <param name="stringLength">Length of desired random string</param>
        /// <returns>A random string the length of a stringLength parameter</returns>
        public static string String(int stringLength) //Where did the tests go for this method? 
        {
            var randomString = String();

            while (randomString.Length <= stringLength)
            {
                randomString += String();
            }

            return randomString.Substring(0, stringLength);
        }

        /// <summary>
        /// Use for getting a random Byte for your unit tests.
        /// </summary>
        /// <returns>A random Byte</returns>
        public static byte Byte(byte maxPossibleValue = byte.MaxValue)
        {
            return (byte)Int(maxPossibleValue);
        }

        /// <summary>
        /// Use for getting a random Signed Byte for your unit tests.
        /// </summary>
        /// <returns>A random (positive) Signed Byte</returns>
        public static sbyte SByte(sbyte maxPossibleValue = sbyte.MaxValue)
        {
            return (sbyte)Int(maxPossibleValue);
        }

        /// <summary>
        /// Use for getting a random integer for your unit tests.
        /// </summary>
        /// <param name="maxPossibleValue">Maximum expected value (will always be a positive number)</param>
        /// <param name="minPossibleValue">Minumum expected value (will always be a positive number, will default to 0 if larger than maxossibleValue)</param>
        /// <returns>A random (positive) integer</returns>
        public static int Int(int maxPossibleValue = int.MaxValue, int minPossibleValue = 0)
        {
            if (minPossibleValue > maxPossibleValue || minPossibleValue < 0)
            {
                minPossibleValue = 0;
            }

            var max = Math.Abs(maxPossibleValue);

            return _Random.Next(max - minPossibleValue) + minPossibleValue;
        }

        /// <summary>
        /// Use for getting a random Unsigned integer for your unit tests.
        /// </summary>
        /// <returns>A random Unsigned integer</returns>
        public static uint UInt(uint maxPossibleValue = uint.MaxValue)
        {
            var buffer = new byte[sizeof(uint)];
            _Random.NextBytes(buffer);

            var generatedUint = BitConverter.ToUInt32(buffer, 0);

            while (generatedUint > maxPossibleValue)
            {
                generatedUint = generatedUint >> 1;
            }

            return generatedUint;
        }

        /// <summary>
        /// Use for getting a random Short for your unit tests.
        /// </summary>
        /// <returns>A random (positive) Short</returns>
        public static short Short(short maxPossibleValue = short.MaxValue)
        {
            return (short)Int(maxPossibleValue);
        }

        /// <summary>
        /// Use for getting a random Unsigned Short for your unit tests.
        /// </summary>
        /// <returns>A random Unsigned Short</returns>
        public static ushort UShort(ushort maxPossibleValue = ushort.MaxValue)
        {
            return (ushort)Int();
        }

        /// <summary>
        /// Use for getting a random Long for your unit tests.
        /// </summary>
        /// <returns>A random (Positive) Long</returns>
        public static long Long(long maxPossibleValue = long.MaxValue)
        {
            return (long)ULong((ulong)maxPossibleValue);
        }

        /// <summary>
        /// Use for getting a random Unsigned Long for your unit tests.
        /// </summary>
        /// <returns>A random Long</returns>
        public static ulong ULong(ulong maxPossibleValue = ulong.MaxValue)
        {
            var buffer = new byte[sizeof(ulong)];

            _Random.NextBytes(buffer);

            var generatedULongs = BitConverter.ToUInt64(buffer, 0);

            while (generatedULongs > maxPossibleValue)
            {
                generatedULongs = generatedULongs >> 1;
            }

            return generatedULongs;
        }

        public static float Float()
        {
            return (float)_Random.NextDouble();
        }

        public static double Double()
        {
            return _Random.NextDouble();
        }

        public static char Char()
        {
            var buffer = new byte[sizeof(char)];

            _Random.NextBytes(buffer);

            return BitConverter.ToChar(buffer, 0);
        }

        public static bool Bool()
        {
            var randomNumber = Int(2);

            if (randomNumber == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use for getting a random Decimal for your unit test
        /// </summary>
        /// <param name="maxPossibleValue">Maximum decimal value, defaults to 1</param>
        /// <returns></returns>
        public static decimal Decimal(decimal maxPossibleValue = 1m)
        {
            return (decimal)_Random.NextDouble() * maxPossibleValue;
        }

        /// <summary>
        /// Use for getting a random DateTimes for your unit tests. Always returns a date in the past. 
        /// </summary>
        /// <returns>A random DateTime</returns>
        public static DateTime DateTime(DateTime? minDateTime = null, DateTime? maxDateTime = null)
        {
            if (minDateTime == null)
            {
                minDateTime = new DateTime(1610, 1, 7); //discovery of galilean moons. Using system.DateTime.Min just made weird looking dates.
            }

            if (maxDateTime == null)
            {
                maxDateTime = System.DateTime.Now;
            }

            var timeSinceStartOfDateTime = maxDateTime.Value - minDateTime.Value;
            var timeInHoursSinceStartOfDateTime = (int)timeSinceStartOfDateTime.TotalHours;
            var hoursToSubtract = Int(timeInHoursSinceStartOfDateTime) * -1;
            var timeToReturn = maxDateTime.Value.AddHours(hoursToSubtract);

            if (timeToReturn > minDateTime.Value && timeToReturn < maxDateTime.Value)
            {
                return timeToReturn;
            }

            return System.DateTime.Now;
        }

        public static Guid Guid()
        {
            return System.Guid.NewGuid();
        }

        public static TimeSpan TimeSpan()
        {
            var date1 = DateTime();
            var date2 = DateTime();

            return date1 > date2 ? date1.Subtract(date2) : date2.Subtract(date1);
        }

        public static T Object<T>() where T : new()
        {
            var genericObject = (T)Activator.CreateInstance(typeof(T));

            var properties = typeof(T).GetProperties();

            if (properties.Length == 0)
            {
                // Prevent infinite loop when called recursively
                return genericObject;
            }

            foreach (var prop in properties)
            {
                if (prop.SetMethod == null)
                {
                    // Property doesn't have a public setter so let's ignore it
                    continue;
                }

                var method = GetMethodCallAssociatedWithType(prop.PropertyType);

                prop.SetValue(genericObject, method, null);
            }

            return genericObject;
        }

        public static T Enum<T>() where T : struct, IConvertible
        {
            var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);

            var index = _Random.Next(fields.Length);

            return (T)System.Enum.Parse(typeof(T), fields[index].Name, false);
        }

        public static T[] Array<T>(int? optionalLength = null)
        {
            var numberOfItems = optionalLength ?? _Random.Next(1, 10); //Do we care if this is empty or not? I sort of think it would be good if this would be occasionally empty. 

            var enumerable = IEnumerable<T>().Take(numberOfItems);

            var randomArray = new T[numberOfItems];
            for (int i = 0; i < numberOfItems; i++)
            {
                randomArray[i] = enumerable.ElementAt(i);
            }

            return randomArray;
        }

        public static List<T> List<T>(int? optionalLength = null)
        {
            return ICollection<T>(optionalLength).ToList();
        }

        public static IList<T> IList<T>(int? optionalLength = null)
        {
            return ICollection<T>(optionalLength).ToList();
        }

        public static Collection<T> Collection<T>(int? optionalLength = null)
        {
            return (Collection<T>)ICollection<T>(optionalLength);
        }

        public static ICollection<T> ICollection<T>(int? optionalLength = null)
        {
            var numberOfItems = optionalLength ?? _Random.Next(1, 10); //Do we care if this is empty or not? I sort of think it would be good if this would be occasionally empty. 

            var enumerable = IEnumerable<T>().Take(numberOfItems);

            var randomList = new Collection<T>(enumerable.ToList());

            return randomList;
        }

        public static IEnumerable<T> IEnumerable<T>()
        {
            var type = typeof(T);

            var supportType = GetSupportType(type);

            while (supportType != SupportType.NotSupported)
            {
                var method = GetMethodCallAssociatedWithType(type);

                yield return (T)method;
            }
        }

        private static object GetMethodCallAssociatedWithType(Type propertyType)
        {
            var supportType = GetSupportType(propertyType);

            switch (supportType)
            {
                case SupportType.Basic:
                    return SupportedTypes[propertyType].Invoke(propertyType);
                case SupportType.Enum:
                    return EnumMethodCall(propertyType);
                case SupportType.Collection:
                {
                    var collectionType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments()[0];
                    return GetListMethodOfCollections(propertyType, collectionType);
                }
                case SupportType.Nullable:
                    return NullableMethodCall(propertyType, Bool());
                case SupportType.UserDefined:
                    return ObjectMethodCall(propertyType);
                default:
                    return null;
            }
        }

        private static object NullableMethodCall(Type propertyType, bool makeNull)
        {
            // We can enable this if we want but the more I thought about it the more I disliked it.  If somone wants thier
            // nullable field to be null then they can set it equal to null after the fact.  
            // I want consistency in behavior especially when dealing with unit tests.
            // if (makeNull)
            // {
            //     return null;
            // }
            var baseType = propertyType.GetGenericArguments()[0];
            return GetMethodCallAssociatedWithType(baseType);
        }

        private static bool IsNullableType(Type propertyType)
        {
            return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static object GetListMethodOfCollections(Type propertyType, Type genericType)
        {
            var typeOfList = genericType;

            object listMethod = null;

            Type type = propertyType;

            if (propertyType.IsArray)
            {
                listMethod = ArrayMethodCall(propertyType.GetElementType());
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                listMethod = ListMethodCall(typeOfList);
            }
            else if (type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                listMethod = IListMethodCall(typeOfList);
            }
            else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Collection<>)))
            {
                listMethod = CollectionMethodCall(typeOfList);
            }
            else if (type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                listMethod = ICollectionMethodCall(typeOfList);
            }
            else if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                listMethod = IEnumerableMethodCall(typeOfList);
            }

            return listMethod;
        }

        private static bool IsSupportedCollection(Type propertyType)
        {
            return propertyType.GetInterface("ICollection") != null
                   || (propertyType.IsGenericType &&
                       (propertyType.IsArray
                       || propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                       || propertyType.GetGenericTypeDefinition() == typeof(IList<>)
                       || propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                       ));
        }

        private static object ObjectMethodCall(Type type)
        {
            return typeof(RandomValue).GetMethod("Object")
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { });
        }

        private static object EnumMethodCall(Type type)
        {
            return typeof(RandomValue).GetMethod("Enum")
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { });
        }

        private static object IEnumerableMethodCall(Type type)
        {
            return typeof(RandomValue).GetMethod("IEnumerable")
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { });
        }

        private static object ArrayMethodCall(Type typeOfList)
        {
            return typeof(RandomValue).GetMethod("Array")
               .MakeGenericMethod(typeOfList)
               .Invoke(null, new object[] { null });
        }

        private static object ListMethodCall(Type typeOfList)
        {
            return typeof(RandomValue).GetMethod("List")
               .MakeGenericMethod(typeOfList)
               .Invoke(null, new object[] { null });
        }

        private static object IListMethodCall(Type typeOfList)
        {
            return typeof(RandomValue).GetMethod("IList")
                .MakeGenericMethod(typeOfList)
                .Invoke(null, new object[] { null });
        }

        private static object CollectionMethodCall(Type typeOfList)
        {
            return typeof(RandomValue).GetMethod("Collection")
                .MakeGenericMethod(typeOfList)
                .Invoke(null, new object[] { null });
        }

        private static object ICollectionMethodCall(Type typeOfList)
        {
            return typeof(RandomValue).GetMethod("ICollection")
                .MakeGenericMethod(typeOfList)
                .Invoke(null, new object[] { null });
        }

        private static SupportType GetSupportType(Type type)
        {
            if (SupportedTypes.ContainsKey(type))
            {
                return SupportType.Basic;
            }
            if (type.IsEnum)
            {
                return SupportType.Enum;
            }
            if (IsSupportedCollection(type))
            {
                return SupportType.Collection;
            }
            if (IsNullableType(type))
            {
                return SupportType.Nullable;
            }
            if (type.IsClass)
            {
                return SupportType.UserDefined;
            }

            return SupportType.NotSupported;
        }
    }

    internal enum SupportType
    {
        NotSupported,
        UserDefined,
        Basic,
        Enum,
        Collection,
        Nullable
    }
}
