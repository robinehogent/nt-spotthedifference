using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class SpotthedifferencedbContext : DbContext
{
    public SpotthedifferencedbContext()
    {
    }

    public SpotthedifferencedbContext(DbContextOptions<SpotthedifferencedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answeroption> Answeroptions { get; set; }

    public virtual DbSet<Difference> Differences { get; set; }

    public virtual DbSet<Differenceoption> Differenceoptions { get; set; }

    public virtual DbSet<Difficulty> Difficulties { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Playerround> Playerrounds { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Answeroption>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PRIMARY");

            entity.ToTable("answeroption");

            entity.HasIndex(e => e.QuestionId, "question_id");

            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");
            entity.Property(e => e.TextNl)
                .HasMaxLength(255)
                .HasColumnName("text_nl"); 
            entity.Property(e => e.TextFr)
                .HasMaxLength(255)
                .HasColumnName("text_fr"); 

            entity.HasOne(d => d.Question).WithMany(p => p.Answeroptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("answeroption_ibfk_1");
        });

        modelBuilder.Entity<Difference>(entity =>
        {
            entity.HasKey(e => e.DifferenceId).HasName("PRIMARY");

            entity.ToTable("difference");

            entity.HasIndex(e => e.RoundId, "round_id");

            entity.Property(e => e.DifferenceId).HasColumnName("difference_id");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Width).HasColumnName("width");
            entity.Property(e => e.X).HasColumnName("x");
            entity.Property(e => e.Y).HasColumnName("y");

            entity.HasOne(d => d.Round).WithMany(p => p.Differences)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("difference_ibfk_1");
        });

        modelBuilder.Entity<Differenceoption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PRIMARY");

            entity.ToTable("differenceoption");

            entity.HasIndex(e => e.RoundId, "round_id");

            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");

            entity.HasOne(d => d.Round).WithMany(p => p.Differenceoptions)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("differenceoption_ibfk_1");
        });

        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.HasKey(e => e.DifficultyId).HasName("PRIMARY");

            entity.ToTable("difficulty");

            entity.Property(e => e.DifficultyId).HasColumnName("difficulty_id");
            entity.Property(e => e.DifferenceCount).HasColumnName("difference_count");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.QuestionCount).HasColumnName("question_count");
            entity.Property(e => e.TimeLimitSeconds).HasColumnName("time_limit_seconds");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.ToTable("image");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.Type)
                .HasColumnType("enum('original','difference')")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PRIMARY");

            entity.ToTable("player");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Playerround>(entity =>
        {
            entity.HasKey(e => e.PlayerRoundId).HasName("PRIMARY");

            entity.ToTable("playerround");

            entity.HasIndex(e => e.PlayerId, "player_id");

            entity.HasIndex(e => e.RoundId, "round_id");

            entity.Property(e => e.PlayerRoundId).HasColumnName("player_round_id");
            entity.Property(e => e.Completed)
                .HasDefaultValueSql("'0'")
                .HasColumnName("completed");
            entity.Property(e => e.CorrectDifferences)
                .HasDefaultValueSql("'0'")
                .HasColumnName("correct_differences");
            entity.Property(e => e.CorrectQuestions)
                .HasDefaultValueSql("'0'")
                .HasColumnName("correct_questions");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Score)
                .HasDefaultValueSql("'0'")
                .HasColumnName("score");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.TimeSpent).HasColumnName("time_spent");

            entity.HasOne(d => d.Player).WithMany(p => p.Playerrounds)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playerround_ibfk_1");

            entity.HasOne(d => d.Round).WithMany(p => p.Playerrounds)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playerround_ibfk_2");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PRIMARY");

            entity.ToTable("question");

            entity.HasIndex(e => e.RoundId, "round_id");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");
            entity.Property(e => e.TextNl)
                .HasMaxLength(255)
                .HasColumnName("text_nl");
            entity.Property(e => e.TextFr)
                .HasMaxLength(255)
                .HasColumnName("text_fr");

            entity.HasOne(d => d.Round).WithMany(p => p.Questions)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("question_ibfk_1");
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.RoundId).HasName("PRIMARY");

            entity.ToTable("round");

            entity.HasIndex(e => e.DifferenceImageId, "difference_image_id");

            entity.HasIndex(e => e.DifficultyId, "difficulty_id");

            entity.HasIndex(e => e.OriginalImageId, "original_image_id");

            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DifferenceImageId).HasColumnName("difference_image_id");
            entity.Property(e => e.DifficultyId).HasColumnName("difficulty_id");
            entity.Property(e => e.OriginalImageId).HasColumnName("original_image_id");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.DifferenceImage).WithMany(p => p.RoundDifferenceImages)
                .HasForeignKey(d => d.DifferenceImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("round_ibfk_3");

            entity.HasOne(d => d.Difficulty).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.DifficultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("round_ibfk_1");

            entity.HasOne(d => d.OriginalImage).WithMany(p => p.RoundOriginalImages)
                .HasForeignKey(d => d.OriginalImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("round_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected static void ErrorAndExit(string msg1, string msg2)
    {
        Console.WriteLine($@"
*********
* ERROR *
*********
{msg1}

{msg2}

Druk een toets om de backend af te sluiten...
");
        Console.ReadKey(true);
        Environment.Exit(1);
    }

}
