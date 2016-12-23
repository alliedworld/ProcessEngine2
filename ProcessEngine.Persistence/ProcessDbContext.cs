using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;

namespace KlaudWerk.ProcessEngine.Persistence
{
    public class AccountData
    {
        public Guid Id { get; set; }
        public string SourceSystem { get; set; }
        public string Name { get; set; }
        public int AccountType { get; set; }
    }

    public class ProcessDefinitionAccount
    {
        [Key, Column(Order = 0)]
        public Guid ProcessDefinitionId { get; set; }
        [Key, Column(Order = 1)]
        public int ProcessDefinitionVersion { get; set; }
        [Key, Column(Order = 2)]
        public Guid AccountDataId { get; set; }

        public virtual ProcessDefinitionPersistence ProcessDefinition { get; set; }
        public virtual AccountData Account { get; set; }
    }



    public class ProcessDbContext:DbContext
    {
        #region Constructors
        public ProcessDbContext()
        {
        }

        protected ProcessDbContext(DbCompiledModel model) : base(model)
        {
        }

        public ProcessDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public ProcessDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        {
        }

        public ProcessDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public ProcessDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        {
        }

        public ProcessDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {
        }
        #endregion

        public DbSet<ProcessDefinitionPersistence> ProcessDefinition { get; set; }
        public DbSet<ProcessDefinitionAccount> ProcessDefinitionAccounts { get; set; }
        public DbSet<AccountData> Accounts { get; set; }
        public DbSet<PersistentPropertyCollection> PropertySet { get; set; }
        public DbSet<PersistentSchemaElement> Schemas { get; set; }
        public DbSet<PersistentPropertyElement> Elements { get; set; }
        public DbSet<ProcessRuntimePersistence> Process { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountData>().HasKey(c => c.Id).ToTable("ACCOUNTS");
            modelBuilder.Entity<AccountData>().Property(c => c.Name).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<AccountData>().Property(c => c.SourceSystem).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<AccountData>().Property(c => c.AccountType).IsRequired();

            modelBuilder.Entity<ProcessDefinitionAccount>().ToTable("PROCESS_DEFINITION_ACCOUNT");

            modelBuilder.Entity<PersistentPropertyCollection>().HasKey(c => c.Id).ToTable("PROPERTIES_COLLECTIONS");
            modelBuilder.Entity<PersistentPropertyElement>().HasKey(c => c.Id).ToTable("PROPERTY_ELEMENTS");
            modelBuilder.Entity<PersistentSchemaElement>().HasKey(c => c.Id)
                .ToTable("PROPERTY_SCHEMA_ELEMENTS")
                .Property(c=>c.SchemaBody).IsRequired();
            modelBuilder.Entity<PersistentSchemaElement>().Property(c => c.SchemaName).IsRequired();
            modelBuilder.Entity<PersistentSchemaElement>().Property(c => c.SchemaType).IsRequired();
            modelBuilder.Entity<PersistentPropertyElement>().Property(c => c.Name).IsRequired();

            modelBuilder.Entity<ProcessDefinitionPersistence>().ToTable("PROCESS_DEFINITIONS");
            modelBuilder.Entity<ProcessDefinitionPersistence>().HasKey(c => new {c.Id, c.Version});
            modelBuilder.Entity<ProcessDefinitionPersistence>().Property(c => c.Version).IsRequired();
            modelBuilder.Entity<ProcessDefinitionPersistence>().Property(c => c.Name).IsRequired();
            modelBuilder.Entity<ProcessDefinitionPersistence>().Property(c => c.Md5)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_MD5", 1) { IsUnique = true }));
            modelBuilder.Entity<ProcessDefinitionPersistence>().Property(c => c.Status).IsRequired();
            modelBuilder.Entity<ProcessDefinitionPersistence>().Property(c => c.JsonProcessDefinition).IsRequired();

            modelBuilder.Entity<ProcessRuntimePersistence>().HasKey(c => c.Id).ToTable("PROCESSES");
            base.OnModelCreating(modelBuilder);
        }
    }
}