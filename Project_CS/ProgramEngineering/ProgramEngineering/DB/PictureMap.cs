using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgramEngineering.DB.Models;

namespace ProgramEngineering.DB
{
    public class PictureMap
    {
        public PictureMap(EntityTypeBuilder<Picture> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.ToTable("pictures");

            entityBuilder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entityBuilder.Property(x => x.Title).HasColumnName("title");
            entityBuilder.Property(x => x.AuthorId).HasColumnName("authorId");
            entityBuilder.Property(x => x.PublicationDate).HasColumnName("publication_date");

            entityBuilder.HasOne(x => x.Author).WithMany(x => x.Pictures).HasForeignKey(x => x.AuthorId);
        }
    }
}