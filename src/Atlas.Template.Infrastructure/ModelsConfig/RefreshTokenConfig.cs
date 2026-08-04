using Atlas.Template.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Template.Infrastructure.ModelsConfig
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(x => x.CreatedOn)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
