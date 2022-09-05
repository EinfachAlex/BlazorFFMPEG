FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorFFMPEG.Backend/BlazorFFMPEG.Backend.csproj", "BlazorFFMPEG.Backend/"]
RUN dotnet restore "BlazorFFMPEG.Backend/BlazorFFMPEG.Backend.csproj"
COPY . .
WORKDIR "/src/BlazorFFMPEG.Backend"
RUN dotnet build "BlazorFFMPEG.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorFFMPEG.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorFFMPEG.Backend.dll"]
