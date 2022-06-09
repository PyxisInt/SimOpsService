FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY *.sln .
COPY src/SimOps.Models/SimOps.Models.csproj ./src/SimOps.Models/
COPY src/SimOpsService/SimOpsService.csproj ./src/SimOpsService/
COPY src/SimOps.Sdk/SimOps.Sdk.csproj ./src/SimOps.Sdk/
COPY src/SimOpsService.Interfaces/SimOpsService.Interfaces.csproj ./src/SimOpsService.Interfaces/
COPY tests/SimOpsService.Tests/SimOpsService.Tests.csproj ./tests/SimOpsService.Tests/
RUN dotnet restore

# copy everything else
COPY src/SimOps.Models/. ./src/SimOps.Models/
COPY src/SimOpsService/. ./src/SimOpsService/
COPY src/SimOps.Sdk/. ./src/SimOps.Sdk/
COPY src/SimOpsService.Interfaces/. ./SimOpsService.Interfaces/
COPY tests/SimOpsService.Tests/. ./tests/SimOpsService.Tests/
WORKDIR /src/SimOpsService
RUN dotnet build  -c Release -o /app/build

FROM build AS test
WORKDIR /tests/SimOpsService.Tests/
RUN dotnet test

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimOpsService.dll"]
