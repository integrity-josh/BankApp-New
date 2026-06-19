using BankApp.Data;
using BankApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// we have three lifecycles to choose from for mapping DI here
    // singleton - one instance for everyone forever - inject that container one time only, reuse that for every request until the server shuts off
        // not common - we have different users, usually don't want to reuse the same instance for every user, but could be useful for something like a logger, where we want to reuse the same instance for every user
        // for when you really only need one, and everyone can just use it/share it
        // doesnt work well for things that are going to be changed after they are created, because syncing changes just doesn't work well with it
    // scoped - one instance per request, for the whole request
        // throughout lifespan of the request, something that has scoped will instantiate the object, then reuse that same instance for every time it's called during that request, but then once the request is done, that instance is done, and a new one will be created for the next request
        // good for repositories, or units of work
            // everything done in that unit of work all needs to succeed or fail together, so you want to use the same instance for everything in that unit of work, but then once that unit of work is done, you want to get rid of that instance and start fresh for the next unit of work
        // this is part of UNIT OF WORK pattern
            // tracks changes but commits at the end
            // do all you want, but nothing actually sends those changes back to the DB until you complete the transaction
            // EF implements this pattern, or you could create a custom unit of work object that implements it
                // ex: we hypothetically instead of using save changes in the code, we had in the pipeline that when a request was over to call EF save changes
    // transient - one instance every time you ask for it
        // every place it's injected gets a new instance of the class
        // transient objects live the shortest generally, meaning they are least available to be shared, and are most disposable, takes up the least resources
        // can't reuse it for anything
        // middleground between this and singleton which can always be used by everyone forever and never goes away, we can scope
            // common use cases:
                // we scope repositories, 
                // and most else is transient
                // GENERAL PRINCIPLE - MAKE IT TRANSIENT UNLESS YOU CAN'T
                    // because it uses the least resources
                    // if you can't, have a good reason for it
                        // one good reason, is if you use a lot of transient objects which could have resused the same resources instead, you then use up way more resources by recreating those dependencies for every transient object


    // when deciding, think of the implication that resources are precious as we are working in systems with 1000s of active users, so small increases in usage are exponential
    
builder.Services.AddControllers();


builder.Services.AddMediatR(
    // license key optional for paid version
    cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
    ); // this is per the MediatR documentation on github, and it will register all the handlers in the assembly that contains the Program class, which is this assembly, so it will register all the handlers in this project, which is what we want


builder.Services.AddDbContext<BankAppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // add the db context to the DI container, so that it can be injected into the repositories, and then we can use those repositories in our handlers, and then we can use those handlers in our controllers, and then we can use those controllers to handle requests from the client
// When your application starts up and reads Program.cs, it builds the dependency injection container and instantiates your BankAppDbContext. 
    // The very first time EF Core builds the internal database model, it runs OnModelCreating.
    // When it hits ApplyConfigurationsFromAssembly(typeof(BankAppDbContext).Assembly) in the DbContext, EF Core asks the .NET runtime:
    // "Look inside the compiled .dll file (Assembly) where BankAppDbContext lives. 
    // Find every single class that implements the IEntityTypeConfiguration<T> interface."
        // this is how it finds and applies the CustomerMapping, AccountMapping classes, and any other mapping classes we create in the future, without us having to explicitly tell it about them


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    //typeof to inject an abstraction, and then the implementation, and then we can use that abstraction in our code, and it will inject the implementation for us
    // this allows us to do things like create mock data, test versions and implement them in here

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
