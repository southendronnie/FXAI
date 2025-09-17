window.renderCandleChart = function (containerId, data) {
    Highcharts.stockChart(containerId, {
        rangeSelector: { selected: 1 },
        series: [{
            type: 'candlestick',
            name: 'EUR/USD',
            data: data,
            tooltip: { valueDecimals: 5 }
        }]
    });
};