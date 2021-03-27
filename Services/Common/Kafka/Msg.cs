namespace KafkaHelper
{
    public class Msg : IMessageContract
    {
        public Msg(string belongTo, string content)
        {
            if (belongTo is null)
            {
                throw new System.ArgumentNullException(nameof(belongTo));
            }

            if (content is null)
            {
                throw new System.ArgumentNullException(nameof(content));
            }
            BelongTo = belongTo;
            Content = content;
        }

        public string Content { get; }
        public string BelongTo { get; }
    }
}