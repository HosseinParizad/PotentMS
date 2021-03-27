namespace Gateway.Controllers
{
    public class GatewayMessage
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public object Extra { get; set; }

        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
    }
}