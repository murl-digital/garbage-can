using System.IO;

namespace GarbageCan.Application.Common.Models
{
    public class FileResponse
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public ulong? ReplyMessageId { get; set; }
        public bool ReplyMention { get; set; }
    }
}
