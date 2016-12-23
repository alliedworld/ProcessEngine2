using System;
using KlaudWerk.ProcessEngine.Builder;
using NUnit.Framework;

namespace KlaudWerk.ProcessEngine.Test
{

    [TestFixture]
    public class SecurityBuilderTest
    {
        [Test]
        public void TestAddNewPotentialOwners()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                AccountTypeEnum.Group)).
                AddPotentialOwners(new Tuple<string, AccountTypeEnum>("assistants",
                AccountTypeEnum.Role));
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(2,sb.PotentialOwners.Count);
        }

        [Test]
        public void TestAddNewBusinessAdministrators()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("assistants",
                    AccountTypeEnum.Role));
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(2,sb.BusinessAdministrators.Count);
        }

        [Test]
        public void DuplicatePotentialOwnerNameShouldBeAddedOnce()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group));
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(1,sb.PotentialOwners.Count);
        }
        [Test]
        public void DuplicateBusinessAdminNameShouldBeAddedOnce()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group));
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(1,sb.BusinessAdministrators.Count);

        }
        [Test]
        public void DifferentAccountTypesWithThesameNameShouldBeAddedToPotentialOwner()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Role));
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(2,sb.PotentialOwners.Count);
        }
        [Test]
        public void DifferentAccountTypesWithThesameNameShouldBeAddedToBusinessAdmin()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Role));
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(2,sb.BusinessAdministrators.Count);
        }

        [Test]
        public void CallToDoneShouldReturnParentBuilder()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            Assert.AreEqual(builder,sb.Done());
        }

        [Test]
        public void ShouldBeAbleToAddItemsUsingFluentInterface()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Role));
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(1,sb.PotentialOwners.Count);
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(1,sb.BusinessAdministrators.Count);

        }
        [Test]
        public void CallToClearShouldClearAllItems()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            sb.AddPotentialOwners(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Group)).
                AddBusinessAdministrators(new Tuple<string, AccountTypeEnum>("underwriters",
                    AccountTypeEnum.Role));
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(1,sb.PotentialOwners.Count);
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(1,sb.BusinessAdministrators.Count);
            sb.Clear();
            Assert.IsNotNull(sb.PotentialOwners);
            Assert.AreEqual(0,sb.PotentialOwners.Count);
            Assert.IsNotNull(sb.BusinessAdministrators);
            Assert.AreEqual(0,sb.BusinessAdministrators.Count);
        }

        [Test]
        public void EmptyPotentialShouldThrowException()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            Assert.Throws<ArgumentException>(() =>
            {
                sb.AddPotentialOwners();
            });

        }

        [Test]
        public void EmptyBusinessAdminsShouldThrowException()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            Assert.Throws<ArgumentException>(() =>
            {
                sb.AddBusinessAdministrators();
            });


        }
        [Test]
        public void NullPotentialShouldThrowException()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            Assert.Throws<ArgumentNullException>(() =>
            {
                sb.AddPotentialOwners(null);
            });

        }

        [Test]
        public void NullBusinessAdminsShouldThrowException()
        {
            var builder = new StepBuilder(parent:new ProcessBuilder(id:"p_1"),id: "123", name: "Start", description: "Start Step");
            SecurityBuilder<StepBuilder> sb=new SecurityBuilder<StepBuilder>(builder);
            Assert.Throws<ArgumentNullException>(() =>
            {
                sb.AddBusinessAdministrators(null);
            });

        }

    }
}