FROM microsoft/dotnet
WORKDIR /app
COPY ..

EXPOSE 80 5000 
ENTRYPOINT ["dotnet", "RealtimeChatSample.Api.dll"]
