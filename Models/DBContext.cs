﻿using Microsoft.EntityFrameworkCore;

namespace EOE_WebAPI.Models
{
    public class EOEDbContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameScore> GameScore { get; set; }
        public DbSet<Media> Medias { get; set; } // Thêm Media

        public EOEDbContext(DbContextOptions<EOEDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Khóa chính cho Account
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);

            // Khóa chính cho User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            // Khóa chính cho Game
            modelBuilder.Entity<Game>()
                .HasKey(g => g.GameId);

            // Khóa chính cho GameScore
            modelBuilder.Entity<GameScore>()
                .HasKey(gs => gs.ScoreId);

            // Khóa chính cho Media
            modelBuilder.Entity<Media>()
                .HasKey(m => m.MediaId);

            // Định nghĩa mối quan hệ giữa User và Media (ProfileImage)
            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfileImage)
                .WithMany(m => m.Users)
                .HasForeignKey(u => u.ProfileImageId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade khi xóa Media

            // Định nghĩa mối quan hệ một-nhiều giữa Account và User
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne(u => u.Account)
                .HasForeignKey<User>(u => u.AccountId);

            // Định nghĩa mối quan hệ một-nhiều giữa User và GameScore
            modelBuilder.Entity<User>()
                .HasMany(u => u.GameScores)
                .WithOne(gs => gs.User)
                .HasForeignKey(gs => gs.UserId);

            // Định nghĩa mối quan hệ một-nhiều giữa Game và GameScore
            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameScores)
                .WithOne(gs => gs.Game)
                .HasForeignKey(gs => gs.GameId);

            // Định nghĩa mối quan hệ giữa Game và Media (GameImageId)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.GameImage)
                .WithMany(m => m.Games)
                .HasForeignKey(u => u.GameImageId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade khi xóa Media
        }
    }
}
