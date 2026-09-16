using Exomine_API_api_ydndua.Models.DTOs;
using Exomine_API_api_ydndua.Models;

List<Colony> colonies = new List<Colony>
{
    new Colony { Id = 1, Name = "Earth" },
    new Colony { Id = 2, Name = "Mars" },
    new Colony { Id = 3, Name = "Europa" }
};

List<Governor> governors = new List<Governor>
{
    new Governor { Id = 1, Name = "Patricia Purdy", Active = true, ColonyId = 1 },
    new Governor { Id = 2, Name = "Katrina Bahringer", Active = true, ColonyId = 2},
    new Governor { Id = 3, Name = "Lola Wolf", Active = true, ColonyId = 3 },
    new Governor { Id = 4, Name = "Damon Hartmann", Active = true, ColonyId = 3 },
    new Governor { Id = 5, Name = "Eleanor Voss", Active = false, ColonyId = 2 }

};

List<MiningFacility> facilities = new List<MiningFacility>
{
    new MiningFacility { Id = 1, Name = "Ganymede", Active = true },
    new MiningFacility { Id = 2, Name = "Io", Active = true },
    new MiningFacility { Id = 3, Name = "Titan", Active = true },
    new MiningFacility { Id = 4, Name = "Luna", Active = false },
};

List<Mineral> minerals = new List<Mineral>
{
    new Mineral { Id = 1, Name = "Iron" },
    new Mineral { Id = 2, Name = "Chromium" },
    new Mineral { Id = 3, Name = "Molybdenum" },
    new Mineral { Id = 4, Name = "Salt" },
    new Mineral { Id = 5, Name = "Nickel" }
};

List<FacilityInventory> facilityInventories = new List<FacilityInventory>
{
    new FacilityInventory { Id = 1, FacilityId = 1, MineralId = 1, Quantity = 18 },
    new FacilityInventory { Id = 2, FacilityId = 1, MineralId = 2, Quantity = 13 },
    new FacilityInventory { Id = 3, FacilityId = 2, MineralId = 3, Quantity = 10 },
    new FacilityInventory { Id = 4, FacilityId = 2, MineralId = 4, Quantity = 28 }
};

List<ColonyInventory> colonyInventories = new List<ColonyInventory>
{
    new ColonyInventory { Id = 1, ColonyId = 1, MineralId = 1, Quantity = 4 },
    new ColonyInventory { Id = 2, ColonyId = 1, MineralId = 2, Quantity = 5 },
    new ColonyInventory { Id = 3, ColonyId = 2, MineralId = 3, Quantity = 4 },
    new ColonyInventory { Id = 4, ColonyId = 3, MineralId = 1, Quantity = 8 },
    new ColonyInventory { Id = 5, ColonyId = 3, MineralId = 2, Quantity = 1 },
    new ColonyInventory { Id = 6, ColonyId = 1, MineralId = 4, Quantity = 1 },
    new ColonyInventory { Id = 7, ColonyId = 2, MineralId = 4, Quantity = 0 }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/colonies", () =>
{
    return colonies.Select(c => new ColonyDTO
    {
        Id = c.Id,
        Name = c.Name,
        Location = c.Location,
        Governors = governors.Where(g => g.ColonyId == c.Id).Select(g => new GovernorDTO
        {
            Id = g.Id,
            Name = g.Name,
            Active = g.Active,
            ColonyId = g.ColonyId
        }).ToList(),
        ColonyInventories = colonyInventories.Where(ci => ci.ColonyId == c.Id).Select(ci => new ColonyInventoryDTO
        {
            Id = ci.Id,
            ColonyId = ci.ColonyId,
            MineralId = ci.MineralId,
            Quantity = ci.Quantity
        }).ToList()
    });
});

app.MapGet("/api/colonies/{id}", (int id) =>
{
    Colony colony = colonies.FirstOrDefault(c => c.Id == id);
    if (colony == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new ColonyDTO
    {
        Id = colony.Id,
        Name = colony.Name,
        Location = colony.Location,
        Governors = governors.Where(g => g.ColonyId == colony.Id).Select(g => new GovernorDTO
        {
            Id = g.Id,
            Name = g.Name,
            Active = g.Active,
            ColonyId = g.ColonyId
        }).ToList(),
        ColonyInventories = colonyInventories.Where(ci => ci.ColonyId == id).Select(ci => new ColonyInventoryDTO
        {
            Id = ci.Id,
            ColonyId = ci.ColonyId,
            MineralId = ci.MineralId,
            Quantity = ci.Quantity
        }).ToList()
    });

});

app.MapPost("/api/colonies", (Colony colony) =>
{

    colony.Id = colonies.Max(c => c.Id) + 1;
    colonies.Add(colony);

    return Results.Created($"/api/colonies/{colony.Id}", new ColonyDTO
    {
        Id = colony.Id,
        Name = colony.Name,
        Location = colony.Location,
        Governors = governors.Where(g => g.ColonyId == colony.Id).Select(g => new GovernorDTO
        {
            Id = g.Id,
            Name = g.Name,
            Active = g.Active,
            ColonyId = g.ColonyId
        }).ToList()
    });
});

app.MapPut("/api/colonies/{id}", (int id, Colony colony) =>
{
    Colony colonyToUpdate = colonies.FirstOrDefault(c => c.Id == id);

    if (colonyToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != colony.Id)
    {
        return Results.BadRequest();
    }

    colonyToUpdate.Id = colony.Id;
    colonyToUpdate.Name = colony.Name;
    colonyToUpdate.Location = colony.Location;

    return Results.NoContent();

});


app.MapDelete("/api/colonies/{id}", (int id) =>
{
    Colony colony = colonies.FirstOrDefault(c => c.Id == id);
    if (colony == null)
    {
        return Results.NotFound();
    }
    colonies.RemoveAt(id - 1);
    return Results.NoContent();
});



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
