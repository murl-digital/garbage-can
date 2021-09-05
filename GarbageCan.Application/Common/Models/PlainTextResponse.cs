namespace GarbageCan.Application.Common.Models
{
    public class PlainTextResponse
    {
        public string Message { get; set; }
        public bool PrependEmoji { get; set; }
        public bool FormatAsBlock { get; set; }
    }
}
