#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SuperSurvey.WebApp/SuperSurvey.WebApp.csproj", "SuperSurvey.WebApp/"]
RUN dotnet restore "SuperSurvey.WebApp/SuperSurvey.WebApp.csproj"
COPY . .
WORKDIR "/src/SuperSurvey.WebApp"
RUN dotnet build "SuperSurvey.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SuperSurvey.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SuperSurvey.WebApp.dll"]

ENV ConnectionStrings__PollDb="Server=localhost;Database=super_survey;Uid=super_survey_user;Pwd=123456;"
ENV ConnectionStrings__VoteQueue="http://localhost:4566/000000000000/votes"