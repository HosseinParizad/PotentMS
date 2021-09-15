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


        [Then(@"I get feedback '(.*)'")]
        public void ThenIGetFeedbackTaskPaused(string expectedResult)
        {
            //Assert.AreEqual("TimeFeedback", TestManager.Instance.LastMessage.Topic);
            Assert.AreEqual(expectedResult, TestManager.Instance.LastMessage.Message.Action);
        }
    }
}

