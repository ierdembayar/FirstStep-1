# --------
#  STEP 1
# --------

# Install .NET Core SDK docker container
FROM microsoft/dotnet:sdk AS build-env

# Create app directory
WORKDIR /app

# Install app dependencies
COPY . .

WORKDIR /app/Sample.MvcWebApi

RUN dotnet restore
RUN dotnet publish -c Release -o out

# --------
#  STEP 2
# --------

# Install .NET Core Runtime docker container
FROM microsoft/dotnet:runtime

# Environment variables
ENV ASPNETCORE_ENVIRONMENT staging

# Create app directory
WORKDIR /app

# Copy build output
COPY --from=build-env /app/Sample.MvcWebApi/out ./

ENTRYPOINT ["dotnet", "OneFrame.Sample.MvcWebApi.dll"]
