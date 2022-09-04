FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorFFMPEG/BlazorFFMPEG.csproj", "BlazorFFMPEG/"]
RUN dotnet restore "BlazorFFMPEG/BlazorFFMPEG.csproj"
COPY . .
WORKDIR "/src/BlazorFFMPEG"
RUN dotnet build "BlazorFFMPEG.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorFFMPEG.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorFFMPEG.dll"]
