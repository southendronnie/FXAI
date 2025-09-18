window.chartInterop = {
    renderChart: function (elementId, chartOptions) {
        Highcharts.chart(elementId, chartOptions);
    },
    updateSeries: function (elementId, seriesData) {
        const chart = Highcharts.charts.find(c => c && c.renderTo.id === elementId);
        if (chart) {
            chart.series[0].setData(seriesData, true);
        }
    }
};