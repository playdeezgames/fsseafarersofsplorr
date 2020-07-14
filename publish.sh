#!/bin/bash

dotnet publish -c Release --runtime win-x64 --framework netcoreapp3.1 -o ./deploy/win ./Splorr.Seafarers/Splorr.Seafarers.fsproj
dotnet publish -c Release --runtime osx-x64 --framework netcoreapp3.1 --self-contained true -o ./deploy/mac ./Splorr.Seafarers/Splorr.Seafarers.fsproj
dotnet publish -c Release --runtime linux-x64 --framework netcoreapp3.1 --self-contained true -o ./deploy/linux ./Splorr.Seafarers/Splorr.Seafarers.fsproj
dotnet publish -c Release --runtime linux-arm64 --framework netcoreapp3.1 --self-contained true -o ./deploy/pi ./Splorr.Seafarers/Splorr.Seafarers.fsproj
