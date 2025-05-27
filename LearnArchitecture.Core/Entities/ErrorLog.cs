using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
