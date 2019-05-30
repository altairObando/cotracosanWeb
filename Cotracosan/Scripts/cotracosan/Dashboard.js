// Variables globales para el contenido
var etiquetaMeses = null;
var dataSets = null;
var valorMaximo = null;
// Obtener los datos por mes
$(function () {
    $.ajax({
        url: "/Home/GetDataSets",
        type : "GET",
        dataType: "JSON",
        success: function (data) {
            etiquetaMeses = data.labels;
            valorMaximo = data.maximo;
            dataSets = []
            dataSets["Carreras"] = [];
            dataSets["Creditos"] = [];
            dataSets["Abonos"] = [];

            //Llenando carreras
            for(var i=0; i < 12; i++)
            {
                dataSets.Carreras.push(data.Carreras[i].Valor);
                dataSets.Creditos.push(data.Creditos[i].Valor);
                dataSets.Abonos.push(data.Abonos[i].Valor);
            }
            // Cargar Grafico
            cargaGrafico();
        }
    });
});
function cargaGrafico(){
    if(etiquetaMeses.length > 0 && dataSets && $('#graficos-ingresos-egresos').length)
    {
        var grafico = $('#graficos-ingresos-egresos').get(0).getContext("2d");
        var data = {
            labels: etiquetaMeses,
            datasets: [
                {
                    label: "Carreras",
                    data: dataSets.Carreras,
                    borderColor: [
                        '#71c016'
                    ],
                    borderWidth: 2,
                    fill: false,
                    pointBackgroundColor: "#fff"
                },
                {
                    label: "Creditos",
                    data: dataSets.Creditos,
                    borderColor: [
                        '#4d83ff'
                    ],
                    borderWidth: 2,
                    fill: false,
                    pointBackgroundColor: "#fff"
                },
                {
                    label: "Abonos",
                    data: dataSets.Abonos,
                    borderColor: [
                        '#ff4747'
                    ],
                    borderWidth: 2,
                    fill: false,
                    pointBackgroundColor: "#fff"
                },
            ]
        };
        var options = {
            scales: {
                yAxes: [{
                    display: true,
                    gridLines: {
                        drawBorder: false,
                        lineWidth: 1,
                        color: "#e9e9e9",
                        zeroLineColor: "#e9e9e9",
                    },
                    ticks: {
                        min: 0,
                        max: 1500,
                        stepSize: 250,
                        fontColor: "#6c7383",
                        fontSize: 16,
                        fontStyle: 300,
                        padding: 10
                    }
                }],
                xAxes: [{
                    display: true,
                    gridLines: {
                        drawBorder: false,
                        lineWidth: 1,
                        color: "#e9e9e9",
                    },
                    ticks: {
                        fontColor: "#6c7383",
                        fontSize: 16,
                        fontStyle: 300,
                        padding: 15
                    }
                }]
            },
            legend: {
                display: false
            },
            legendCallback: function (chart) {
                var text = [];
                text.push('<ul class="dashboard-chart-legend">');
                for (var i = 0; i < chart.data.datasets.length; i++) {
                    text.push('<li><span style="background-color: ' + chart.data.datasets[i].borderColor[0] + ' "></span>');
                    if (chart.data.datasets[i].label) {
                        text.push(chart.data.datasets[i].label);
                    }
                }
                text.push('</ul>');
                return text.join("");
            },
            elements: {
                point: {
                    radius: 3
                },
                line: {
                    tension: 0
                }
            },
            stepsize: 1,
            layout: {
                padding: {
                    top: 0,
                    bottom: -10,
                    left: -10,
                    right: 0
                }
            }
        };
        var cashDeposits = new Chart(grafico, {
            type: 'line',
            data: data,
            options: options
        });
        document.getElementById('graficos-ingresos-egresos-legend').innerHTML = cashDeposits.generateLegend();
    }
}
// Obtener los elementos del dashboard
$(function () {
    // Obtener los valores del dashboard desde el servidor
    $.getJSON("/Home/GetDashboardData", function (data) {
        // Actualizar en base a resultados.
        $("#placa1").html(data.vehiculo.placa1);
        $("#placa2").html(data.vehiculo.placa1);
        $("#totalCarreras").html(data.vehiculo.carreras);
        $("#totalRecaudado").html(data.vehiculo.monto);
        // Actualizar segundo dashboard
        $("#nombreArticulo1").html(data.credito.nombre1);
        $("#nombreArticulo2").html(data.credito.nombre2);
        $("#totalAcreditado1").html(data.credito.total1);
        $("#totalAcreditado2").html(data.credito.total2);
    });
});
// Obtener los elementos del segundo grafico
var etiquetaFechas = [];
var datos = [];
$(function () {
    $.getJSON("/Home/GetTimeSeries", function (data) {
        // Agregando las fechas al arreglo de etiquetas
        for(var i = 0; i < data.etiquetas.length; i++)
        { 
            etiquetaFechas.push(data.data[i].FechaDia);
            // creando el dataset.
            datos.push(data.data[i].TotalDia);
         }
        // Etiqueta para la leyenda.
        $("#subtitulo").html("Periodo desde: " + data.fechaInicio + " hasta: " + data.fechaFin);
        cargarGrafico2();
    });
});

// Crear segundo grafico
function cargarGrafico2() {
    const ctx = document.getElementById('grafico-time-series').getContext('2d');
    const data = {
        labels : etiquetaFechas,
        datasets: [{
            fill: false,
            label: "Carreras Efectuadas",
            data : datos,
            borderColor: '#4ebaba',
            backgroundColor: '#4ebaba',
            lineTension: 0
        }]
    };
    const options = {
        type: 'bar',
        data: data,
        options: {
            fill: false,
            responsive: true,
            tooltips: {
                callbacks: {
                    label: function (t, d) {
                        var xLabel = "Total recaudado";
                        var yLabel = "C$ " + t.yLabel;
                        return xLabel + " : " + yLabel;
                    }
                }
            },
            scaleLabel: {
                xAxes: [{
                    type: 'time',
                    display : true,
                    scaleLabel: {
                        display: true,
                        labelString: "Date",
                    }
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                    }, 
                    display:true,
                    scaleLabel: {
                        display: true,
                        labelString: "Carreras"
                    }
                }]
            }
        }
    }
    const chart = new Chart(ctx, options);
}