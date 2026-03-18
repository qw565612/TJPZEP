// The speed gauge
Highcharts.chart('container', {

    chart: {
        type: 'solidgauge'
    },

    credits: {
        enabled: false
    },

    title: {
        text: '進度'
    },

    subtitle: {
        text: 'Individual point radius in Highcharts'
    },

    pane: {
        center: ['50%', '85%'],
        size: '140%',
        startAngle: -90,
        endAngle: 90,
        background: {
            backgroundColor: '#EEE',
            innerRadius: '60%',
            outerRadius: '100%',
            shape: 'arc'
        }
    },

    // the value axis
    yAxis: {
        lineWidth: 0,
        minorTickInterval: null,
        min: 0,
        max: 100,
        tickPixelInterval: 400,
        tickWidth: 0,
        labels: {
            y: 16
        }
    },

    series: [{
        name: '％',
        data: [{
            name: '預定進度',
            radius: 98,
            innerRadius: 82,
            y: 20,
            color:'red'
        }, {
            name: '實際進度',
            radius: 78,
            innerRadius: 62,
            y: 30,
            color:'blue'
        }],
        dataLabels: {
            enabled: false
        },
        tooltip: {
            pointFormat: '{point.name}: <b>{point.y}</b> km/h'
        }
    }]
});
