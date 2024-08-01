# Tsk

### Requirements

Install the following dependencies on Your machine:
1. .Net 8.0
2. Docker

### How to run

Restore packages and cli-tools:
```bash
dotnet restore
dotnet tool restore
```

Run all dependencies:
```bash
docker-compose up --detach
```

Migrate the database:
```bash
dotnet ef database update --project ./Tsk.HttpApi
```
