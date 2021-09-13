using Newtonsoft.Json;
using NUnit.Framework;
using PotentHelper;
using SpecFlowDemo.Steps;
using System;
using TechTalk.SpecFlow;

namespace iTest.iLocation
{
    [Binding]
    class iLocationTest
    {
        [When(@"User add location '(.*)' to selected task")]
        public void WhenUserAddLocationToSelectedTask(string p0)
        {
            //iTodo.db.Add(TestHelper.BuildContent.Memory.NewMemory(groupKey, id, text, hint, parentId));
        }
    }
}

