using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AutoFixtureSandbox
{
    public class MyClassTests
    {
        [Theory]
        [MyAutoData]
        public void First_instance_is_of_type_BaseImplA(MyClass myClass)
        {
            Assert.IsType<BaseImplA>(myClass.Base);
        }

        [Theory]
        [MyAutoData]
        public void BaseImplA_should_have_Value_populated(MyClass myClass)
        {
            Assert.NotEqual(0, myClass.Base.Value);
        }

        [Theory]
        [MyAutoData]
        public void Second_instance_is_of_type_BaseImplB(MyClass myClass1, MyClass myClass2)
        {
            Assert.IsType<BaseImplB>(myClass2.Base);
        }

        [Theory]
        [MyAutoData]
        public void BaseImplB_should_have_Value_set_to_1(MyClass myClass1, MyClass myClass2)
        {
            Assert.Equal(1, myClass2.Base.Value);
        }
    }

    public abstract class Base
    {
        public string Text { get; set; }
        public abstract int Value { get; set; }
    }

    public class BaseImplA : Base
    {
        public override int Value { get; set; }
    }

    public class BaseImplB : Base
    {
        public override int Value
        {
            get { return 1; }
            set { throw new NotImplementedException(); }
        }
    }

    public class MyClass
    {
        public string Name { get; set; }
        public Base Base { get; set; }
    }

    public class TestCustomization : ICustomization
    {
        private bool _flag;
        private IFixture _fixture;

        public void Customize(IFixture fixture)
        {
            _fixture = fixture;

            fixture.Customize<BaseImplB>(composer =>
            {
                return composer.Without(x => x.Value);
            });

            fixture.Customize<Base>(composer =>
            {
                return composer.FromFactory(CreateBase);
            });
        }

        private Base CreateBase()
        {
            _flag = !_flag;

            if (_flag)
            {
                return _fixture.Create<BaseImplA>();
            }

            return _fixture.Create<BaseImplB>();
        }
    }

    public class MyAutoDataAttribute : AutoDataAttribute
    {
        public MyAutoDataAttribute() : base(new Fixture().Customize(new TestCustomization()))
        {
            
        }
    }
}
