

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NerdCritica.Domain.Entities;
using System;

namespace NerdCritica.Infrastructure.Context;

public class ApplicationDbContext: IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    //public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Aqui você pode adicionar configurações personalizadas do Identity
        // Exemplo: builder.Entity<IdentityUser>().ToTable("Usuarios");
    }
}
