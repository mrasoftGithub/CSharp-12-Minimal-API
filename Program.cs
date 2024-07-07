global using Microsoft.EntityFrameworkCore;

// Alias Array Types
using DeLijst = System.Collections.Generic.List<EIGENAAR>;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Definieer je CORS
// zie launchSettings.json - profiel WebAPI - http://localhost:5168/
builder.Services.AddCors
    (
      options => options.AddPolicy(name: "CORSBeleid",
      policy => { policy.WithOrigins("http://localhost:5168").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); }
    )
);

builder.Services.AddDbContext<DbContextClass>(options => options.UseSqlServer("name=ConnectionStrings:VOORBEELDConnection"));

// Interface IModel
// Activeer de implementatie die van toepassing is. 

// --- Without Keyed Services ---
// de MemoryModel-implementatie:
// builder.Services.AddSingleton<IModel, MemoryModel>();

// de DBModel-implementatie:
// builder.Services.AddScoped<IModel, DBModel>();

// With Keyed Services --
builder.Services.AddKeyedSingleton<IModel, MemoryModel>("memory");
builder.Services.AddKeyedScoped<IModel, DBModel>("DB");

var app = builder.Build();

// Activeer CORS
app.UseCors("CORSBeleid");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/HaalOpEigenaren", async ([FromKeyedServices("memory")] IModel model) =>
{
    try
    {
        return Results.Ok(await model.HaalOpEigenaren());
    }
    catch (Exception ex)
    {
        return Results.Problem("Er is iets fout gegaan met minimal API HaalOpEigenaren - " + ex.Message);
    }
});

app.MapGet("HaalOpEigenaar/{ID}", async ([FromKeyedServices("memory")] IModel model, int ID = 5) =>
{
    try
    {
        return Results.Ok(await model.HaalopEigenaar(ID) is EIGENAAR eigenaar ? Results.Ok(eigenaar) : Results.NotFound("Niks gevonden"));
    }
    catch (Exception ex)
    {
        return Results.Problem("Er is iets fout gegaan met minimal API HaalOpEigenaar - " + ex.Message);
    }
});

app.MapPost("/VoegToe", async ([FromKeyedServices("memory")] IModel model, EIGENAAR eigenaar) =>
{
    try
    {
        // DBCC CHECKIDENT (EIGENAAR, RESEED, 26)
        eigenaar.ID = null;
        await model.VoegToe(eigenaar);
        return Results.Ok(await model.HaalopEigenaar(eigenaar.ID));
    }
    catch (Exception ex)
    {
        return Results.Problem("Er is iets fout gegaan met minimal API VoegToe - " + ex.Message);
    }
});

app.MapPut("/Muteer", async ([FromKeyedServices("memory")] IModel model, EIGENAAR eigenaar) =>
{
    try
    {
        return Results.Ok(await model.Muteer(eigenaar));
    }
    catch (Exception ex)
    {
        return Results.Problem("Er is iets fout gegaan met minimal API Muteer - " + ex.Message);
    }
});

app.MapDelete("Verwijder/{ID}", async ([FromKeyedServices("memory")] IModel model, int ID) =>
{
    try
    {
        return Results.Ok(await model.Verwijder(ID));
    }
    catch (Exception ex)
    {
        return Results.Problem("Er is iets fout gegaan met minimal API Verwijder - " + ex.Message);
    }
});

app.Run();
public class EIGENAAR
{
    public int? ID { get; set; } = 0;

    public string? Omschrijving { get; set; } = string.Empty;

    public string? Voornaam { get; set; } = string.Empty;

    public string? Achternaam { get; set; } = string.Empty;

    public string? Regio { get; set; } = string.Empty;
}

public interface IModel
{
    Task<List<EIGENAAR>> HaalOpEigenaren();

    Task<EIGENAAR?> HaalopEigenaar(int? ID);

    Task<EIGENAAR?> VoegToe(EIGENAAR? eigenaar);

    Task<EIGENAAR?> Muteer(EIGENAAR? eigenaar);

    Task<bool> Verwijder(int ID);
}

