FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimOpsService/SimOpsService.csproj", "SimOpsService/"]
RUN dotnet restore "src/SimOpsService/SimOpsService.csproj"
COPY . .
WORKDIR "/src/SimOpsService"
RUN dotnet build "SimOpsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimOpsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimOpsService.dll"]
