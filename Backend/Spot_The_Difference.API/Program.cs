using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;

using Spot_The_Difference.Domain.Model;
using Spot_The_Difference.Domain.Services;
using Spot_The_Difference.Persistence.Entities.MijnMap;

namespace Spot_The_Difference.API
{
    public class Program
    {
        readonly static string MigrationFile = "Spot_The_Difference.API";
        readonly static string ApplicationName = "Spot The Difference API";
        readonly static string ApplicationVersion = "v1";
        readonly static string ValidIssuerName = "SpotTheDifference";
        readonly static string ValidAudienceName = "SpotTheDifference";
        static void LoadServices(WebApplicationBuilder builder)
        {
            // Services
            builder.Services.AddScoped<GameService>();
            builder.Services.AddScoped<LevelService>();
            builder.Services.AddScoped<AdminService>();
        }

        static void SeedDatabase<T>(T db) where T : DbContext
        {
            SeedDataFromCsv<Difficulty>(db, "Difficulty", "1_Difficulty.csv");
            SeedDataFromCsv<Image>(db, "Image", "2_Image.csv");
            SeedDataFromCsv<Round>(db, "Round", "Round.csv");
            SeedDataFromCsv<Question>(db, "Question", "3_Question.csv");
            SeedDataFromCsv<Answeroption>(db, "AnswerOption", "4_AnswerOption.csv");
            SeedDataFromCsv<Difference>(db, "Difference", "5_Difference.csv");
        }

        public static void Main(string[] args)
        {
            // Load .env if present
            try { Env.TraversePath().Load(); } catch { /* ignore */ }

            var builder = WebApplication.CreateBuilder(args);

            var app = SetupFor<SpotthedifferencedbContext>(builder);

            app.Run();
        }

