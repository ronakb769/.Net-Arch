using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class AppSettings
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value{ get; set; }
        public string? description { get;set; }
    }
}
