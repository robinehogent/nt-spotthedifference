FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Spot_The_DifferenceBackend.sln ./
COPY Spot_The_Difference.API/Spot_The_Difference.API.csproj Spot_The_Difference.API/
COPY Spot_The_Difference.Contracts/Spot_The_Difference.Contracts.csproj Spot_The_Difference.Contracts/
COPY Spot_The_Difference.Domain.Model/Spot_The_Difference.Domain.Model.csproj Spot_The_Difference.Domain.Model/
COPY Spot_The_Difference.Domain.Services/Spot_The_Difference.Domain.Services.csproj Spot_The_Difference.Domain.Services/
COPY Spot_The_Difference.Domain.Services.Tests/Spot_The_Difference.Domain.Services.Tests.csproj Spot_The_Difference.Domain.Services.Tests/
COPY Spot_The_Difference.Persistence/Spot_The_Difference.Persistence.csproj Spot_The_Difference.Persistence/
COPY Spot_The_Difference.Persistence.Entities/Spot_The_Difference.Persistence.Entities.csproj Spot_The_Difference.Persistence.Entities/

RUN dotnet restore Spot_The_DifferenceBackend.sln

COPY . .

RUN ls -R /src/Spot_The_Difference.API/SeedData

RUN dotnet publish Spot_The_Difference.API/Spot_The_Difference.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Spot_The_Difference.API.dll"]

