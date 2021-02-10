using Microsoft.EntityFrameworkCore;
using SoaApp.Models;
using SoaApp.Utilities;

namespace Intranet.DataAccess.Data
{
    public class SOADbContext : DbContext
    {
        public SOADbContext(DbContextOptions<SOADbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(SD.SOAConString);
        }

        public DbSet<ADRC> ADRCs { get; set; }
        public DbSet<BKPF> BKPFs { get; set; }
        public DbSet<BSAD> BSADs { get; set; }
        public DbSet<BSEG> BSEGs { get; set; }
        public DbSet<BSID> BSIDs { get; set; }
        public DbSet<KNA1> KNA1s { get; set; }
        public DbSet<KNB1> KNB1s { get; set; }
        public DbSet<KNKK> KNKKs { get; set; }
        public DbSet<KNVK> KNVKs { get; set; }
        public DbSet<R_BKPF> R_BKPFs { get; set; }
        public DbSet<T001> T001s { get; set; }
        public DbSet<T014> T014s { get; set; }
        public DbSet<T052> T052s { get; set; }
    }
}