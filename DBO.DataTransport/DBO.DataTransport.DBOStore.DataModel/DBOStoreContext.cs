using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DBO.DataTransport.DBOStore.DataModel
{
    public partial class DBOStoreContext : DbContext
    {
        public DBOStoreContext()
        {
        }

        public DBOStoreContext(DbContextOptions<DBOStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionType> ActionTypes { get; set; }
        public virtual DbSet<DBOTransportHistory> DbotransportHistory { get; set; }
        public virtual DbSet<PostgreSQLConfiguration> PostgreSqlconfigurations { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectRDBMSRelationship> ProjectRdbmsrelationships { get; set; }
        public virtual DbSet<RDBMSConfiguration> Rdbmsconfigurations { get; set; }
        public virtual DbSet<SQLServerConfiguration> SqlserverConfigurations { get; set; }
        public virtual DbSet<SupportedRDBMS> SupportedRdbms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-279L8SC\\SQLEXPRESS;Database=DBOStore;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_ActionTypes_TypeId");

                entity.Property(e => e.TypeId).ValueGeneratedOnAdd();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DBOTransportHistory>(entity =>
            {
                entity.ToTable("DBOTransportHistory");

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.DbotransportHistory)
                    .HasForeignKey(d => d.ActionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DBOTransportHistory_ActionTypes");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.DbotransportHistory)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DBOTransportHistory_Projects");
            });

            modelBuilder.Entity<PostgreSQLConfiguration>(entity =>
            {
                entity.HasKey(e => e.ConnectId)
                    .HasName("PK_PostgreSQLConfigurations_ConnectId");

                entity.ToTable("PostgreSQLConfigurations");

                entity.Property(e => e.Database)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ProtectedPassword)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.PostgreSqlconfigurations)
                    .HasForeignKey(d => d.ConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostgreSQLConfigurations_RDBMSConfigurations");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.OwnerId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectRDBMSRelationship>(entity =>
            {
                entity.HasKey(e => e.RelationId)
                    .HasName("PK_ProjectRDBMSRelationships_RelationId");

                entity.ToTable("ProjectRDBMSRelationships");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RdbmsconsumerId).HasColumnName("RDBMSConsumerId");

                entity.Property(e => e.RdbmsownerId).HasColumnName("RDBMSOwnerId");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectRdbmsrelationships)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectRDBMSRelationships_Projects");

                entity.HasOne(d => d.Rdbmsconsumer)
                    .WithMany(p => p.ProjectRDBMSRelationshipRdbmsconsumer)
                    .HasForeignKey(d => d.RdbmsconsumerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectRDBMSRelationships_SupportedRDBMS_Consumer");

                entity.HasOne(d => d.Rdbmsowner)
                    .WithMany(p => p.ProjectRDBMSRelationshipRdbmsowner)
                    .HasForeignKey(d => d.RdbmsownerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectRDBMSRelationships_SupportedRDBMS_Owner");
            });

            modelBuilder.Entity<RDBMSConfiguration>(entity =>
            {
                entity.HasKey(e => e.ConfigId)
                    .HasName("PK_RDBMSConfigurations_ConfigId");

                entity.ToTable("RDBMSConfigurations");

                entity.Property(e => e.Rdmbsid).HasColumnName("RDMBSId");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Rdbmsconfigurations)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RDBMSConfigurations_Projects");
            });

            modelBuilder.Entity<SQLServerConfiguration>(entity =>
            {
                entity.HasKey(e => e.ConnectId)
                    .HasName("PK_SQLServerConfigurations_ConnectId");

                entity.ToTable("SQLServerConfigurations");

                entity.Property(e => e.Application)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DataSource)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProtectedPassword)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.SqlserverConfigurations)
                    .HasForeignKey(d => d.ConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QLServerConfigurations_RDBMSConfigurations");
            });

            modelBuilder.Entity<SupportedRDBMS>(entity =>
            {
                entity.HasKey(e => e.Rdbmsid)
                    .HasName("PK_SupportedRDBMS_RDBMSId");

                entity.ToTable("SupportedRDBMS");

                entity.Property(e => e.Rdbmsid)
                    .HasColumnName("RDBMSId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
