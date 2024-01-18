using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestQuestionParser.Data;

public partial class SystemContext : DbContext
{
    
    public virtual DbSet<Question> Questions { get; set; }
    
    public SystemContext()
    {
    }

    public SystemContext(DbContextOptions<SystemContext> options)
        : base(options)
    {
        Options = options;
    }
    
    public readonly DbContextOptions<SystemContext> Options;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_czech_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("_question");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Answer1)
                .HasColumnType("text")
                .HasColumnName("answer_1");
            entity.Property(e => e.Answer2)
                .HasColumnType("text")
                .HasColumnName("answer_2");
            entity.Property(e => e.Answer3)
                .HasColumnType("text")
                .HasColumnName("answer_3");
            entity.Property(e => e.Answer4)
                .HasColumnType("text")
                .HasColumnName("answer_4");
            entity.Property(e => e.AtplA)
                .HasColumnType("tinyint(4)")
                .HasColumnName("atpl_a");
            entity.Property(e => e.AtplH)
                .HasColumnType("tinyint(4)")
                .HasColumnName("atpl_h");
            entity.Property(e => e.B11).HasColumnName("b11");
            entity.Property(e => e.B12).HasColumnName("b12");
            entity.Property(e => e.B2).HasColumnName("b2");
            entity.Property(e => e.Chapter)
                .HasMaxLength(45)
                .HasColumnName("chapter");
            entity.Property(e => e.CorrectAnswer)
                .HasColumnType("int(11)")
                .HasColumnName("correct_answer");
            entity.Property(e => e.CplA)
                .HasColumnType("tinyint(4)")
                .HasColumnName("cpl_a");
            entity.Property(e => e.CplH)
                .HasColumnType("tinyint(4)")
                .HasColumnName("cpl_h");
            entity.Property(e => e.Explanation)
                .HasColumnType("text")
                .HasColumnName("explanation");
            entity.Property(e => e.ImageLink)
                .HasMaxLength(255)
                .HasColumnName("image_link");
            entity.Property(e => e.ImageName)
                .HasMaxLength(45)
                .HasColumnName("image_name");
            entity.Property(e => e.InternalId)
                .HasMaxLength(10)
                .HasColumnName("internal_id");
            entity.Property(e => e.IrA)
                .HasColumnType("tinyint(4)")
                .HasColumnName("ir_a");
            entity.Property(e => e.IrH)
                .HasColumnType("tinyint(4)")
                .HasColumnName("ir_h");
            entity.Property(e => e.PplA)
                .HasColumnType("tinyint(4)")
                .HasColumnName("ppl_a");
            entity.Property(e => e.PplH)
                .HasColumnType("tinyint(4)")
                .HasColumnName("ppl_h");
            entity.Property(e => e.Question1)
                .HasColumnType("text")
                .HasColumnName("question");
            entity.Property(e => e.QuestionId)
                .HasColumnType("int(11)")
                .HasColumnName("question_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
