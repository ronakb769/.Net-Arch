using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class Users: CommonField
    {
        [Key]
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }    
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string? profileUrl { get; set; }

    }
}
