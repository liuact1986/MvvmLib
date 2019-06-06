using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{
    [TestClass]
    public class ModelWrapperTests
    {
        [TestMethod]
        public void Validate()
        {
            var user = new UserModel
            {
                FirstName = "M",
                LastName = "Bellin"
            };

            var wrapper = new UserModelWrapper(user);

            wrapper.ValidateProperty("FirstName");

            var errors = wrapper.GetErrors("FirstName");
            var r = errors.Cast<string>().ToList();

            Assert.IsTrue(wrapper.HasErrors);
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("FirstName too short", r[0]);

            wrapper.FirstName = "Marie";

            Assert.IsFalse(wrapper.HasErrors);

            Assert.AreEqual("Marie", wrapper.Model.FirstName);
            Assert.AreEqual("Marie", user.FirstName);
        }
    }


    public class UserModel
    {
        [Required(ErrorMessage = "FirstName required")]
        [MinLength(2, ErrorMessage = "FirstName too short")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName required")]
        public string LastName { get; set; }
    }

    public class UserModelWrapper : ModelWrapper<UserModel>
    {
        public UserModelWrapper(UserModel model)
            : base(model)
        { }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
