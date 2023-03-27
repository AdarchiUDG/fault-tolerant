using System.Text.Json.Serialization;
using DockerExample;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<WikiContext>(WikiContextOptionsAction);
builder.Services.AddRouting(c => {
    c.LowercaseUrls = true;
    c.LowercaseQueryStrings = true;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    using var context = scope.ServiceProvider.GetRequiredService<WikiContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseHttpsRedirection();
app.UseFileServer(enableDirectoryBrowsing: false);
MapEndpoints(app);
app.Run($"http://*:{Environment.GetEnvironmentVariable("PORT")}");

void MapEndpoints(IEndpointRouteBuilder router) {
    router.MapGet("/entries", (WikiContext context) => context.Entries.ToListAsync());
    router.MapGet("/entries/{id:int}", async Task<Results<Ok<Entry>, NotFound>>(int id, WikiContext context) => {
        var entry = await context.Entries.FirstOrDefaultAsync(e => e.Id == id);

        return entry is not null ? TypedResults.Ok(entry) : TypedResults.NotFound();
    });
    router.MapPost("/entries", async (EntryPayload payload, WikiContext context) => {
        var entity = await context.Entries.AddAsync(new Entry() {
            Title = payload.Title,
            Content = payload.Contents
        });

        await context.SaveChangesAsync();

        return TypedResults.Created($"/entries/{entity.Entity.Id}", entity.Entity);

    });
    router.MapDelete("/entries/{id:int}", async Task<Results<Ok<Entry>, NotFound>> (int id, WikiContext context) => {
        Entry entry;
        try {
            var entity = context.Remove(new Entry {
                Id = id
            });
            entry = entity.Entity;
            await context.SaveChangesAsync();
        } catch (Exception e) {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(entry);
    });
}

void WikiContextOptionsAction(DbContextOptionsBuilder optionsBuilder) {
    if (Environment.GetEnvironmentVariable("CONN_STRING_POSTGRES") is not null) {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONN_STRING_POSTGRES"));
    }
}
