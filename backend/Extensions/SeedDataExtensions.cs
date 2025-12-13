using backend.Abstractions;
using backend.Database;
using backend.Models;
using Bogus;

namespace backend.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var faker = new Faker();

        var users = new List<User>();
        
        for (int i = 0; i < 20; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
            });
        }

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var mainComments = new List<Comment>();

        for (int i = 0; i < 10; i++)
        {
            var user = faker.PickRandom(users);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Text = faker.Lorem.Sentence(8),
                UserId = user.Id,
                CreatedAt = faker.Date.Recent(1000).ToUniversalTime(),
            };

            if (faker.Random.Bool())
            {
                comment.Attachment = new Attachment
                {
                    Id = Guid.NewGuid(),
                    CommentId = comment.Id,
                    FileUrl = faker.Image.PicsumUrl(),
                    FileName = faker.System.FileName(),
                    FileType = AttachmentType.Image
                };
            }

            var children = new List<Comment>();

            int replies = faker.Random.Int(2, 5);

            for (int j = 0; j < replies; j++)
            {
                var childUser = faker.PickRandom(users);

                var child = new Comment
                {
                    Id = Guid.NewGuid(),
                    ParentId = comment.Id,
                    Text = faker.Lorem.Sentence(6),
                    UserId = childUser.Id,
                    CreatedAt = faker.Date.Recent(3).ToUniversalTime()
                };

                if (faker.Random.Bool(0.3f))
                {
                    child.Attachment = new Attachment
                    {
                        Id = Guid.NewGuid(),
                        CommentId = child.Id,
                        FileUrl = faker.Image.PicsumUrl(),
                        FileName = faker.System.FileName(),
                        FileType = AttachmentType.Image
                    };
                }

                children.Add(child);
            }

            comment.Children = children;
            mainComments.Add(comment);
        }

        dbContext.Comments.AddRange(mainComments);
        dbContext.SaveChanges();
    }
}
