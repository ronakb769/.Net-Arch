using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.RequestModels
{
    public class CreateUserRoleRequestModel
    {
        public int userId {get; set;}
        public string firstName {get; set; }
        public string lastName {get; set;}
        public string email {get; set;}
        public string password {get; set;}
        public string phone {get; set;}
        public IFormFile? profile {get; set;}
        public int roleId {get; set;}
        //public bool isActive {get; set;}
        //public bool isDelete {get; set;}
        //public int createdBy { get;  set; }
        //public DateTime createdOn {get; set;}
        //public int? updatedBy  {get; set;}
        //public DateTime? updatedOn {get; set;}
    }
}
