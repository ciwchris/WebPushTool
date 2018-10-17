# WebPushTool

Dotnet tool to experiment sending web push notifications to web sites.

## Create and install the tool

```
dotnet pack
dotnet tool install --global --add-source ./bin/Debug WebPushTool
```

## Run the tool

Running the Generate command will create a public and private VAPID key pair and output a sample
website which uses the key pair.

```
dotnet web-push generate
```

Follow the instructions from the command output. This will run the site, subscribe to push
notifications and use the Send command to send a push notification to the sample site. See `dotnet
web-push send --help` for more information.

## Uninstall the tool

```
dotnet tool uninstall --global WebPushTool
```

