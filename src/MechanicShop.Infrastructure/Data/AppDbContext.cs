
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Entities.Customers;
using MechanicShop.Domain.Entities.Customers.Vehicles;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Parts;
using MechanicShop.Domain.Entities.WorkOrders;
using MechanicShop.Domain.Entities.WorkOrders.Billing;
using MechanicShop.Domain.Identity;
using MechanicShop.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options), IAppDbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<RepairTask> RepairTasks => Set<RepairTask>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
 

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

   
}