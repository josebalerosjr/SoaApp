using Microsoft.EntityFrameworkCore;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using SoaApp.Utilities;

namespace Intranet.DataAccess.Data
{
    public class SOADbContext : DbContext
    {
        public SOADbContext()
        {

        }
        public SOADbContext(DbContextOptions<SOADbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(SD.SoaConString);
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
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BKPFNew> BKPFNews{ get; set; }
        public DbSet<BSADNew> BSADNews{ get; set; }
        public DbSet<BSEGNew> BSEGNews { get; set; }
        public DbSet<BSIDNew> BSIDNews { get; set; }
        public DbSet<R_BKPFNew> R_BKPFNews { get; set; }
    }
}