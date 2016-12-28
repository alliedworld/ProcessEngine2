/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
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