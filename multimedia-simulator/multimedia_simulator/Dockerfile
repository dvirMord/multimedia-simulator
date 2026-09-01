FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 8554
ENV ASPNETCORE_HTTP_PORTS=5000
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["multimedia-simulator.csproj", "./"]
RUN dotnet restore "./multimedia-simulator.csproj"
COPY . .
RUN dotnet build "multimedia-simulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "multimedia-simulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y ffmpeg wget tar && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /opt/mediamtx \
    && wget https://github.com/bluenviron/mediamtx/releases/download/v1.9.0/mediamtx_v1.9.0_linux_amd64.tar.gz \
    && tar -xvzf mediamtx_v1.9.0_linux_amd64.tar.gz -C /opt/mediamtx \
    && rm mediamtx_v1.9.0_linux_amd64.tar.gz \
    && chmod +x /opt/mediamtx/mediamtx

ENV FFMPEG_PATH=/usr/bin/ffmpeg
ENV FFmpeg__Path=/usr/bin/ffmpeg
ENV FFmpeg__ExecutablePath=/usr/bin/ffmpeg
ENV MEDIAMTX_PATH=/opt/mediamtx/mediamtx

COPY --from=publish /app/publish .
COPY .env.docker .

ENTRYPOINT ["dotnet", "multimedia-simulator.dll"]