using PotentHelper;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iTagTest
    {
        [When(@"User add tag '(.*)' '(.*)' to task '(.*)' for group '(.*)'")]
        public void WhenUserAddLocationToSelectedTask(string tagKey, string tag, string id, string groupKey)
        {
            TestManager.Services.TodoDb.Add(TestHelper.BuildContent.Task.SetTag(groupKey, id, tagKey, tag));
        }
    }
}

