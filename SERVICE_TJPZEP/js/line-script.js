Highcharts.chart('container2', {

  title: {
    
  },
    title: {
        text: '施工期程進度'
    },

    subtitle: {
        text: '更新日期：2024/08/14',        
        
        style: {
            color: '#385b66',
            fontWeight: 'bold'
        }
    },
  yAxis: {
    title: {
      text: '施工進度'
    }
  },

  xAxis: {
    categories: [
      '113年3月','113年6月','113年9月','113年12月','114年3月','114年6月','114年9月','114年12月','115年3月','115年6月','115年9月','115年12月','116年3月','116年6月','116年9月','116年12月'
    ],
    crosshair: true
  },
  
  legend: {
    layout: 'vertical',
    align: 'center',
    verticalAlign: 'top',
    x: 0,
    y: 0
  },

 

  series: [{
    name: '預定進度',
    data: [0.1,3.60,12,17,22,30,31,45,50,50,61,70,79,83,93,100 
],
	color: '#06A3DA'
  }, {
    name: '實際進度',
    data: [0.36,4.23
],
	color:'#F57E57'
  }],

  responsive: {
    rules: [{
      condition: {
        maxWidth: 500
      },
      chartOptions: {
        legend: {
          layout: 'horizontal',
          align: 'center',
          verticalAlign: 'bottom'
        }
      }
    }]
  }

});