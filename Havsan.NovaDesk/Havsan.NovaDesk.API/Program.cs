using Havsan.NovaDesk.API.Data;
using Havsan.NovaDesk.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Cors;

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", 
                                             "https://localhost:3000") 
                                 .AllowAnyHeader() 
                                 .AllowAnyMethod(); 
                      });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<UygulamaVeriBaglami>()
.AddDefaultTokenProviders();


var baglantiDizesi = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UygulamaVeriBaglami>(secenekler =>
    secenekler.UseSqlServer(baglantiDizesi));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
    };
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Havsan NovaDesk API", Version = "v1" }); 

    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "API Baðlantýsý Baþarýlý!");

app.MapControllers();




using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Rollarý kontrol et ve oluþtur
    if (!await roleManager.RoleExistsAsync(KullaniciRolleri.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(KullaniciRolleri.Admin));
    }
    if (!await roleManager.RoleExistsAsync(KullaniciRolleri.DestekPersoneli))
    {
        await roleManager.CreateAsync(new IdentityRole(KullaniciRolleri.DestekPersoneli));
    }
    if (!await roleManager.RoleExistsAsync(KullaniciRolleri.SonKullanici))
    {
        await roleManager.CreateAsync(new IdentityRole(KullaniciRolleri.SonKullanici));
    }

    
    var adminKullanici = await userManager.FindByNameAsync("admin");
    if (adminKullanici == null)
    {
        adminKullanici = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@novadesk.com",
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(adminKullanici, "AdminPass123!"); 

        
        await userManager.AddToRoleAsync(adminKullanici, KullaniciRolleri.Admin);
    }
}


app.Run();