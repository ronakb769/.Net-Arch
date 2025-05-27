using LearnArchitecture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IErrorLogRepository
    {
        Task LogAsync(ErrorLog log);
    }
}
