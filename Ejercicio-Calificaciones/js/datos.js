var Datos = function () {

    var url = 'Cargar-Calificaciones.aspx/';

    var idVentaCurrent = 0;

    var callAjax = function (method, params, onSuccess, onError) {
        $.ajax({
            type: 'POST',
            url: url + method,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: params,
            responseType: "json",
            success: function (data) {
                onSuccess(data);
            },
            error: function (error) {
                onError(error);
            }
        });
    };

    var cargarGraficaInformacion = function (canvas, funcion, etiqueta) {
        callAjax(funcion, '', function (result) {
            var jsonfile = JSON.parse(result.d);
            var ctx = document.getElementById(canvas);

            var lblTitulo = document.getElementById('lblGraficaInformacion');

            lblTitulo.innerText = 'Esta gráfica muestra la cantidad de alumnos que presentan la misma calificación.';

            if (jsonfile == null) {
                lblTitulo.innerText = '';
            }

            //Declaro la variable 
            var json =
            {
                datas: [],
                background: [],
                label: []
            }

            //Lleno la variable "json" con cada item de la lista
            jsonfile.forEach(function (item) {
                json.datas.push(item.EjeY);
                json.background.push(item.Color);
                json.label.push(item.EjeX);
            });


            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: json.label,
                    datasets: [{
                        label: etiqueta,
                        data: json.datas,
                        backgroundColor: json.background,
                        borderColor: json.background,
                        borderWidth: 1
                    }]
                },
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    },
                    legend: { position: 'bottom' },
                    responsive: true
                }
            });

        }, function (error) {
            console.log(error);

        });
    };

    var cargarGraficaPromedio = function (canvas, funcion, etiqueta) {
        callAjax(funcion, '', function (result) {

            var lblTitulo = document.getElementById('lblGraficaPromedio');

            lblTitulo.innerText = 'Esta gráfica muestra el promedio de las calificaciones de cada grado.';

            var jsonfile = JSON.parse(result.d);

            if (jsonfile == null) {
                lblTitulo.innerText = '';
            }
            var ctx = document.getElementById(canvas);

            //Declaro la variable vacia
            var json =
            {
                datas: [],
                label: []
            }

            //Lleno la variable "json" con cada item de la lista
            jsonfile.forEach(function (item) {
                json.datas.push(item.EjeY);
                json.label.push(item.EjeX);
            });


            var myChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: json.label,
                    datasets: [{
                        data: json.datas,
                        label: etiqueta,
                        backgroundColor: '#0040FF',
                        borderColor: '#0040FF',
                        borderWidth: 3,
                        fill: false
                    }]
                },
                options: {
                    legend: { position: 'bottom' },
                    responsive: true,
                    maintainAspectRatio: true
                }
            });

        }, function (error) {
            console.log(error);

        });
    };

    var cargarAlumno = function (lblAlumno, funcion, leyenda) {
        callAjax(funcion, '', function (result) {
            var jsonfile = JSON.parse(result.d);
            
            var label = document.getElementById(lblAlumno);

            label.innerText = leyenda +' ' + jsonfile.Nombres + ' ' + jsonfile.ApellidoPaterno + ' ' + jsonfile.ApellidoMaterno;

        }, function (error) {
            console.log(error);

        });
    };

    var cargarPromedio = function (lblAlumno, funcion, leyenda) {
        callAjax(funcion, '', function (result) {
            var jsonfile = JSON.parse(result.d);

            var label = document.getElementById(lblAlumno);

            label.innerText = leyenda + ' ' + jsonfile.toFixed(2);

        }, function (error) {
            console.log(error);

        });
    };

    var CargarInformacion = function () {
 

            cargarGraficaInformacion('cnvCalificaciones', 'ObtenerGraficaInformacionAlumnos', 'Calificación');
            cargarGraficaPromedio('cnvPromedio', 'ObtenerGraficaPromedio', 'Promedio');
            cargarAlumno('lblMenorCalificacion', 'ObtenerPeorCalificacion', 'El estudiante con la calificación más alta:');
            cargarAlumno('lblMejorCalificacion', 'ObtenerMayorCalificacion', 'El estudiante con la calificación más baja:');
            cargarPromedio('lblPromedioCalificacion', 'ObtenerPromedioCalificaciones', 'El promedio de las calificaciones de todos los estudiantes es:');
    };

    var init = function () {
        $(document).ready(function () {
            
        });

        $(document).on('click', '#btnGenerarInformacion', function () {

            CargarInformacion();
        });
    }

    init();
}

Datos();
