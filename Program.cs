using Exomine_API_api_ydndua.Models.DTOs;
using Exomine_API_api_ydndua.Models;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

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

List<Transaction> transactions = new List<Transaction>
{
    new Transaction { Id = 1, GovernorId = 1, ColonyId = 1, FacilityId = 1, MineralId = 1, Quantity = 4, Timestamp = new DateTime(2100, 9, 19)},
    new Transaction { Id = 2, GovernorId = 2, ColonyId = 2, FacilityId = 2, MineralId = 3, Quantity = 2, Timestamp = new DateTime(2100, 9, 20)}

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

app.MapGet("/api/minerals", () =>
{
    return minerals.Select(m => new MineralDTO
    {
        Id = m.Id,
        Name = m.Name
    });
});

app.MapGet("/api/minerals/{id}", (int id) =>
{
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == id);
    if (mineral == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new MineralDTO
    {
        Id = mineral.Id,
        Name = mineral.Name
    });
});

app.MapPost("/api/minerals", (Mineral mineral) =>
{
    mineral.Id = minerals.Max(c => c.Id) + 1;
    minerals.Add(mineral);

    return Results.Created($"/api/minerals/{mineral.Id}", new MineralDTO
    {
        Id = mineral.Id,
        Name = mineral.Name
    }
    );
});

app.MapPut("/api/minerals/{id}", (int id, Mineral mineral) =>
{
    Mineral mineralToUpdate = minerals.FirstOrDefault(m => m.Id == id);

    if (mineralToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != mineral.Id)
    {
        return Results.BadRequest();
    }

    mineralToUpdate.Id = mineral.Id;
    mineralToUpdate.Name = mineral.Name;

    return Results.NoContent();

});

app.MapDelete("/api/minerals/{id}", (int id) =>
{
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == id);
    if (mineral == null)
    {
        return Results.NotFound();
    }

    minerals.RemoveAt(id);
    return Results.NoContent();
});

app.MapGet("api/colonyInventories", () =>
{
    return colonyInventories.Select(ci => new ColonyInventoryDTO
    {
        Id = ci.Id,
        ColonyId = ci.ColonyId,
        MineralId = ci.MineralId,
        MineralName = minerals.FirstOrDefault(m => ci.MineralId == m.Id).Name,
        Quantity = ci.Quantity
    });
});

app.MapGet("api/colonyInventories/{id}", (int id) =>
{
    ColonyInventory colonyInventory = colonyInventories.FirstOrDefault(ci => ci.Id == id);

    if (colonyInventory == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new ColonyInventoryDTO
    {
        Id = colonyInventory.Id,
        ColonyId = colonyInventory.ColonyId,
        MineralId = colonyInventory.MineralId,
        MineralName = minerals.FirstOrDefault(m => colonyInventory.MineralId == m.Id).Name,
        Quantity = colonyInventory.Quantity
    });
});

app.MapPost("/api/colonyInventories", (ColonyInventory colonyInventory) =>
{
    colonyInventory.Id = colonyInventories.Max(ci => ci.Id) + 1;
    colonyInventories.Add(colonyInventory);

    return Results.Created($"/api/colonyInventories/{colonyInventory.Id}", new ColonyInventoryDTO
    {
        Id = colonyInventory.Id,
        ColonyId = colonyInventory.ColonyId,
        MineralId = colonyInventory.MineralId,
        MineralName = minerals.FirstOrDefault(m => colonyInventory.MineralId == m.Id).Name,
        Quantity = colonyInventory.Quantity
    });

});

app.MapPut("/api/colonyInventories/{id}", (int id, ColonyInventory colonyInventory) =>
{
    ColonyInventory colonyInventoryToUpdate = colonyInventories.FirstOrDefault(ci => ci.Id == id);

    if (colonyInventoryToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != colonyInventory.Id)
    {
        return Results.BadRequest();
    }

    colonyInventoryToUpdate.Id = colonyInventory.Id;
    colonyInventoryToUpdate.ColonyId = colonyInventory.ColonyId;
    colonyInventoryToUpdate.MineralId = colonyInventory.MineralId;
    colonyInventoryToUpdate.Quantity = colonyInventory.Quantity;

    return Results.NoContent();


});

app.MapDelete("/api/colonyInventories/{id}", (int id) =>
{
    ColonyInventory colonyInventory = colonyInventories.FirstOrDefault(ci => ci.Id == id);
    if (colonyInventory == null)
    {
        return Results.NotFound();
    }

    colonyInventories.RemoveAt(id);
    return Results.NoContent();

});


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
    colonies.RemoveAt(id);
    return Results.NoContent();
});

app.MapGet("/api/facilityInventories", () =>
{
    return facilityInventories.Select(fi => new FacilityInventoryDTO
    {
        Id = fi.Id,
        FacilityId = fi.FacilityId,
        MineralId = fi.MineralId,
        MineralName = minerals.FirstOrDefault(m => fi.MineralId == m.Id).Name,
        Quantity = fi.Quantity
    });
});

app.MapGet("/api/facilityInventories/{id}", (int id) =>
{
    FacilityInventory facilityInventory = facilityInventories.FirstOrDefault(fi => fi.Id == id);

    if (facilityInventory == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new FacilityInventoryDTO
    {
        Id = facilityInventory.Id,
        FacilityId = facilityInventory.FacilityId,
        MineralId = facilityInventory.MineralId,
        MineralName = minerals.FirstOrDefault(m => facilityInventory.MineralId == m.Id).Name,
        Quantity = facilityInventory.Quantity
    });
});

