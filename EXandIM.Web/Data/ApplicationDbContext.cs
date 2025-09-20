using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EXandIM.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ReferenceNumber> ReferenceNumbers { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<SideEntity> SideEntities { get; set; }
        public DbSet<SubEntity> SubEntities { get; set; }
        public DbSet<SecondSubEntity> SecondSubEntities { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<ReadingFile> ReadingFiles { get; set; }

        public DbSet<ActivityBook> Activities { get; set; }
        public DbSet<ItemInActivity> ItemsInActivity { get; set; }
        public DbSet<Circle> Circles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<MyFolder> MyFolders { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var cascadeFKs = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);



            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            

            base.OnModelCreating(builder);
        }
    }
}
