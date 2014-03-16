using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Generator.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        public class SomeClass
        {
            public string PropertyName { get; set; }

            public string FieldName;
        }

        [TestMethod]
        public void Continue_WithLocalField_SetsField()
        {
            var gen = Gen.Create(WithLocalField);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithLocalField()
        {
            string name = null;
            yield return Gen.Yield(() => name);
            yield return Gen.Yield(name);
        }

        private class Outer
        {
            public Inner InnerThing;

            public class Inner
            {
                public string Field;

                public string Property { get; set; }
            }
        }

        [TestMethod]
        public void Continue_WithNestedField_SetsField()
        {
            var gen = Gen.Create(WithNestedField);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithNestedField()
        {
            var data = new Outer { InnerThing = new Outer.Inner() };
            yield return Gen.Yield(() => data.InnerThing.Field);
            yield return Gen.Yield(data.InnerThing.Field);
        }

        [TestMethod]
        public void Continue_WithNestedProperty_SetsField()
        {
            var gen = Gen.Create(WithNestedProperty);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithNestedProperty()
        {
            var data = new Outer { InnerThing = new Outer.Inner() };
            yield return Gen.Yield(() => data.InnerThing.Property);
            yield return Gen.Yield(data.InnerThing.Property);
        }


        [TestMethod]
        public void Continue_WithParameterProperty_SetsParameter()
        {
            var gen = Gen.Create(WithParameterProperty(new SomeClass()));
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithParameterProperty(SomeClass param)
        {
            yield return Gen.Yield(() => param.PropertyName);
            yield return Gen.Yield(param.PropertyName);
        }


        [TestMethod]
        public void Yield_WithIndexer_SetsParameter()
        {
            var gen = Gen.Create(WithIndexer);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithIndexer()
        {
            var data = new Dictionary<string, string>();
            yield return Gen.Yield(() => data["thing"]);
            yield return Gen.Yield(data["thing"]);
        }


        private static string _staticField;

        [TestMethod]
        public void Yield_WithStaticField_SetsValue()
        {
            var gen = Gen.Create(WithStaticField);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithStaticField()
        {
            yield return Gen.Yield(() => _staticField);
            yield return Gen.Yield(_staticField);
        }


        private static string StaticProperty { get; set; }

        [TestMethod]
        public void Yield_WithStaticProperty_SetsValue()
        {
            var gen = Gen.Create(WithStaticProperty);
            gen.Send("tim");
            Assert.AreEqual("tim", gen.Next<string>());
        }

        public IEnumerable<IYield> WithStaticProperty()
        {
            yield return Gen.Yield(() => _staticField);
            yield return Gen.Yield(_staticField);
        }



        [TestMethod]
        public void Yield_WithWellDefinedTypes_SetsValue()
        {
            var gen = Gen.Create(WithWellDefinedTypes);
            gen.Send(3);
            Assert.AreEqual("3", gen.Next());
        }

        public IEnumerable<IYield<int, string>> WithWellDefinedTypes()
        {
            var value = 0;
            yield return Gen.Yield(() => value);
            yield return Gen<int>.Yield(value.ToString());
        }



        [TestMethod]
        public void Yield_WithValueInFirstYield_ReturnsValue()
        {
            var gen = Gen.Create(WithValueInFirstYield);
            Assert.AreEqual("response", gen.Next<string>());
        }

        public IEnumerable<IYield> WithValueInFirstYield()
        {
            yield return Gen.Yield("response");
        }
    }
}
