using iGroup;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iAssistantTest
    {
        [Given(@"Register location service for user '(.*)'")]
        public void GivenRegisterLocationServiceForUser(string member)
        {
            TestManager.Instance.LocationDb.Add(TestHelper.BuildContent.Location.RegisterMember(member, member));
        }

        [Given(@"Simulate location service detect '(.*)' move to '(.*)'")]
        public void GivenSimulateLocationServiceDetectMoveTo(string groupKey, string location)
        {
            TestManager.Instance.LocationDb.Add(TestHelper.BuildContent.Location.TestOnlyLocationChanged(groupKey, location));
        }
    }
}

