self.addEventListener('fetch', function (event) { });

self.addEventListener('push', function (e) {
    var options;
    if (e.data) {
        options = JSON.parse(e.data.text())
    } else {
        throw new Error('Invalid notification object');
    }
    
    e.waitUntil(
        self.registration.showNotification("Push Notification", options)
    );
});

self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;
    if (action === 'close') {
        notification.close();
    } else {
        // Some actions
        clients.openWindow(notification.data.navigateTo);
        notification.close();
    }
});