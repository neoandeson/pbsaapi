using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataService.Models
{
    public partial class PBSAContext : DbContext
    {
        public PBSAContext()
        {
        }

        public PBSAContext(DbContextOptions<PBSAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<BarberSchedules> BarberSchedules { get; set; }
        public virtual DbSet<BarberServices> BarberServices { get; set; }
        public virtual DbSet<Barbers> Barbers { get; set; }
        public virtual DbSet<BookingHistory> BookingHistory { get; set; }
        public virtual DbSet<BookingServices> BookingServices { get; set; }
        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<ComplaintImages> ComplaintImages { get; set; }
        public virtual DbSet<Complaints> Complaints { get; set; }
        public virtual DbSet<CustomerLocations> CustomerLocations { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Districts> Districts { get; set; }
        public virtual DbSet<PaymentMethods> PaymentMethods { get; set; }
        public virtual DbSet<Photos> Photos { get; set; }
        public virtual DbSet<RefBookingStates> RefBookingStates { get; set; }
        public virtual DbSet<RefComplaintStates> RefComplaintStates { get; set; }
        public virtual DbSet<RefComplaintTypes> RefComplaintTypes { get; set; }
        public virtual DbSet<RefCurrencyCodes> RefCurrencyCodes { get; set; }
        public virtual DbSet<RefPaymentTypes> RefPaymentTypes { get; set; }
        public virtual DbSet<RefPhotoTypes> RefPhotoTypes { get; set; }
        public virtual DbSet<RefTransactionStates> RefTransactionStates { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<Wallets> Wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:pbsa.database.windows.net,1433;Initial Catalog=PBSA;Persist Security Info=False;User ID=nambach2;Password=fu11PBSA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=Fal`se;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<BarberSchedules>(entity =>
            {
                entity.Property(e => e.BarberId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Repetition)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RepetitionType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Barber)
                    .WithMany(p => p.BarberSchedules)
                    .HasForeignKey(d => d.BarberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BarberSchedules_Barbers");
            });

            modelBuilder.Entity<BarberServices>(entity =>
            {
                entity.Property(e => e.BarberId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Barber)
                    .WithMany(p => p.BarberServices)
                    .HasForeignKey(d => d.BarberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BarberServices_Barbers");
            });

            modelBuilder.Entity<Barbers>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_Barber");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.CityCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContactPhone)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DistrictCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(256);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.CityCodeNavigation)
                    .WithMany(p => p.Barbers)
                    .HasForeignKey(d => d.CityCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Barbers_Cities");

                entity.HasOne(d => d.DistrictCodeNavigation)
                    .WithMany(p => p.Barbers)
                    .HasForeignKey(d => d.DistrictCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Barbers_Districts");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Barbers)
                    .HasForeignKey<Barbers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Barber_AspNetUsers");
            });

            modelBuilder.Entity<BookingHistory>(entity =>
            {
                entity.Property(e => e.BookedTime).HasColumnType("datetimeoffset(0)");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingHistory)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingStateHistory_Bookings");

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.BookingHistory)
                    .HasForeignKey(d => d.State)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingStateHistory_BookingStates");
            });

            modelBuilder.Entity<BookingServices>(entity =>
            {
                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingServices)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingServices_Bookings");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.BookingServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingServices_BarberServices");
            });

            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.Property(e => e.BarberId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.BookedTime).HasColumnType("datetimeoffset(0)");

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CustomerPaymentType).HasMaxLength(50);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Barber)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.BarberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_Barbers");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_Customers");

                entity.HasOne(d => d.CustomerPaymentTypeNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerPaymentType)
                    .HasConstraintName("FK_Bookings_RefPaymentTypes");

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.State)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_BookingStates");
            });

            modelBuilder.Entity<Cities>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ComplaintImages>(entity =>
            {
                entity.HasOne(d => d.Complaint)
                    .WithMany(p => p.ComplaintImages)
                    .HasForeignKey(d => d.ComplaintId)
                    .HasConstraintName("FK_ComplaintImages_Complaints");
            });

            modelBuilder.Entity<Complaints>(entity =>
            {
                entity.Property(e => e.StaffId).HasMaxLength(450);

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_Complaints_Bookings");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_Complaints_AspNetUsers");

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.State)
                    .HasConstraintName("FK_Complaints_ComplaintStates");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("FK_Complaints_ComplaintTypes");
            });

            modelBuilder.Entity<CustomerLocations>(entity =>
            {
                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.CustomerLocations)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerLocation_Booking");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerLocations)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerLocations_Customers");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_Customer");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.FirstName).HasMaxLength(256);

                entity.Property(e => e.LastName).HasMaxLength(256);

                entity.Property(e => e.MiddleName).HasMaxLength(256);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Customers)
                    .HasForeignKey<Customers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_AspNetUsers");
            });

            modelBuilder.Entity<Districts>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.CitiCode).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.CitiCodeNavigation)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.CitiCode)
                    .HasConstraintName("FK_Districts_Cities");
            });

            modelBuilder.Entity<PaymentMethods>(entity =>
            {
                entity.Property(e => e.PaymentType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.PaymentTypeNavigation)
                    .WithMany(p => p.PaymentMethods)
                    .HasForeignKey(d => d.PaymentType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMethods_RefPaymentTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PaymentMethods)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentMethods_AspNetUsers");
            });

            modelBuilder.Entity<Photos>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Url).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPhotos_UserPhotoTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPhotos_AspNetUsers1");
            });

            modelBuilder.Entity<RefBookingStates>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_BookingStates");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);
            });

            modelBuilder.Entity<RefComplaintStates>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_ComplaintStates");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);
            });

            modelBuilder.Entity<RefComplaintTypes>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_ComplaintTypes");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<RefCurrencyCodes>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<RefPaymentTypes>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<RefPhotoTypes>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_UserPhotoTypes");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);
            });

            modelBuilder.Entity<RefTransactionStates>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_RefTransactionTypes");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.CurrencyCode).HasMaxLength(50);

                entity.Property(e => e.ReceiverName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.SenderName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_Transaction_Booking");

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CurrencyCode)
                    .HasConstraintName("FK_Transactions_RefCurrencyCodes");

                entity.HasOne(d => d.ReceiverPaymentMethod)
                    .WithMany(p => p.TransactionsReceiverPaymentMethod)
                    .HasForeignKey(d => d.ReceiverPaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transactions_PaymentMethods1");

                entity.HasOne(d => d.SenderPaymentMethod)
                    .WithMany(p => p.TransactionsSenderPaymentMethod)
                    .HasForeignKey(d => d.SenderPaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transactions_PaymentMethods");

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.State)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transactions_RefTransactionTypes");
            });

            modelBuilder.Entity<Wallets>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Balance).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.CurrencyCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wallets_RefCurrencyCodes");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Wallets)
                    .HasForeignKey<Wallets>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wallets_PaymentMethods");
            });
        }
    }
}
