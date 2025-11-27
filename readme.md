# Dapr Actors Multi-App Project

This project demonstrates Dapr actors with a service that hosts actors and a client that interacts with them.

## Project Structure

- `service/` - Actor service that hosts the LightActor
- `client/` - Client application that calls the actor
- `interfaces/` - Shared interfaces for the actor (ILight, LightData)

## Prerequisites

1. Install Dapr CLI: https://docs.dapr.io/getting-started/install-dapr-cli/
2. Install Redis (for state store):
   ```bash
   # macOS with Homebrew
   brew install redis
   brew services start redis
   
   # Or use Docker
   docker run -d -p 6379:6379 redis:alpine
   ```

## Running the Multi-App


### Option 1: Using Individual Dapr Files (Recommended)

Each project has its own `dapr.yaml` file for independent execution:

```bash
# Terminal 1 - Start the actor service
cd service
dapr run -f dapr.yaml

# Terminal 2 - Start the client (after service is running)
cd client
dapr run -f dapr.yaml
```

### Option 2: Using Dapr Multi-App from Root 

```bash
# Start both applications with Dapr from the root directory
dapr run -f dapr-multiapp.yaml
```

This will start:
- LightActor service on port 5008 with Dapr sidecar
- Actor client with Dapr sidecar



### Option 3: Manual Dapr Run

```bash
# Terminal 1 - Start the actor service
cd service
dapr run --app-id ActorService --app-port 5008 --resources-path ../resources -- dotnet run --framework net8.0

# Terminal 2 - Start the client (after service is running)
cd client
dapr run --app-id ActorClient --resources-path ../resources -- dotnet run --framework net8.0
```


## What the Application Does

The client application:
1. Creates a proxy to a LightActor with ID "light-42"
2. Increases the brightness by 1, then by 10
3. Gets the current brightness value
4. Decreases the brightness by 1 twice
5. Resets the brightness to 0
6. Performs 10 concurrent brightness increase operations
7. Monitors the auto-dim feature (brightness decreases every 20 seconds)

The actor service:
1. Hosts the LightActor implementation
2. Provides state management for brightness level
3. Auto-dims the light by 1 every 20 seconds using Dapr reminders (if brightness > 0)

## Testing the Actor Service

You can test the actor service directly using HTTP requests without running the client:

1. Start the actor service (using any of the options above)
2. Open `service/request.http` in VS Code
3. Use the REST Client extension to send requests directly to the Dapr sidecar

The `request.http` file includes sample requests to:
- Get the current brightness
- Increase brightness by a specified amount
- Decrease brightness by a specified amount

The requests use the Dapr HTTP endpoint on port 3500 to invoke actor methods.

## Configuration Files

- `dapr-multiapp.yaml` - Multi-app configuration for running both services from root
- `service/dapr.yaml` - Individual Dapr configuration for the actor service
- `client/dapr.yaml` - Individual Dapr configuration for the client
- `config.yaml` - Dapr configuration with tracing and metrics
- `resources/` - Dapr components (actorstore.yaml for Redis state store)

## Monitoring

- Service metrics: http://localhost:9090/metrics
- Client metrics: http://localhost:9091/metrics

