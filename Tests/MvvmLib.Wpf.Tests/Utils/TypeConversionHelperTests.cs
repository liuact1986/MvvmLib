using System;
using System.Globalization;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Utils;

namespace MvvmLib.Wpf.Tests.Utils
{

    [TestClass]
    public class TypeConversionHelperTests
    {
        [TestMethod]
        public void Converts()
        {
            // numbers
            Assert.AreEqual((int)10, TypeConversionHelper.TryConvert(typeof(int), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((short)10, TypeConversionHelper.TryConvert(typeof(short), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((uint)10, TypeConversionHelper.TryConvert(typeof(uint), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((double)10.5, TypeConversionHelper.TryConvert(typeof(double), "10.5", CultureInfo.InvariantCulture));
            // bool
            Assert.AreEqual(true, TypeConversionHelper.TryConvert(typeof(bool), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, TypeConversionHelper.TryConvert(typeof(bool), "true", CultureInfo.InvariantCulture));
            // enum
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.TryConvert(typeof(MyEnum), "1", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.TryConvert(typeof(MyEnum), "Two", CultureInfo.InvariantCulture));
            // nullables
            Assert.AreEqual((int?)10, TypeConversionHelper.TryConvert(typeof(int?), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, TypeConversionHelper.TryConvert(typeof(bool?), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.TryConvert(typeof(MyEnum?), "1", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.TryConvert(typeof(MyEnum?), "Two", CultureInfo.InvariantCulture));
            // dattetime
            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.InvariantCulture); // 18/07/2019 00:00:00 => invariant 07/18/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.TryConvert(typeof(DateTime), dateAsString, CultureInfo.InvariantCulture));
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.TryConvert(typeof(DateTime), dateAsStringFr, CultureInfo.GetCultureInfo("fr")));

            // uri
            Assert.AreEqual(new Uri("http://mysite.com/"), TypeConversionHelper.TryConvert(typeof(Uri), "http://mysite.com/", CultureInfo.GetCultureInfo("fr")));

            // guid
            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, TypeConversionHelper.TryConvert(typeof(Guid), guid.ToString(), CultureInfo.InvariantCulture));

            // timespan
            var ts = new TimeSpan(10, 32, 28);
            Assert.AreEqual(ts, TypeConversionHelper.TryConvert(typeof(TimeSpan), ts.ToString(), CultureInfo.InvariantCulture));

            // char
            Assert.AreEqual('c', TypeConversionHelper.TryConvert(typeof(char), "c", CultureInfo.InvariantCulture));

            // byte[]
            // Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, TypeConversionHelper.TryConvert(typeof(byte[]), "\"AQIDBA==\"", CultureInfo.InvariantCulture));

            // brush
            Assert.AreEqual("#FFFF0000", TypeConversionHelper.TryConvert(typeof(Brush), "Red", CultureInfo.InvariantCulture).ToString());
        }

        [TestMethod]
        public void Converts_With_ConvertChangeType()
        {
            // numbers
            Assert.AreEqual((int)10, TypeConversionHelper.ConvertWithChangeType(typeof(int), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((short)10, TypeConversionHelper.ConvertWithChangeType(typeof(short), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((uint)10, TypeConversionHelper.ConvertWithChangeType(typeof(uint), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((double)10.5, TypeConversionHelper.ConvertWithChangeType(typeof(double), "10.5", CultureInfo.InvariantCulture));
            // bool
            Assert.AreEqual(true, TypeConversionHelper.ConvertWithChangeType(typeof(bool), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, TypeConversionHelper.ConvertWithChangeType(typeof(bool), "true", CultureInfo.InvariantCulture));

            //  Fails on enum, nullables, uri, timespan, brush, ...

            // enum
            //Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithChangeType(typeof(MyEnum), "1", CultureInfo.InvariantCulture));
            //Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithChangeType(typeof(MyEnum), "Two", CultureInfo.InvariantCulture));
            // nullables
            //Assert.AreEqual((int?)10, TypeConversionHelper.ConvertWithChangeType(typeof(int?), "10", CultureInfo.InvariantCulture));
            //Assert.AreEqual(true, TypeConversionHelper.ConvertWithChangeType(typeof(bool?), "true", CultureInfo.InvariantCulture));
            //Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithChangeType(typeof(MyEnum?), "1", CultureInfo.InvariantCulture));
            //Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithChangeType(typeof(MyEnum?), "Two", CultureInfo.InvariantCulture));
            // dattetime
            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.InvariantCulture); // 18/07/2019 00:00:00 => invariant 07/18/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.ConvertWithChangeType(typeof(DateTime), dateAsString, CultureInfo.InvariantCulture));
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.ConvertWithChangeType(typeof(DateTime), dateAsStringFr, CultureInfo.GetCultureInfo("fr")));

            // uri
            //  Assert.AreEqual(new Uri("http://mysite.com/"), TypeConversionHelper.ConvertWithChangeType(typeof(Uri), "http://mysite.com/", CultureInfo.GetCultureInfo("fr")));

            // guid
            //var guid = Guid.NewGuid();
            //Assert.AreEqual(guid, TypeConversionHelper.ConvertWithChangeType(typeof(Guid), guid.ToString(), CultureInfo.InvariantCulture));

            // timespan
            //var ts = new TimeSpan(10, 32, 28);
            //Assert.AreEqual(ts, TypeConversionHelper.ConvertWithChangeType(typeof(TimeSpan), ts.ToString(), CultureInfo.InvariantCulture));

            // char
            Assert.AreEqual('c', TypeConversionHelper.ConvertWithChangeType(typeof(char), "c", CultureInfo.InvariantCulture));

            // byte[]
            // Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, TypeConversionHelper.ConvertWithChangeType(typeof(byte[]), "\"AQIDBA==\"", CultureInfo.InvariantCulture));

            // brush
            //Assert.AreEqual("#FFFF0000", TypeConversionHelper.ConvertWithChangeType(typeof(Brush), "Red", CultureInfo.InvariantCulture).ToString());
        }

        [TestMethod]
        public void Can_Converts_With_ConvertChangeType()
        {
            // https://referencesource.microsoft.com/#mscorlib/system/convert.cs,fc990bd1275d43d6
            // numbers
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(int)));
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(short)));
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(uint)));
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(double)));
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(byte)));
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(sbyte)));
            // bool
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(bool)));