app.MapPost("/api/facilityInventories", (FacilityInventory facilityInventory) =>
{
    facilityInventory.Id = facilityInventories.Max(fi => fi.Id) + 1;
    facilityInventories.Add(facilityInventory);

    return Results.Created($"/api/facilityInventories/{facilityInventory.Id}", new FacilityInventoryDTO
    {
        Id = facilityInventory.Id,
        FacilityId = facilityInventory.FacilityId,
        MineralId = facilityInventory.MineralId,
        MineralName = minerals.FirstOrDefault(m => facilityInventory.MineralId == m.Id).Name,
        Quantity = facilityInventory.Quantity
    });
});

app.MapPut("/api/facilityInventories/{id}", (int id, FacilityInventory facilityInventory) =>
{
    FacilityInventory facilityInventoryToUpdate = facilityInventories.FirstOrDefault(fi => fi.Id == id);

    if (facilityInventoryToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != facilityInventory.Id)
    {
        return Results.BadRequest();
    }

    facilityInventoryToUpdate.Id = facilityInventory.Id;
    facilityInventoryToUpdate.FacilityId = facilityInventory.FacilityId;
    facilityInventoryToUpdate.MineralId = facilityInventory.MineralId;
    facilityInventoryToUpdate.Quantity = facilityInventory.Quantity;

    return Results.NoContent();
});

app.MapDelete("/api/facilityInventories/{id}", (int id) =>
{
    FacilityInventory facilityInventory = facilityInventories.FirstOrDefault(fi => fi.Id == id);

    if (facilityInventory == null)
    {
        return Results.NotFound();
    }

    facilityInventories.RemoveAt(id);
    return Results.NoContent();
});

app.MapGet("/api/transactions", () =>
{
    return transactions.Select(t => new TransactionDTO
    {
        Id = t.Id,
        GovernorId = t.GovernorId,
        ColonyId = t.ColonyId,
        FacilityId = t.FacilityId,
        MineralId = t.MineralId,
        Quantity = t.Quantity,
        Timestamp = t.Timestamp
    });
});

app.MapGet("/api/transactions/{id}", (int id) =>
{
    Transaction transaction = transactions.FirstOrDefault(t => t.Id == id);

    if (transaction == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new TransactionDTO
    {
        Id = transaction.Id,
        GovernorId = transaction.GovernorId,
        ColonyId = transaction.ColonyId,
        FacilityId = transaction.FacilityId,
        MineralId = transaction.MineralId,
        Quantity = transaction.Quantity,
        Timestamp = transaction.Timestamp
    });
});


app.MapPut($"/api/transactions", (Transaction transaction) =>
{

    if (transaction.Id == 0)
    {
        transaction.Id = transactions.Max(t => t.Id) + 1;
    }

    MiningFacility facility = facilities.FirstOrDefault(f => f.Id == transaction.FacilityId);
    // if facility does not exist
    if (facility == null)
    {
        return Results.BadRequest();
    }
    // if facility is inactive
    if (!facility.Active)
    {
        return Results.BadRequest();
    }

    Governor governor = governors.FirstOrDefault(g => g.Id == transaction.GovernorId);
    // if governor does not exist
    if (governor == null)
    {
        return Results.BadRequest();
    }
    // if governor is inactive
    if (!governor.Active)
    {
        return Results.BadRequest();
    }

    transaction.ColonyId = governor.ColonyId;

    FacilityInventory facilityInventory = facilityInventories.FirstOrDefault(fi => fi.FacilityId == transaction.FacilityId && fi.MineralId == transaction.MineralId);
    //if matching facility does not exist
    if (facilityInventory == null)
    {
        return Results.BadRequest();
    }
    //if facility does not have enough
    if (facilityInventory.Quantity < 1)
    {
        return Results.BadRequest();
    }
    //If facility exists and has enough mineral, decrement by one
    if (facilityInventory != null)
    {
        facilityInventory.Quantity -= 1;
    }

    ColonyInventory colonyInventory = colonyInventories.FirstOrDefault(ci => ci.ColonyId == governor.ColonyId && ci.MineralId == transaction.MineralId);
    //if matching colonyInventory exists, increment by 1
    if (colonyInventory != null)
    {
        colonyInventory.Quantity += 1;
    }
    else
    {
        //create colony inventory object
        ColonyInventory newColonyInventory = new ColonyInventory
        {
            Id = 0,
            ColonyId = transaction.ColonyId,
            MineralId = transaction.MineralId,
            Quantity = transaction.Quantity
        };

        newColonyInventory.Id = colonyInventories.Max(ci => ci.Id) + 1;

        colonyInventories.Add(newColonyInventory);


    }

    Transaction transactionToUpdate = transactions.FirstOrDefault(t => t.Id == transaction.Id);
    if (transactionToUpdate == null)
    {
        //Add datetime to transaction
        transaction.Timestamp = DateTime.Now;
        // create transaction object and add it
        transactions.Add(transaction);

        //do I need a return for this?
        return Results.Created($"/api/transactions/{transaction.Id}", new TransactionDTO
        {
            Id = transaction.Id,
            GovernorId = transaction.GovernorId,
            ColonyId = transaction.ColonyId,
            FacilityId = transaction.FacilityId,
            MineralId = transaction.MineralId,
            Quantity = transaction.Quantity,
            Timestamp = transaction.Timestamp
        });

    }
    return Results.NoContent();

});



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
