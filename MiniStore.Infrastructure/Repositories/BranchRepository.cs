using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class BranchRepository(AppDbContext context) : IBranchRepository { public Task<List<Branch>> GetAllAsync()=>context.Branches.OrderBy(x=>x.Code).ToListAsync(); public Task<Branch?> GetByIdAsync(int id)=>context.Branches.FirstOrDefaultAsync(x=>x.Id==id); public Task AddAsync(Branch branch)=>context.Branches.AddAsync(branch).AsTask(); public Task SaveChangesAsync()=>context.SaveChangesAsync(); }
