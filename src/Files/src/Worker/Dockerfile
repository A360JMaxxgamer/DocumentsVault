FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Files/src/Files.Worker/Files.Worker.csproj", "Files.Worker/"]
RUN dotnet restore "src/Files/src/Files.Worker/Files.Worker.csproj"
COPY . .
WORKDIR "/src/Files.Worker"
RUN dotnet build "Files.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Files.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Files.Worker.dll"]
