using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;

public class ReportAnswerConfigurations : IEntityTypeConfiguration<ReportAnswer>
{
    public void Configure(EntityTypeBuilder<ReportAnswer> builder)
    {
        builder.HasKey(x => x.ReportId);

        builder.HasOne(x => x.Report)
            .WithOne(x => x.ReportAnswer)
            .HasForeignKey("ReportAnswer", "ReportId");

    }
}