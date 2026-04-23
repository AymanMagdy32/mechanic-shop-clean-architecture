using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Entities.Customers;
using MechanicShop.Domain.Entities.Customers.Vehicles;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Parts;
using MechanicShop.Domain.Entities.WorkOrders;
using MechanicShop.Domain.Entities.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
    public DbSet<Customer> Customers { get; }
    public DbSet<Part> Parts { get; }
    public DbSet<RepairTask> RepairTasks { get; }
    public DbSet<Vehicle> Vehicles { get; }
    public DbSet<WorkOrder> WorkOrders { get; }
    public DbSet<Employee> Employees { get; }
    public DbSet<Invoice> Invoices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}