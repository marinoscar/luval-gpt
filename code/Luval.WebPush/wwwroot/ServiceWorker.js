self.addEventListener('fetch', function (event) { });

self.addEventListener('push', function (e) {
    var body;
    var payloadOptions;
    if (e.data) {
        payloadOptions = JSON.parse(e.data.text())
        body = payloadOptions.body;
    } else {
        body = "Standard Message";
    }
    

    var options = {
        body: body,
        icon: "images/icon-512x512.png",
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now()
        },
        actions: [
            {
                action: "explore", title: "Go interact with this!",
                icon: "images/checkmark.png"
            },
            {
                action: "close", title: "Ignore",
                icon: "images/red_x.png"
            },
        ]
    };
    e.waitUntil(
        self.registration.showNotification("Push Notification", payloadOptions)
    );
});

self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;
    console.log(e);
    if (action === 'close') {
        notification.close();
    } else {
        // Some actions
        clients.openWindow(notification.data.navigateTo);
        notification.close();
    }
});