using Newtonsoft.Json;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iTimeTest
    {
        string GroupKey = Guid.NewGuid().ToString();
        string SelectedId = Guid.NewGuid().ToString();

        [Given(@"User have a selected service")]
        public void UserHaveaSelectedService()
        {
            TestManager.Instance.TodoDb.Add(TestHelper.BuildContent.Task.NewTask(GroupKey, SelectedId, "Do want ever you want!", ""));
        }


        [When(@"User '(.*)' selected task")]
        public void WhenUserPauseSelectedTask(string action)
        {
            switch (action)
            {
                case "Start":
                    TestManager.Instance.TimeDb.Add(TestHelper.BuildContent.Time.StartTime(GroupKey, SelectedId, "Task", "Hossein"));
                    break;
                case "Pause":
                    TestManager.Instance.TimeDb.Add(TestHelper.BuildContent.Time.PauseTime(GroupKey, SelectedId, "Task", "Hossein"));
                    break;
                case "Done":
                    TestManager.Instance.TimeDb.Add(TestHelper.BuildContent.Time.DoneTime(GroupKey, SelectedId, "Task", "Hossein"));
                    break;
                default:
                    break;
            }

        }

        private const string ignoreContent = "do not care";

        [Then(@"I get feedback action '(.*)'")]
        public void ThenAssertFeedback(string expectedAction)
        {
            AssertFeedback(expectedAction, ignoreContent);
        }

        [Then(@"I get feedback '(.*)' with content '(.*)'")]
        public void ThenAssertFeedbackWithContent(string expectedAction, string expectedContent)
        {
            AssertFeedback(expectedAction, expectedContent);
        }

        static void AssertFeedback(string expectedAction, string expectedContent)
        {
            Assert.AreEqual(expectedAction, TestManager.Instance.LastMessage.Message.Action);
            if (expectedContent != ignoreContent)
            {
                Assert.AreEqual(expectedContent, JsonConvert.SerializeObject(TestManager.Instance.LastMessage.Message.Content));
            }
        }

        [Then(@"I get a feedback '(.*)' and content contain '(.*)'")]
        public void ThenAssertFeedbackContentContain(string expectedAction, string expectedContent)
        {
            Assert.AreEqual(expectedAction, TestManager.Instance.LastMessage.Message.Action);
            var content = JsonConvert.SerializeObject(TestManager.Instance.LastMessage.Message.Content);
            Assert.AreEqual(true, content.IndexOf(expectedContent) > -1, content);
        }
    }
}

