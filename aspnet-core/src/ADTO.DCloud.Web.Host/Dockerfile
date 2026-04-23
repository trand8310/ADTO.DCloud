FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["src/ADTO.DCloud.Web.Host/ADTO.DCloud.Web.Host.csproj", "src/ADTO.DCloud.Web.Host/"]
COPY ["src/ADTO.DCloud.Web.Core/ADTO.DCloud.Web.Core.csproj", "src/ADTO.DCloud.Web.Core/"]
COPY ["src/ADTO.DCloud.Application/ADTO.DCloud.Application.csproj", "src/ADTO.DCloud.Application/"]
COPY ["src/ADTO.DCloud.Core/ADTO.DCloud.Core.csproj", "src/ADTO.DCloud.Core/"]
COPY ["src/ADTO.DCloud.EntityFrameworkCore/ADTO.DCloud.EntityFrameworkCore.csproj", "src/ADTO.DCloud.EntityFrameworkCore/"]
WORKDIR "/src/src/ADTO.DCloud.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/ADTO.DCloud.Web.Host", "src/ADTO.DCloud.Web.Host"]
COPY ["src/ADTO.DCloud.Web.Core", "src/ADTO.DCloud.Web.Core"]
COPY ["src/ADTO.DCloud.Application", "src/ADTO.DCloud.Application"]
COPY ["src/ADTO.DCloud.Core", "src/ADTO.DCloud.Core"]
COPY ["src/ADTO.DCloud.EntityFrameworkCore", "src/ADTO.DCloud.EntityFrameworkCore"]
WORKDIR "/src/src/ADTO.DCloud.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "ADTO.DCloud.Web.Host.dll"]
