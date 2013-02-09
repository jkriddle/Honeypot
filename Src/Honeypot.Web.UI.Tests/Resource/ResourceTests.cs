using System.Threading;
using Honeypot.Web.UI.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Honeypot.Web.UI.Tests.User
{
    /// <summary>
    /// Resource UI Tests
    /// </summary>
    [TestClass]
    public class ResourceTests : BaseTest
    {
        [TestMethod]
        public void Content_Num_Per_Page_Shows_Records()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(u => u.Url.Contains("Manage/Resource"));
                driver.FindElement(By.CssSelector("a.select2-choice")).Click();
                driver.FindElement(By.CssSelector(".select2-result-selectable:nth-child(4)")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > DefaultNumPerPage);
                Assert.IsTrue(driver.FindElementsByCssSelector(".table tbody tr").Count > DefaultNumPerPage);
            }
        }
        
        [TestMethod]
        public void Search_Content_By_Name_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".searchTerm")).SendKeys("resource-30");
                driver.FindElement(By.CssSelector(".table-filter button")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count < DefaultNumPerPage);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Name_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""Name""]').val('resource-27')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Type_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('select[name=""ResourceType""]').val('String')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var td = driver.FindElementByCssSelector(".table tbody tr td:first-child");
                Assert.AreEqual("100", td.Text);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_Value_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""Value""]').val('27')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                Assert.AreEqual(1, driver.FindElementsByCssSelector(".table tbody tr").Count);
            }
        }

        [TestMethod]
        public void Advanced_Search_By_ReadOnly_Filters_Results()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".advanced")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count > 0);
                (driver as IJavaScriptExecutor).ExecuteScript(@"$('input[name=""IsReadOnly""]').attr('checked', 'checked')");
                ModalHelper.ClickPrimary(driver);
                driver.WaitFor(d => d.FindElements(By.CssSelector(".modal-body")).Count == 0);
                var td = driver.FindElementByCssSelector(".table tbody tr td:first-child");
                Assert.AreEqual("100", td.Text);
            }
        }
        
        [TestMethod]
        public void Paging_Content_Shows_Next_page()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".page-2")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table .resource-row-30")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector(".table .resource-row-30")));
            }
        }

        [TestMethod]
        public void View_Content_Details_Shows_Resource_Info()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".resource-row-1 a[title='View resource']")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector("#viewResourceForm .form-horizontal")).Count > 0);
                Assert.IsTrue(driver.FindElementByCssSelector("#viewResourceForm").Text.Contains("resource-1"));
            }
        }

        [TestMethod]
        public void Cannot_Edit_ReadOnly_Content_Resource()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".resource-row-1 a[title='Edit resource']")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector("#editResourceForm .form-horizontal")).Count > 0);
                Assert.AreEqual(0, driver.FindElementsByCssSelector("#editResourceForm .form-horizontal .btn-submit").Count);
            }
        }

        [TestMethod]
        public void Can_Edit_Content_Resource()
        {
            using (var driver = BrowserFactory.Create())
            {
                SignInAsAdmin(driver);
                driver.FindElement(By.PartialLinkText("Resources")).Click();
                driver.FindElement(By.PartialLinkText("Content")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 1);
                driver.FindElement(By.CssSelector(".resource-row-26 a[title='Edit resource']")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector("#editResourceForm .form-horizontal")).Count > 0);
                driver.FindElement(By.CssSelector("#editResourceForm .form-horizontal .btn-primary")).Click();
                driver.WaitFor(d => d.FindElements(By.CssSelector("#editResourceForm .alert-success")).Count > 0);
                Assert.IsTrue(driver.ElementExists(By.CssSelector("#editResourceForm .alert-success")));
            }
        }
    }
}
