using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.ResponseModel
{
    public class PagingResponseModel<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}
