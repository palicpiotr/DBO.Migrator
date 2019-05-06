using DBO.DataTransport.HelpersStandard.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DBO.DataTransport.HelpersStandard.Proxy
{
    public class DBContextWrapper<Context> where Context : DbContext
    {
        private Context _context;

        public DBContextWrapper()
        { }

        public void Init(Context context) => _context = context;

        public DBContextWrapper(Context context)
        {
            Init(context);
        }

        public virtual Context DbContext => _context;

        public virtual TransactionWrapper BeginTransaction() => new TransactionWrapper(_context.Database.BeginTransaction());

        public virtual void SaveChanges() => _context.SaveChanges();
        public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public virtual int SaveChangesWithLog(NLog.ILogger logger) => _context.SaveChangesWithLog(logger);
    }
}
