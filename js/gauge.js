// The speed gauge
Highcharts.chart('container', {

  chart: {
      type: 'solidgauge',
      //backgroundColor: 'transparent'
  },

  credits: {
      enabled: false
  },

title: {
      text: '預定/實際進度表'
},

  subtitle: {
      text: '更新日期：2024/08/14'
    },
  //gauge 根本沒有legend，所以下面5行都是無作用的^^
  legend: {
      align: 'center',
      verticalAlign: 'top',
      layout: 'vertical',
      x: 0,
      y: 0
    },

  
  pane: {
      center: ['50%', '85%'],
      size: '100%',
      startAngle: -90,
      endAngle: 90,
      background: {
          backgroundColor: '#2e318b',
          borderWidth: 0,
          innerRadius: '57%',
          outerRadius: '103%',
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
          y: 5.44,
          color:'#06A3DA'
      }, {
          name: '實際進度',
          radius: 78,
          innerRadius: 62,
          y: 3.20,
          color:'#F57E57'
      }],
      dataLabels: {
          enabled: false
      },
      tooltip: {
          pointFormat: '{point.name}: <b>{point.y}</b> %'
      }
  }]
});
