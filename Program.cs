

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
    new Governor { Id = 2, Name = "Eleanor Voss", Active = false, ColonyId = 2 }

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

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