public class MemoryModel : IModel
{
    // Zonder Alias:
    // private readonly List<EIGENAAR> _Eigenaren = new List<EIGENAAR>
    private readonly DeLijst _Eigenaren =
    [
        new EIGENAAR() { ID = 1,Omschrijving = "Sandra's auto", Regio = "Noord", Voornaam = "Sandra",Achternaam = "Janssen"},
        new EIGENAAR() { ID =2, Omschrijving="Peter's auto", Regio="Midden", Voornaam="Peter", Achternaam="Veerman"},
        new EIGENAAR() { ID =3, Omschrijving="Olga's auto", Regio="Zuid", Voornaam="Olga", Achternaam="Mulder"},
        new EIGENAAR() { ID =4, Omschrijving="Piet's auto", Regio="Noord", Voornaam="Piet", Achternaam="Pietersen"},
        new EIGENAAR() { ID =5, Omschrijving="Klaas' auto", Regio="Midden", Voornaam="Klaas", Achternaam="Vaak"},
        new EIGENAAR() { ID =6, Omschrijving="Jan's auto", Regio="Zuid", Voornaam="Jan", Achternaam="Janssen"},
        new EIGENAAR() { ID =7, Omschrijving="Petra's auto", Regio="Noord", Voornaam="Petra", Achternaam="Petersen"},
        new EIGENAAR() { ID =8, Omschrijving="Nicole's auto", Regio="Midden", Voornaam="Nicole", Achternaam="Nicholsen"},
        new EIGENAAR() { ID =9, Omschrijving="Olga's auto", Regio="Zuid", Voornaam="Olga", Achternaam="Olafsson"},
        new EIGENAAR() { ID =10, Omschrijving="Helga's auto", Regio="Noord", Voornaam="Helga", Achternaam="Helgoland"},
        new EIGENAAR() { ID =11, Omschrijving="Maaike's auto", Regio="Midden", Voornaam="Maaike", Achternaam="Bourtange"},
        new EIGENAAR() { ID =12, Omschrijving="Eric's auto", Regio="Zuid", Voornaam="Eric", Achternaam="Ericsson"},
        new EIGENAAR() { ID =13, Omschrijving="Jürgen's auto", Regio="Noord", Voornaam="Jürgen", Achternaam="Peelhoven"},
        new EIGENAAR() { ID =14, Omschrijving="Patrick's auto", Regio="Midden", Voornaam="Patrick", Achternaam="Oss"},
        new EIGENAAR() { ID =15, Omschrijving="Kristina's auto", Regio="Zuid", Voornaam="Kristina", Achternaam="Woudrich"},
        new EIGENAAR() { ID =16, Omschrijving="Dirk's auto ", Regio="Noord", Voornaam="Dirk", Achternaam="Broekema"},
        new EIGENAAR() { ID =17, Omschrijving="Fred's auto", Regio="Midden", Voornaam="Fred", Achternaam="Seedorf"},
        new EIGENAAR() { ID =18, Omschrijving="Richard's auto", Regio="Zuid", Voornaam="Richard", Achternaam="Lith"},
        new EIGENAAR() { ID =19, Omschrijving="Herman's auto", Regio="Noord", Voornaam="Herman", Achternaam="Zwijger"},
        new EIGENAAR() { ID =20, Omschrijving="Lara's auto", Regio="Midden", Voornaam="Lara", Achternaam="Tielsen"},
        new EIGENAAR() { ID =21, Omschrijving="Bernhard's auto", Regio="Zuid", Voornaam="Bernhard", Achternaam="Amshof"},
        new EIGENAAR() { ID =22, Omschrijving="Marisca's auto", Regio="Noord", Voornaam="Marisca", Achternaam="Schiermonnik"},
        new EIGENAAR() { ID =23, Omschrijving="Tamara's auto", Regio="Midden", Voornaam="Tamara", Achternaam="Laerhoven"},
        new EIGENAAR() { ID =24, Omschrijving="Ria's auto", Regio="Zuid", Voornaam="Ria", Achternaam="Berg"}
    ];

    // Zonder Alias:
    // public async Task<List<EIGENAAR>> HaalOpEigenaren()
    public async Task<DeLijst> HaalOpEigenaren()
    {
        return await Task.Run(() => _Eigenaren);
    }

