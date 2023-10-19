#Part 1
FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS base

# Build app from sdk image
WORKDIR /src
COPY . .
RUN dotnet publish "NetCa.sln" -c Release -o /app /p:VersionPrefix=#{major-minor}#.#{patch}# /p:VersionSuffix=#{suffix-version}#

#Part 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy AS final

# Add library
RUN apt-get update && apt-get install -y curl

# Change timezone to local time
ENV TZ 'Asia/Jakarta'
RUN echo $TZ > /etc/timezone && \
apt-get update && apt-get install -y tzdata && \
rm /etc/localtime && \
ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
dpkg-reconfigure -f noninteractive tzdata && \
apt-get clean

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_TieredPGO=1
ENV DOTNET_ReadyToRun=0
ENV DOTNET_TC_QuickJitForLoops=1
ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Copy to runtime image
WORKDIR /app
COPY --from=base /app .

# Change to non-root privilege
RUN useradd -u 1234 default
USER default

EXPOSE 8080

ENTRYPOINT ["dotnet", "NetCa.Api.dll"]