            //  Fails on enum, nullables, uri, timespan, brush, ...

            // enum
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(MyEnum)));
            // nullables
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(int?)));
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(short?)));
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(bool?)));

            // dattetime
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(DateTime)));

            // uri
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(Uri)));
            //  Assert.AreEqual(new Uri("http://mysite.com/"), TypeConversionHelper.ConvertWithChangeType(typeof(Uri), "http://mysite.com/", CultureInfo.GetCultureInfo("fr")));

            // guid
            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(Guid)));
            // timespan
            //var ts = new TimeSpan(10, 32, 28);
            //Assert.AreEqual(ts, TypeConversionHelper.ConvertWithChangeType(typeof(TimeSpan), ts.ToString(), CultureInfo.InvariantCulture));

            // char
            Assert.AreEqual(true, TypeConversionHelper.CanConvertWithChangeType(typeof(char)));

            Assert.AreEqual(false, TypeConversionHelper.CanConvertWithChangeType(typeof(Brush)));
        }

        [TestMethod]
        public void Converts_With_TypeConverter()
        {
            // numbers
            Assert.AreEqual((int)10, TypeConversionHelper.ConvertWithTypeConverter(typeof(int), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((short)10, TypeConversionHelper.ConvertWithTypeConverter(typeof(short), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((uint)10, TypeConversionHelper.ConvertWithTypeConverter(typeof(uint), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual((double)10.5, TypeConversionHelper.ConvertWithTypeConverter(typeof(double), "10.5", CultureInfo.InvariantCulture));
            // bool
            Assert.AreEqual(true, TypeConversionHelper.ConvertWithTypeConverter(typeof(bool), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, TypeConversionHelper.ConvertWithTypeConverter(typeof(bool), "true", CultureInfo.InvariantCulture));
            // enum
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithTypeConverter(typeof(MyEnum), "1", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithTypeConverter(typeof(MyEnum), "Two", CultureInfo.InvariantCulture));
            // nullables
            Assert.AreEqual((int?)10, TypeConversionHelper.ConvertWithTypeConverter(typeof(int?), "10", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, TypeConversionHelper.ConvertWithTypeConverter(typeof(bool?), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithTypeConverter(typeof(MyEnum?), "1", CultureInfo.InvariantCulture));
            Assert.AreEqual(MyEnum.Two, TypeConversionHelper.ConvertWithTypeConverter(typeof(MyEnum?), "Two", CultureInfo.InvariantCulture));
            // dattetime
            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.InvariantCulture); // 18/07/2019 00:00:00 => invariant 07/18/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.ConvertWithTypeConverter(typeof(DateTime), dateAsString, CultureInfo.InvariantCulture));
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00
            Assert.AreEqual(date, TypeConversionHelper.ConvertWithTypeConverter(typeof(DateTime), dateAsStringFr, CultureInfo.GetCultureInfo("fr")));

            // uri
            Assert.AreEqual(new Uri("http://mysite.com/"), TypeConversionHelper.ConvertWithTypeConverter(typeof(Uri), "http://mysite.com/", CultureInfo.GetCultureInfo("fr")));

            // guid
            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, TypeConversionHelper.ConvertWithTypeConverter(typeof(Guid), guid.ToString(), CultureInfo.InvariantCulture));

            // timespan
            var ts = new TimeSpan(10, 32, 28);
            Assert.AreEqual(ts, TypeConversionHelper.ConvertWithTypeConverter(typeof(TimeSpan), ts.ToString(), CultureInfo.InvariantCulture));

            // char
            Assert.AreEqual('c', TypeConversionHelper.ConvertWithTypeConverter(typeof(char), "c", CultureInfo.InvariantCulture));

            // byte[]
            // Assert.AreEqual(new byte[] { 1, 2, 3, 4 }, TypeConversionHelper.ConvertWithTypeConverter(typeof(byte[]), "\"AQIDBA==\"", CultureInfo.InvariantCulture));

            // brush
            Assert.AreEqual("#FFFF0000", TypeConversionHelper.ConvertWithTypeConverter(typeof(Brush), "Red", CultureInfo.InvariantCulture).ToString());
        }
    }

    public enum MyEnum
    {
        One,
        Two,
        Three
    }
}
