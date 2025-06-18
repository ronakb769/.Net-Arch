using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.RequestModels
{
	public class CreateRoleRequestModel
	{
		public int roleId { get; set; }
		public string roleName { get; set; }
		public string description { get; set; }
		public int[] pemissionids { get; set; }
	}
	public class RolePagingRequestModel
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? searchText { get; set; }
		public string? SortColumn { get; set; }
		public string? SortDirection { get; set; } // "asc" / "desc"
	}

}
