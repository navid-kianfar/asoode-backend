FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Asoode.Backend.Api/Asoode.Backend.Api.csproj", "Asoode.Backend.Api/"]
RUN dotnet restore "Asoode.Backend.Api/Asoode.Backend.Api.csproj"
COPY . .
WORKDIR "/src/Asoode.Backend.Api"
RUN dotnet build "Asoode.Backend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asoode.Backend.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asoode.Backend.Api.dll"]