    public async Task<EIGENAAR?> HaalopEigenaar(int? ID)
    {
        return ID.HasValue ? await Task.Run(() => _Eigenaren.FirstOrDefault(x => x.ID == ID)) : new EIGENAAR();
    }

    public async Task<EIGENAAR?> VoegToe(EIGENAAR? eigenaar)
    {
        return await Task.Run(() =>
        {
            if (eigenaar == null) return new EIGENAAR();

            // Bepaal de ID
            if (_Eigenaren.Count == 0) eigenaar.ID = 1; else eigenaar.ID = _Eigenaren.Max(e => e.ID) + 1;

            // Toevoegen
            _Eigenaren.Add(eigenaar);

            // Retourneer
            return eigenaar;
        });
    }

    public async Task<EIGENAAR?> Muteer(EIGENAAR? eigenaar)
    {
        return await Task.Run(() =>
        {
            int index = 0;

            // Niks gevonden, retourneer leeg object
            if (eigenaar == null) return new EIGENAAR();

            // Origineel object ophalen
            EIGENAAR? eigenaarOrg = _Eigenaren.FirstOrDefault(x => x.ID == eigenaar.ID);

            // Werk de List bij 
            if (eigenaarOrg != null) index = _Eigenaren.IndexOf(eigenaarOrg);
            _Eigenaren[index] = eigenaar;

            // Retourneer
            return eigenaar;
        });
    }

    public async Task<bool> Verwijder(int ID)
    {
        return await Task.Run(() =>
        {
            // throw new NotImplementedException();

            // Haal de originele gegevens op
            var eigenaarOrg = _Eigenaren.FirstOrDefault(x => x.ID == ID);

            // Niks gevonden, retourneer leeg object
            if (eigenaarOrg == null) return false;

            // Verwijder 
            _Eigenaren.Remove(eigenaarOrg);
            return true;
        });
    }
}

public class DbContextClass : DbContext
{
    public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }

    public DbSet<EIGENAAR> EIGENAAR => Set<EIGENAAR>();
}

public class DBModel : IModel
{
    private readonly DbContextClass _dbContextClass;

    public DBModel(DbContextClass dbContextClass)
    {
        _dbContextClass = dbContextClass;
    }

    public async Task<List<EIGENAAR>> HaalOpEigenaren()
    {
        return await _dbContextClass.EIGENAAR.ToListAsync();
    }

    public async Task<EIGENAAR?> HaalopEigenaar(int? ID)
    {
        return ID.HasValue ? await Task.Run(() => _dbContextClass.EIGENAAR.FirstOrDefaultAsync(x => x.ID == ID)) : new EIGENAAR();
    }

    public async Task<EIGENAAR?> VoegToe(EIGENAAR? eigenaar)
    {
        if (eigenaar == null) return new EIGENAAR();

        EIGENAAR eigenaar_ = eigenaar;

        // Toevoegen
        _dbContextClass.EIGENAAR.Add(eigenaar_);
        await _dbContextClass.SaveChangesAsync();
        eigenaar = eigenaar_;

        // Retourneer
        return eigenaar;
    }

    public async Task<EIGENAAR?> Muteer(EIGENAAR? eigenaar)
    {
        if (eigenaar == null) return new EIGENAAR();
        EIGENAAR? eigenaarOrg = await _dbContextClass.EIGENAAR.FindAsync(eigenaar.ID);
        if (eigenaarOrg == null) return new EIGENAAR();

        eigenaarOrg.Omschrijving = eigenaar.Omschrijving;
        eigenaarOrg.Voornaam = eigenaar.Voornaam;
        eigenaarOrg.Achternaam = eigenaar.Achternaam;
        eigenaarOrg.Regio = eigenaar.Regio;

        await _dbContextClass.SaveChangesAsync();
        return eigenaarOrg;
    }

    public async Task<bool> Verwijder(int ID)
    {
        // throw new NotImplementedException();
        EIGENAAR? eigenaar = await _dbContextClass.EIGENAAR.FindAsync(ID);
        if (eigenaar == null) return false;

        _dbContextClass.EIGENAAR.Remove(eigenaar);
        await _dbContextClass.SaveChangesAsync();
        return true;
    }
}