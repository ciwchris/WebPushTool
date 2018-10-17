using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using WebPush;

namespace WebPushTool
{
    [Subcommand("Send", typeof(SendCommand))]
    [Subcommand("Generate", typeof(GenerateCommand))]
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private int OnExecute()
        {
            return 0;
        }
    }

    [Command(Name = "Generate", Description = "Generate VAPID keys for web push notifications.")]
    class GenerateCommand
    {
        private void OnExecute()
        {
            var vapidKeys = VapidHelper.GenerateVapidKeys();

            var publicKey = $"const vapidPublicKey = '{vapidKeys.PublicKey}';";
            var privateKey = $"const vapidPrivateKey = '{vapidKeys.PrivateKey}';";

            File.WriteAllText("service-worker.js", serviceWorkerFile);
            File.WriteAllText("index.html", indexFile);
            File.WriteAllLines("app.js", new string[] { publicKey, privateKey, appFile });

            var bytes = Convert.FromBase64String(iconFile);
            using (var imageFile = new FileStream("icon.png", FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            Console.WriteLine(
                $"\nGenerated!\n\nPublic key: {vapidKeys.PublicKey}\nPrivate key: {vapidKeys.PrivateKey}\n\n" +
                "- Run dotnet serve -o\n" +
                "- Click the Subscribe button\n" +
                "- Copy command from textarea (Send command)\n" +
                "- Paste command into terminal to send push notification\n");
        }

        private const string serviceWorkerFile = @"
self.addEventListener('push', function(event) {
    const payload = event.data ? event.data.text() : 'no payload';
    event.waitUntil(
        self.registration.showNotification('Notification from web-push', {
            body: payload,
            icon: 'icon.png'
        })
    );
});
";

        private const string iconFile = "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 ";

        private const string indexFile = @"
<!DOCTYPE html>
<html lang=""en"">

<body>
    <textarea rows=""6"" cols=""200"" id=""command""></textarea>
    <button id=""subscribe"" style=""display: none"">Subscribe</button>
    <button id=""unsubscribe"" style=""display: none"">Unsubscribe</button>

    <script type=""text/javascript"" src=""app.js""></script>
</body>

</html>
";

        private const string appFile = @"
navigator.serviceWorker.register('service-worker.js');

const subscribe = document.getElementById('subscribe');
const unsubscribe = document.getElementById('unsubscribe');

navigator.serviceWorker.ready.then(function(registration) {
    return registration.pushManager.getSubscription().then(async function(subscription) {
        if (subscription) unsubscribe.style.display = 'block';
        else subscribe.style.display = 'block';
    });
});

subscribe.addEventListener('click', function(event) {
    navigator.serviceWorker.ready
        .then(function(registration) {
            return registration.pushManager.getSubscription().then(async function(subscription) {
                if (subscription) return subscription;

                const convertedVapidKey = urlBase64ToUint8Array(vapidPublicKey);

                return registration.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: convertedVapidKey
                });
            });
        })
        .then(function(subscription) {
            const subscriptionObject = JSON.parse(JSON.stringify(subscription));
            const command = `dotnet vapid-key-generator send ""${vapidPublicKey}"" ""${vapidPrivateKey}"" ""${subscriptionObject.endpoint}"" ""${subscriptionObject.keys.p256dh}"" ""${subscriptionObject.keys.auth}""`;
            document.getElementById('command').innerText = command;
            subscribe.style.display = 'none';
            unsubscribe.style.display = 'block';
        });
});

unsubscribe.addEventListener('click', function(event) {
    navigator.serviceWorker.ready.then(function(reg) {
        reg.pushManager.getSubscription().then(function(subscription) {
            subscription.unsubscribe().then(function(successful) {
                document.getElementById('command').innerText = 'unsubscribed';
                subscribe.style.display = 'block';
                unsubscribe.style.display = 'none';
            });
        });
    });
});

// This function is needed because Chrome doesn't accept a base64 encoded string
// as value for applicationServerKey in pushManager.subscribe yet
// https://bugs.chromium.org/p/chromium/issues/detail?id=802280
function urlBase64ToUint8Array(base64String) {
    var padding = '='.repeat((4 - (base64String.length % 4)) % 4);
    var base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/');

    var rawData = window.atob(base64);
    var outputArray = new Uint8Array(rawData.length);

    for (var i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}
";
    }

    [Command(Description = "Send push notification")]
    class SendCommand
    {
        [Argument(0, "PublicKey", "Public key generated by the Generate command")]
        public string PublicKey { get; }

        [Argument(1, "PrivateKey", "Private key generated by the Generate command")]
        public string PrivateKey { get; }

        [Argument(2, "PushEndpoint", "Endpoint generated by the device")]
        public string PushEndpoint { get; }

        [Argument(3, "P256DH", "P256DH generated by the device")]
        public string PushP256DH { get; }

        [Argument(4, "PushAuth", "Auth generated by the device")]
        public string PushAuth { get; }

        [Option(Description = "Message to send in WebPush")]
        public string Message { get; } = "Test message";

        private void OnExecute()
        {
            var pushSubscription = new PushSubscription(PushEndpoint, PushP256DH, PushAuth);
            var vapidDetails = new VapidDetails("mailto:example@example.com", PublicKey, PrivateKey);

            var webPushClient = new WebPushClient();
            webPushClient.SendNotification(pushSubscription, Message, vapidDetails);
        }
    }

}
