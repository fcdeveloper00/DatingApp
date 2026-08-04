using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<AppUser> Users => Set<AppUser>();
        
    public DbSet<UserLike> Likes => Set<UserLike>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLike>()
            .HasKey(
                userLike => new
                { 
                    userLike.SourceUserId, 
                    userLike.TargetUserId 
                }
            );

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.LikedUsers)
            .WithOne(ul => ul.SourceUser)
            //.WithMany(ul => ul.TargetU)
            .HasForeignKey(ul => ul.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //** 
        //modelBuilder.Entity<UserLike>()
        //    .HasOne(ul => ul.SourceUser)
        //    .WithMany(u => u.LikedUsers)
        //    .HasForeignKey(ul => ul.SourceUserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.LikedByUsers)
            .WithOne(ul => ul.TargetUser)
            .HasForeignKey(ul => ul.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //**
        //modelBuilder.Entity<UserLike>()
        //    .HasOne(ul => ul.TargetUser)
        //    .WithMany(u => u.LikedByUsers) // Why not a list of 'AppUser'?
        //    .HasForeignKey(ul => ul.TargetUserId)
        //    .OnDelete(DeleteBehavior.Cascade); // NOTE: If you're using SQL Server, then you should use 'CascadeBehavoir.NoAction' instead.


        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.MessagesReceived)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);



    }


}
