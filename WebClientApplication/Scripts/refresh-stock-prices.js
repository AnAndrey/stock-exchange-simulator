$('document').ready(function() {
    var url = "StockPrices/Home";
    var notifications = $("#stock-prices-table");
    setInterval(function () {
        notifications.load(url);
    }, 3000);
})
