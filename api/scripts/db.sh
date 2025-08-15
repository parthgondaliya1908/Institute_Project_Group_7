rm -rf ./api/src/Migrations/*
dotnet build api/src &&
dotnet ef database drop --project api/src --force --no-build && 
dotnet ef migrations add Initial --project api/src --no-build &&
dotnet build api/src &&
dotnet ef database update --project api/src --no-build
