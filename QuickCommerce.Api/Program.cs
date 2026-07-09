using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuickCommerce.Api.Authorization;
using QuickCommerce.Api.BackgroundWorkers;
using QuickCommerce.Api.Hubs;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Repositories;
using QuickCommerce.Infrastructure.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using QuickCommerce.Api.Middleware;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// =======================
// CONTROLLERS
// =======================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHostedService<SettlementWorker>();

// =======================
// CORS
// =======================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// =======================
// JWT CONFIGURATION
// =======================

var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
});

// =======================
// AUTHORIZATION
// =======================

builder.Services.AddAuthorization(options =>
{
    // Dashboard
    options.AddPolicy("DASHBOARD.VIEW", p => p.Requirements.Add(new PermissionRequirement("DASHBOARD.VIEW")));

    // Store
    options.AddPolicy("STORE.CREATE", p => p.Requirements.Add(new PermissionRequirement("STORE.CREATE")));
    options.AddPolicy("STORE.UPDATE", p => p.Requirements.Add(new PermissionRequirement("STORE.UPDATE")));
    options.AddPolicy("STORE.DELETE", p => p.Requirements.Add(new PermissionRequirement("STORE.DELETE")));
    options.AddPolicy("STORE.VIEW", p => p.Requirements.Add(new PermissionRequirement("STORE.VIEW")));

    // Users
    options.AddPolicy("USER.CREATE", p => p.Requirements.Add(new PermissionRequirement("USER.CREATE")));
    options.AddPolicy("USER.UPDATE", p => p.Requirements.Add(new PermissionRequirement("USER.UPDATE")));
    options.AddPolicy("USER.DELETE", p => p.Requirements.Add(new PermissionRequirement("USER.DELETE")));
    options.AddPolicy("USER.VIEW", p => p.Requirements.Add(new PermissionRequirement("USER.VIEW")));

    // Roles
    options.AddPolicy("ROLE.CREATE", p => p.Requirements.Add(new PermissionRequirement("ROLE.CREATE")));
    options.AddPolicy("ROLE.UPDATE", p => p.Requirements.Add(new PermissionRequirement("ROLE.UPDATE")));
    options.AddPolicy("ROLE.DELETE", p => p.Requirements.Add(new PermissionRequirement("ROLE.DELETE")));
    options.AddPolicy("ROLE.VIEW", p => p.Requirements.Add(new PermissionRequirement("ROLE.VIEW")));

    // Products
    options.AddPolicy("PRODUCT.CREATE", p => p.Requirements.Add(new PermissionRequirement("PRODUCT.CREATE")));
    options.AddPolicy("PRODUCT.UPDATE", p => p.Requirements.Add(new PermissionRequirement("PRODUCT.UPDATE")));
    options.AddPolicy("PRODUCT.DELETE", p => p.Requirements.Add(new PermissionRequirement("PRODUCT.DELETE")));
    options.AddPolicy("PRODUCT.VIEW", p => p.Requirements.Add(new PermissionRequirement("PRODUCT.VIEW")));

    // Inventory
    options.AddPolicy("INVENTORY.VIEW", p => p.Requirements.Add(new PermissionRequirement("INVENTORY.VIEW")));
    options.AddPolicy("INVENTORY.UPDATE", p => p.Requirements.Add(new PermissionRequirement("INVENTORY.UPDATE")));

    // Orders
    options.AddPolicy("ORDER.CREATE", p => p.Requirements.Add(new PermissionRequirement("ORDER.CREATE")));
    options.AddPolicy("ORDER.VIEW", p => p.Requirements.Add(new PermissionRequirement("ORDER.VIEW")));
    options.AddPolicy("ORDER.UPDATE", p => p.Requirements.Add(new PermissionRequirement("ORDER.UPDATE")));
    options.AddPolicy("ORDER.CANCEL", p => p.Requirements.Add(new PermissionRequirement("ORDER.CANCEL")));

    // Delivery
    options.AddPolicy("DELIVERY.VIEW", p => p.Requirements.Add(new PermissionRequirement("DELIVERY.VIEW")));
    options.AddPolicy("DELIVERY.UPDATE", p => p.Requirements.Add(new PermissionRequirement("DELIVERY.UPDATE")));
    options.AddPolicy("DELIVERY.ASSIGN", p => p.Requirements.Add(new PermissionRequirement("DELIVERY.ASSIGN")));

    // Analytics
    options.AddPolicy("ANALYTICS.VIEW", p => p.Requirements.Add(new PermissionRequirement("ANALYTICS.VIEW")));

    // Finance
    options.AddPolicy("SETTLEMENT.VIEW", p => p.Requirements.Add(new PermissionRequirement("SETTLEMENT.VIEW")));
    options.AddPolicy("PLATFORM_FEE.UPDATE", p => p.Requirements.Add(new PermissionRequirement("PLATFORM_FEE.UPDATE")));
});
// ====================================
// CUSTOM PERMISSION HANDLER
// ====================================

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// =======================
// SWAGGER
// =======================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuickCommerce.Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =======================
// DATABASE
// =======================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// =======================
// REPOSITORIES
// =======================

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();

// =======================
// SERVICES
// =======================

builder.Services.AddHttpContextAccessor(); // ✅ ONLY ONCE

builder.Services.AddScoped<IStoreProductService, StoreProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<DeliveryAssignmentService>();
builder.Services.AddScoped<IOtpSender, ConsoleOtpSender>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStoreOnboardingService, StoreOnboardingService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IDemandForecastService, DemandForecastService>();

builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IStoreUserService, StoreUserService>();
builder.Services.AddScoped<IAdminRoleService, AdminRoleService>();
builder.Services.AddScoped<IAdminPermissionService, AdminPermissionService>();


// 🔥 AUDIT SYSTEM
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAdminAuditService, AdminAuditService>();

builder.Services.AddSignalR();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IGlobalSearchService, GlobalSearchService>();
builder.Services.AddScoped<ICustomerAddressService, CustomerAddressService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<IOrderManagementService, OrderManagementService>();
builder.Services.AddScoped<IDeliveryPartnerService, DeliveryPartnerService>();
builder.Services.AddScoped<IDeliveryAppService, DeliveryAppService>();
builder.Services.AddScoped<CustomerService>();

// =======================
// BUILD APP
// =======================

var app = builder.Build();

// =======================
// MIGRATION + SEEDER
// =======================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("🔄 Applying migrations...");
        await context.Database.MigrateAsync();

        Console.WriteLine("🌱 Running DbSeeder...");
        await DbSeeder.SeedAsync(context);

        Console.WriteLine("✅ Database seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error during migration/seeding:");
        Console.WriteLine(ex.Message);
    }
}

// =======================
// MIDDLEWARE
// =======================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseMiddleware<QuickCommerce.Api.Middleware.GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// 🔥 AUDIT (AFTER AUTH)
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();