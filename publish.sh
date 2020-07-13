#!/bin/bash

dotnet publish -c Release -o ./deploy/win ./Splorr.Seafarers/Splorr.Seafarers.fsproj
dotnet publish -c Release -r osx-x64 -o ./deploy/mac ./Splorr.Seafarers/Splorr.Seafarers.fsproj