        static WebApplication SetupFor<T>(WebApplicationBuilder builder) where T : DbContext
        {
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? Array.Empty<string>();
                    var allowedMethods = builder.Configuration["Cors:AllowedMethods"]?.Split(',') ?? new[] { "GET", "POST", "PUT", "DELETE", "PATCH" };

                    if (allowedOrigins.Length > 0 && !string.IsNullOrEmpty(allowedOrigins[0]))
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .WithMethods(allowedMethods);
                    }
                    else
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApplicationVersion,
                new OpenApiInfo { Title = ApplicationName, Version = ApplicationVersion });
            });

            SetupDatabase<T>(builder);
            LoadServices(builder);

            var adminJwtSecret =
                Environment.GetEnvironmentVariable("ADMIN_JWT_SECRET")
                ?? builder.Configuration["AdminJwt:Secret"];

            if (adminJwtSecret == null)
            {
                throw new Exception("Admin JWT secret not configured");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = ValidIssuerName,
                        ValidAudience = ValidAudienceName,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(adminJwtSecret)),
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            var app = builder.Build();

            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            // Serve Swagger JSON and UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    $"{ApplicationName} API {ApplicationVersion}");
            });

            app.MapControllers();
            InitializeDatabase<T>(app);

            app.MapGet("/api/health", () => Results.Ok(new
            {
                ok = true,
                message = $"{ApplicationName} API up"
            }));

            return app;
        }

        static void SetupDatabase<T>(WebApplicationBuilder builder) where T : DbContext
        {
            // DbContext
            var connectionString =
                builder.Configuration["MYSQL_CONNECTION"] ??
                builder.Configuration.GetConnectionString("DefaultConnection") ??
                builder.Configuration.GetConnectionString("LocalMySQL");

            Console.WriteLine($"[CONFIG DEBUG] Connect to {connectionString}");

            if (connectionString == null)
            {
                throw new Exception("no connectionstring");
            }
            // Avoid a hard dependency on the DB being reachable during startup (e.g. in Docker Compose).
            // We'll connect with retries further down when ensuring the schema exists
            builder.Services.AddDbContext<T>(options =>
                {
                    options.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString),
                        mysql =>
                        {
                            // ADD THIS LINE to prevent Segfault (Exit 139)
                            mysql.MaxBatchSize(1);
                            mysql.MigrationsAssembly(MigrationFile);
                        }
                    );
                    var WithData = builder.Configuration["TRACE_SQL_WITH_DATA"];
                    if (WithData == "true")
                    {
                        options.LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableSensitiveDataLogging();
                    }
                }
            );
        }

        static void InitializeDatabase<T>(WebApplication app) where T : DbContext
        {
            // Ensure DB exists + apply migrations
            using (var scope = app.Services.CreateScope())
            {
                T db = scope.ServiceProvider.GetRequiredService<T>();

                const int maxAttempts = 30;
                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        // Use Migrate() instead of EnsureCreated() to apply all migrations
                        // This will create missing tables if the database already exists
                        db.Database.Migrate();
                        break;
                    }
                    catch (Exception ex) when (attempt < maxAttempts && (ex is MySqlException || ex is InvalidOperationException))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }

                SeedDatabase<T>(db);
            }
        }
        static void FindInconfig(WebApplicationBuilder builder, string key)
        {
            // in the .env and in the docker_compose.yml there is an environment section that derives from .
            var value = builder.Configuration[key];
            foreach (var provider in ((IConfigurationRoot)builder.Configuration).Providers.Reverse())
            {
                if (provider.TryGet(key, out var providerValue))
                {
                    Console.WriteLine($"[CONFIG DEBUG] Key '{key}' value '{value}' was provided by {provider}");
                    break;
                }
            }
        }

        static void FindEnvFile()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            Console.WriteLine($"Starting .env search from: {currentDir.FullName}");

            while (currentDir != null)
            {
                var potentialPath = Path.Combine(currentDir.FullName, ".env");
                if (File.Exists(potentialPath))
                {
                    Console.WriteLine($"✅ FOUND .env at: {potentialPath}");
                    break;
                }
                else
                {
                    Console.WriteLine($"❌ Not in: {currentDir.FullName}");
                }
                currentDir = currentDir.Parent;
            }
        }

        static void SeedDataFromCsv<T>(DbContext db, string EntityName, 
            string resourceName) where T : class
        {
            if (db.Set<T>().Any())
            {
                Console.WriteLine($"No need to seed {EntityName}");
                return;
            }

            var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", resourceName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: CSV file not found at {filePath}");
                return;
            }

            // Clear the cache so EF doesn't complain about "already tracking"
            db.ChangeTracker.Clear();

            Console.WriteLine($"Seeding {EntityName} from {filePath}");

            using var reader = new StreamReader(filePath);
            if( reader == null)
            {
                Console.WriteLine($"Error: Could not read file at {filePath}");
                return;
            }
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            using var csv = new CsvReader(reader, csvConfig);
            var records = csv.GetRecords<T>().ToList();

            var strategy = db.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entityType = db.Model.FindEntityType(typeof(T));
                    var tableName = entityType.GetTableName();
                    var primaryKey = entityType.FindPrimaryKey();
                    var properties = entityType.GetProperties().ToList();

                    var columnNames = string.Join(", ", properties.Select(p => $"`{p.GetColumnName()}`"));
                    var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                    // Include the PK explicitly in the SQL string
                    var sql = $"INSERT INTO `{tableName}` ({columnNames}) VALUES ({parameterNames})";

                    foreach (var record in records)
                    {
                        var parameters = new List<MySqlParameter>();

                        foreach (var prop in properties)
                        {
                            var val = typeof(T).GetProperty(prop.Name)?.GetValue(record) ?? DBNull.Value;
                            parameters.Add(new MySqlParameter($"@{prop.Name}", val));
                        }

                        db.Database.ExecuteSqlRaw(sql, parameters.ToArray());
                    }

                    transaction.Commit();
                    Console.WriteLine($"Successfully seeded {records.Count} records into {typeof(T).Name}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the inner exception to see the actual SQL error (like Foreign Key violations)
                    var inner = ex.InnerException?.Message ?? ex.Message;
                    throw new Exception($"Failed to seed {typeof(T).Name}: {inner}");
                }
            });
            // 4. Clear memory again for the next table in the loop
            db.ChangeTracker.Clear();
        }
    }
}