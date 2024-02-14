

using Microsoft.EntityFrameworkCore;
using NerdCritica.Domain.Entities;
using System;

namespace NerdCritica.Infrastructure.Context;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
}
