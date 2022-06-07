FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY *.sln .
COPY src/SimOpsService/SimOpsService.csproj ./src/SimOpsService/
COPY tests/SimOpsService.Tests/SimOpsService.Tests.csproj ./tests/SimOpsService.Tests/
RUN dotnet restore

# copy everything else
COPY src/SimOpsService/. ./src/SimOpsService/
COPY tests/SimOpsService.Tests/. ./tests/SimOpsService.Tests/
WORKDIR /source/src/SimOpsService
RUN dotnet build  -c Release -o /app/build

FROM build AS test
WORKDIR /source/tests/SimOpsService.Tests/
RUN dotnet test

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimOpsService.dll"]
