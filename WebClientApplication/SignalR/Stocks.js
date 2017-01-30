if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {

    var stocksHub = $.connection.stocksHub,
                    $stockTableBody = $('#stock-prices-table').find('tbody'),
                    rowTemplate = '<tr data-symbol="{Name}"><td>{Name}</td><td>{Price}</td></tr>';

    function init(){ 
        stocksHub.server.getPricesForStocks().done(function (stocks) {
            $stockTableBody.empty();
            $.each(stocks, function () {
                var stock = formatStock(this);
                $stockTableBody.append(rowTemplate.supplant(stock));
            });
        });
    }

    function formatStock(stock) {
        return {
            Name: stock.Name,
            Price: stock.Price
        };
    }

    stocksHub.client.updateStocks = function (stocks) {
        $.each(stocks, function () {
            var stock = formatStock(this);
            $row = $(rowTemplate.supplant(stock));
            $stockTableBody.find('tr[data-symbol=' + stock.Name + ']')
                       .replaceWith($row);
        });
    }

    // Start the connection
    $.connection.hub.start().done(init);
});