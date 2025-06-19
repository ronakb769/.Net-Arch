using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.RequestModels
{
    public class UserPagingRequestModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? isActive { get; set; }
        public string? searchText { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } // "asc" / "desc"
    }
}
