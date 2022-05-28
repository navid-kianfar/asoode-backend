FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Asoode.Backend.Background/Asoode.Backend.Background.csproj", "Asoode.Backend.Background/"]
RUN dotnet restore "Asoode.Backend.Background/Asoode.Backend.Background.csproj"
COPY . .
WORKDIR "/src/Asoode.Backend.Background"
RUN dotnet build "Asoode.Backend.Background.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asoode.Backend.Background.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asoode.Backend.Background.dll"]
