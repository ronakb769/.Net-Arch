using LearnArchitecture.Core.Entities;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class ErrorLogRepository:IErrorLogRepository
    {
        private readonly LearnArchitectureDbContext _context;
        public ErrorLogRepository(LearnArchitectureDbContext context)
        {
            _context = context;
        }
        public async Task LogAsync(ErrorLog log)
        {
            _context.ErrorLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
