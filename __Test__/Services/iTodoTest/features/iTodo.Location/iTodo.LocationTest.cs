using Newtonsoft.Json;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using TechTalk.SpecFlow;

namespace iTest.Task
{
    [Binding]
    class iTagTest
    {
        [When(@"User add location '(.*)' to task '(.*)' for group '(.*)'")]
        public void WhenUserAddLocationToSelectedTask(string location, string id, string groupKey)
        {
            TestManager.Services.TodoDb.Add(TestHelper.BuildContent.Task.SetLocation(groupKey, id, location));
        }
    }
}

