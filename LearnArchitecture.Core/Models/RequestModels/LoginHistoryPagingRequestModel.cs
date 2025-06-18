using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.RequestModels
{
    public class LoginHistoryPagingRequestModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;


        public List<string>? SelectedUserNames { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } // "asc" / "desc"
        public string? searchText { get; set; } //global search
    }
}
