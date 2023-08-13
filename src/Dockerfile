FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish TooGoodToGoNotifier.Presentation.Console/TooGoodToGoNotifier.Presentation.Console.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "TooGoodToGoNotifier.Presentation.Console.dll"]