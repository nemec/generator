using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GeneratorAsync.UnitTest
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
            using (var gen = new Generator(WithLocalField))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithLocalField(IYield gen)
        {
            var name = (string)await gen.Yield();
            await gen.Yield(name);
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
            using (var gen = new Generator(WithNestedField))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithNestedField(IYield gen)
        {
            var data = new Outer { InnerThing = new Outer.Inner() };
            data.InnerThing.Field = (string)await gen.Yield();
            await gen.Yield(data.InnerThing.Field);
        }

        [TestMethod]
        public void Continue_WithNestedProperty_SetsField()
        {
            using (var gen = new Generator(WithNestedProperty))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithNestedProperty(IYield gen)
        {
            var data = new Outer { InnerThing = new Outer.Inner() };
            data.InnerThing.Property = (string)await gen.Yield();
            await gen.Yield(data.InnerThing.Property);
        }


        [TestMethod]
        public void Continue_WithParameterProperty_SetsParameter()
        {
            using (var gen = new Generator<SomeClass>(WithParameterProperty, new SomeClass()))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithParameterProperty(IYield gen, SomeClass param)
        {
            param.PropertyName = (string)await gen.Yield();
            await gen.Yield(param.PropertyName);
        }


        [TestMethod]
        public void Yield_WithIndexer_SetsParameter()
        {
            using (var gen = new Generator(WithIndexer))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithIndexer(IYield gen)
        {
            var data = new Dictionary<string, string>();
            data["thing"] = (string)await gen.Yield();
            await gen.Yield(data["thing"]);
        }


        private static string _staticField;

        [TestMethod]
        public void Yield_WithStaticField_SetsValue()
        {
            using (var gen = new Generator(WithStaticField))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithStaticField(IYield gen)
        {
            _staticField = (string)await gen.Yield();
            await gen.Yield(_staticField);
        }


        private static string StaticProperty { get; set; }

        [TestMethod]
        public void Yield_WithStaticProperty_SetsValue()
        {
            using (var gen = new Generator(WithStaticProperty))
            {
                gen.Send("tim");
                Assert.AreEqual("tim", gen.Next<string>());
            }
        }

        public async Task WithStaticProperty(IYield gen)
        {
            StaticProperty = (string)await gen.Yield();
            await gen.Yield(StaticProperty);
        }



        [TestMethod]
        public void Yield_WithWellDefinedTypes_SetsValue()
        {
            using (var gen = new Generator<int, string>(WithWellDefinedTypes))
            {
                gen.Send(3);
                Assert.AreEqual("3", gen.Next());
            }
        }

        public async Task WithWellDefinedTypes(IYield<int, string> gen)
        {
            var value = await gen.Yield();
            await gen.Yield(value.ToString());
        }



        [TestMethod]
        public void Yield_WithValueInFirstYield_ReturnsValue()
        {
            using (var gen = new Generator(WithValueInFirstYield))
            {
                Assert.AreEqual("response", gen.Next<string>());
            }
        }

        public async Task WithValueInFirstYield(IYield gen)
        {
            await gen.Yield("response");
        }



        [TestMethod]
        public void AsEnumerable_WithTenYields_ReturnsCountOfTen()
        {
            using (var gen = new Generator(WithTenYields))
            {
                var actual = gen.AsEnumerable<string>().Count();
                Assert.AreEqual(10, actual);
            }
        }

        [TestMethod]
        public void AsEnumerable_WithTenYieldsAndTakingEleven_ReturnsLengthOfTen()
        {
            using (var gen = new Generator(WithTenYields))
            {
                var actual = gen
                    .AsEnumerable<string>()
                    .Take(11)
                    .ToList()
                    .Count;
                Assert.AreEqual(10, actual);
            }
        }

        public async Task WithTenYields(IYield gen)
        {
            for (var i = 0; i < 10; i++)
            {
                await gen.Yield("response");
            }
        }
    }
}
