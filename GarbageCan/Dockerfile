FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/netcoreapp5.0 GarbageCan/
WORKDIR /GarbageCan
CMD ASPNETCORE_URLS=http://*:$PORT dotnet GarbageCan.dll
# ENTRYPOINT ["dotnet", "GarbageCan.dll"]