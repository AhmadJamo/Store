using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class CustomerRepository(AppDbContext context) : ICustomerRepository { public Task<List<Customer>> GetAllAsync()=>context.Customers.OrderBy(x=>x.Name).ToListAsync(); public Task<Customer?> GetByIdAsync(int id)=>context.Customers.FirstOrDefaultAsync(x=>x.Id==id); public Task AddAsync(Customer customer)=>context.Customers.AddAsync(customer).AsTask(); public Task SaveChangesAsync()=>context.SaveChangesAsync(); }
