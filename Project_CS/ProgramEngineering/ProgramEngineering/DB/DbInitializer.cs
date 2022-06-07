using ProgramEngineering.DB.Models;
using System;
using System.Linq;

namespace ProgramEngineering.DB
{
    public static class DbInitializer
    {
        public static void Initialize(ApiDbContext ctx)
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Authors.Any())
            {
                ctx.Authors.Add(new Author
                {
                    Name = "Винсент В.Г."
                });
                ctx.Authors.Add(new Author
                {
                    Name = "Айвазовский И.К."
                });
            }

            if (!ctx.Pictures.Any())
            {
                ctx.Pictures.Add(new Picture
                {
                    Title = "Звездная ночь",
                    AuthorId = 1,
                    PublicationDate = new DateTime(1889, 11, 1),
                });
                ctx.Pictures.Add(new Picture
                {
                    Title = "Девятый вал",
                    AuthorId = 2,
                    PublicationDate = new DateTime(1850, 6, 8)
                });
                ctx.SaveChanges();
            }
        }
    }
}
