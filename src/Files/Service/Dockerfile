FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Files/Service/Files.Service.csproj", "Service/"]
COPY ["Files/Grpc/Files.Grpc.csproj", "Grpc/"]
COPY ["Shared/AspNetCore.Utilities/AspNetCore.Utilities.csproj", "AspNetCore.Utilities/"]
RUN dotnet restore "Files/Service/Files.Service.csproj"
COPY . .
WORKDIR "/src/Service"
RUN dotnet build "Files.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Files.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Files.Service.dll"]
