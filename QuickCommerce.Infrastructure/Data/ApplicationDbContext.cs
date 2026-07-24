using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace QuickCommerce.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // CORE TABLES
        // =========================

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<CategoryCommissionRule> CategoryCommissionRules { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryPartner> DeliveryPartners { get; set; }
        public DbSet<DeliveryPartnerBeat> DeliveryPartnerBeats { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DbSet<AdminSession> AdminSessions { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<StoreSettlement> StoreSettlements { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<CustomerActivity> CustomerActivities { get; set; }
        public DbSet<CustomerSegment> CustomerSegments { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<OnboardingRequest> OnboardingRequests { get; set; }

        

        // =========================
        // STORE TABLES
        // =========================

        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }
        public DbSet<UserStore> UserStores { get; set; }


        // =========================
        // RBAC TABLES
        // =========================

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // TABLE NAMES
            // =========================

            modelBuilder.Entity<OtpVerification>()
                .ToTable("otp_verifications");

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Permission>().ToTable("permission"); 
            modelBuilder.Entity<RolePermission>().ToTable("role_permission");

            modelBuilder.Entity<InventoryMovement>()
                .ToTable("inventory_movements");
            
            // =========================
            // STORE CONFIG
            // =========================

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasIndex(s => s.Code).IsUnique();

                entity.Property(s => s.Name).IsRequired();
                entity.Property(s => s.Code).IsRequired();
                entity.Property(s => s.City).IsRequired();
                entity.Property(s => s.Area).IsRequired();
                entity.Property(s => s.State).IsRequired();
                entity.Property(s => s.Pincode).IsRequired();
            });

            // =========================
            // STORE PRODUCT CONFIG (ENTERPRISE)
            // =========================

            modelBuilder.Entity<StoreProduct>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.StoreId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.Store)
                      .WithMany(s => s.StoreProducts)
                      .HasForeignKey(e => e.StoreId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // USER-STORE MAPPING
            // =========================

            modelBuilder.Entity<UserStore>()
                .HasKey(us => new { us.UserId, us.StoreId });

            modelBuilder.Entity<UserStore>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserStores)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserStore>()
                .HasOne(us => us.Store)
                .WithMany(s => s.UserStores)
                .HasForeignKey(us => us.StoreId);

            // =========================
            // RBAC CONFIGURATION
            // =========================

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // BUSINESS RELATIONSHIPS
            // =========================

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Store)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // DELIVERY RELATIONS (FIXED)
            // =========================

            // Internal Agent (User)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.InternalAgent)
                .WithMany()
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // External Agent (Delivery Partner)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.ExternalAgent)
                .WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.AssignedToPartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            modelBuilder.Entity<Delivery>().HasIndex(d => d.AssignedToUserId);
            modelBuilder.Entity<Delivery>().HasIndex(d => d.AssignedToPartnerId);

            modelBuilder.Entity<DeliveryPartnerBeat>()
                .HasOne(b => b.DeliveryPartner)
                .WithMany(p => p.Beats)
                .HasForeignKey(b => b.AssignedToPartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.Product)
                .WithMany()
                .HasForeignKey(im => im.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.Store)
                .WithMany()
                .HasForeignKey(im => im.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // REFRESH TOKEN CONFIG (CRITICAL)
            // =========================

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.HasIndex(rt => rt.Token).IsUnique();

                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);

                entity.Property(rt => rt.DeviceInfo).HasMaxLength(200);

                entity.Property(rt => rt.IpAddress).HasMaxLength(50);
            });

            // =========================
            // PASSWORD RESET TOKEN
            // =========================

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Token)
                      .IsUnique();

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Token)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.Property(x => x.IpAddress)
                      .HasMaxLength(50);

                entity.Property(x => x.UserAgent)
                      .HasMaxLength(300);

                entity.Property(x => x.CreatedBy)
                      .HasMaxLength(100);

                entity.Property(x => x.Purpose)
                      .HasMaxLength(50);
            });
            // =========================
            // ADMIN SESSION
            // =========================

            modelBuilder.Entity<AdminSession>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.RefreshToken)
                      .IsUnique();

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.RefreshToken)
                      .HasMaxLength(500)
                      .IsRequired();
                
                entity.Property(x => x.DeviceInfo)
                      .HasMaxLength(200);

                entity.Property(x => x.Browser)
                      .HasMaxLength(200);

                entity.Property(x => x.OperatingSystem)
                      .HasMaxLength(200);

                entity.Property(x => x.IpAddress)
                      .HasMaxLength(50);

                entity.Property(x => x.RevokeReason)
                      .HasMaxLength(300);
            });
            // =========================
            // INDEXES
            // =========================

            modelBuilder.Entity<Order>().HasIndex(o => o.CustomerId);
            modelBuilder.Entity<Order>().HasIndex(o => o.Status);
            modelBuilder.Entity<Delivery>().HasIndex(d => d.OrderId);
            modelBuilder.Entity<NotificationRecipient>()
    .HasOne(nr => nr.Notification)
    .WithMany(n => n.Recipients)
    .HasForeignKey(nr => nr.NotificationId)
    .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges()
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimesToUtc();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimesToUtc()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                foreach (var property in entry.Properties)
                {
                    if (property.CurrentValue is DateTime dt)
                    {
                        property.CurrentValue =
                            DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
            }
        }
    }
}