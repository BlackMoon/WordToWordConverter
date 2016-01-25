using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordToWordConverter.Validation;

namespace WordToWordConverter.Tests
{
    [TestClass]
    public class StringValidatorTest
    {
        [TestMethod]
        public void TestValidate()
        {
            string msg;

            IStringValidator validator = new StringValidator(){ Value = "123"} ;
            Assert.IsFalse(validator.Validate(out msg));

            validator.Value = "нота1";
            Assert.IsFalse(validator.Validate(out msg));

            validator.Value = "нора";
            Assert.IsTrue(validator.Validate(out msg));
        }
    }
}
