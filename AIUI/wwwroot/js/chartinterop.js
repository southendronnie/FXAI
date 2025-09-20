window.renderOhlcWithTicks = function (ohlcData, tickData) {
    Highcharts.stockChart('chartContainer', {
        rangeSelector: { selected: 1 },
        title: { text: 'Live OHLC + Tick Overlay' },
        yAxis: [{
            labels: { align: 'right', x: -3 },
            title: { text: 'Price' },
            height: '80%',
            lineWidth: 2
        }, {
            labels: { align: 'right', x: -3 },
            title: { text: 'Ticks' },
            top: '80%',
            height: '20%',
            offset: 0,
            lineWidth: 2
        }],
        series: [
            {
                type: 'candlestick',
                name: 'OHLC',
                data: ohlcData,
                tooltip: { valueDecimals: 5 },
                yAxis: 0
            },
            {
                type: 'line',
                name: 'Ticks',
                data: tickData,
                color: '#00BFFF',
                tooltip: { valueDecimals: 5 },
                yAxis: 1
            }
        ]
    });
};