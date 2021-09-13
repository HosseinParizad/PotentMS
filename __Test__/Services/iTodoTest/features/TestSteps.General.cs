using PotentHelper;
using TechTalk.SpecFlow;

namespace iTest
{
    [Binding]
    internal class TestManager
    {
        [BeforeScenario]
        public virtual void BeforeScenario()
        {
            ProducerHelper.OnSendAMessageEvent += ProducerHelper_OnSendAMessageEvent;

            iTodo.Engine.Reset(null, null);
            iTime.Engine.Reset(null, null);
            iGroup.Engine.Reset(null, null);
            iMemory.Engine.Reset(null, null);
        }

        [AfterScenario]
        public virtual void AfterScenario()
        {
            ProducerHelper.OnSendAMessageEvent -= ProducerHelper_OnSendAMessageEvent;
        }

        private void ProducerHelper_OnSendAMessageEvent(object sender, SendAMessageEventArgs e)
        {
            instance.LastMessage = new SendAMessageEventArgs(e.Topic, e.Message);
        }

        public DbText TodoDb => Services.TodoDb;
        public DbText TimeDb => Services.TimeDb;
        public DbText GroupDb => Services.GroupDb;
        public DbText MemoryDb => Services.MemoryDb;
        public static ServiceContiner Services = new ServiceContiner();
        public SendAMessageEventArgs LastMessage { set; get; }

        public static TestManager Instance => instance ?? (instance = new TestManager());
        static TestManager instance;
    }


    internal class ServiceContiner
    {
        public ServiceContiner()
        {
            TodoActions = new();
            TimeActions = new();
            GroupActions = new();
            MemoryActions = new();
            TimeActions.Ini();
            TodoActions.Ini();
            GroupActions.Ini();
            MemoryActions.Ini();
        }

        public iTodo.SetupActions TodoActions;
        public iTime.SetupActions TimeActions;
        public iGroup.SetupActions GroupActions;
        public iMemory.SetupActions MemoryActions;

        public DbText TodoDb => TodoActions.db;
        public DbText TimeDb => TimeActions.db;
        public DbText GroupDb => GroupActions.db;
        public DbText MemoryDb => MemoryActions.db;
    }
}