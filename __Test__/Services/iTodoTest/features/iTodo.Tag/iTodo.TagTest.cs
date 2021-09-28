using PotentHelper;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    class iTagTest
    {
        [When(@"User add tag '(.*)' '(.*)' to task '(.*)' for member '(.*)' in group '(.*)'")]
        public void WhenUserAddLocationToSelectedTask(string tagKey, string tag, string id, string memberKey, string groupKey)
        {
            TestManager.Services.TodoDb.Add(TestHelper.BuildContent.Task.SetTag(groupKey, memberKey, id, tagKey, tag));
        }
    }
}

