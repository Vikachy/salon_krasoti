using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using salon_krasoti;
using System.Windows;
using System.Linq;

namespace CaptchaTests
{
    [TestClass]
    public class CaptchaTests
    {
        [TestMethod]
        public void Login_Fails_WithWrongCredentials()
        {
            var authPage = new AuthPage();

            bool result = authPage.AttemptLogin("wrong", "wrong");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Captcha_Appears_AfterThreeFailedAttempts()
        {
            var authPage = new AuthPage();

            for (int i = 0; i < 3; i++)
            {
                authPage.AttemptLogin("wrong", "wrong");
            }

            Assert.AreEqual(Visibility.Visible, authPage.CaptchaPanel.Visibility);
            Assert.AreEqual(3, authPage.FailedLoginAttempts);
        }

        [TestMethod]
        public void Login_Succeeds_WithCorrectCaptcha()
        {
            var authPage = new AuthPage();

            using (var db = new Entities())
            {
                if (!db.UserAccounts.Any(u => u.Username == "testuser"))
                {
                    var testUser = new UserAccounts
                    {
                        Username = "testuser",
                        PasswordHash = AuthPage.GetHash("testpass"),
                        RoleID = 3
                    };
                    db.UserAccounts.Add(testUser);
                    db.SaveChanges();
                }
            }

            for (int i = 0; i < 3; i++)
            {
                authPage.AttemptLogin("wrong", "wrong");
            }

            bool result = authPage.AttemptLogin("vladbez", "vladbez1", authPage.GeneratedCaptcha);


            Assert.IsTrue(result, "Вход должен быть успешным с правильными данными и CAPTCHA");
            Assert.AreEqual(Visibility.Collapsed, authPage.CaptchaPanel.Visibility);
        }
    }
}
