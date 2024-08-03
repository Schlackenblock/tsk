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

### Local development

To minimize setup overhead and maximize usability and productiveness, I would recommend:

Runing the database from the docker-compose configuration using the following command:
```bash
docker-compose up --detach postgres
```

Launching the `Tsk.HttpApi` project in the Debug mode.

Now You don't need to install PostreSQL locally and getting all the benefits from having a debugger attached to Your .Net application.
