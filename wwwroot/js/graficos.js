document.addEventListener('DOMContentLoaded', function () {
    // Función auxiliar para configuraciones comunes
    const commonOptions = {
        plugins: {
            legend: { 
                labels: { 
                    font: { 
                        size: 14 
                    } 
                } 
            }
        },
        scales: {
            x: { 
                ticks: { 
                    font: { 
                        size: 14 
                    } 
                } 
            },
            y: { 
                ticks: { 
                    font: { 
                        size: 14 
                    } 
                } 
            }
        }
    };

    // === 1. Horas trabajadas por empleado ===
    try {
        const chartElement = document.getElementById('chartAsistencia');
        if (chartElement && window.horasTrabajadasLabels && window.horasTrabajadasData) {
            new Chart(chartElement, {
                type: 'bar',
                data: {
                    labels: window.horasTrabajadasLabels,
                    datasets: [{
                        label: 'Horas trabajadas',
                        data: window.horasTrabajadasData,
                        backgroundColor: 'rgba(54, 162, 235, 0.6)'
                    }]
                },
                options: commonOptions
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de horas trabajadas:', error);
    }

    // === 2. Empleados por sede ===
    try {
        const chartElement = document.getElementById('chartSede');
        if (chartElement && window.sedesLabels && window.sedesData) {
            new Chart(chartElement, {
                type: 'doughnut',
                data: {
                    labels: window.sedesLabels,
                    datasets: [{
                        label: 'Empleados por sede',
                        data: window.sedesData,
                        backgroundColor: [
                            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'
                        ]
                    }]
                },
                options: {
                    plugins: commonOptions.plugins
                }
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de empleados por sede:', error);
    }

    // === 3. Empleados por turno ===
    try {
        const chartElement = document.getElementById('chartTurno');
        if (chartElement && window.turnosLabels && window.turnosData) {
            new Chart(chartElement, {
                type: 'bar',
                data: {
                    labels: window.turnosLabels,
                    datasets: [{
                        label: 'Empleados por turno',
                        data: window.turnosData,
                        backgroundColor: 'rgba(255, 99, 132, 0.6)'
                    }]
                },
                options: commonOptions
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de empleados por turno:', error);
    }

    // === 4. Supervisores con más empleados ===
    try {
        const chartElement = document.getElementById('chartSupervisores');
        if (chartElement && window.supervisoresLabels && window.supervisoresData) {
            new Chart(chartElement, {
                type: 'bar',
                data: {
                    labels: window.supervisoresLabels,
                    datasets: [{
                        label: 'Cantidad de empleados supervisados',
                        data: window.supervisoresData,
                        backgroundColor: 'rgba(153, 102, 255, 0.6)'
                    }]
                },
                options: commonOptions
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de supervisores:', error);
    }

    // === 5. Horas trabajadas por semana ===
    try {
        const chartElement = document.getElementById('chartHorasSemana');
        if (chartElement && window.semanasLabels && window.semanasData) {
            new Chart(chartElement, {
                type: 'line',
                data: {
                    labels: window.semanasLabels,
                    datasets: [{
                        label: 'Total horas por semana',
                        data: window.semanasData,
                        borderColor: 'rgba(75, 192, 192, 1)',
                        backgroundColor: 'rgba(75, 192, 192, 0.3)',
                        tension: 0.3
                    }]
                },
                options: commonOptions
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de horas por semana:', error);
    }

    // === 6. Pagos mensuales ===
    try {
        const chartElement = document.getElementById('graficoPagosMensuales');
        if (chartElement && window.mesesPagosLabels && window.mesesPagosData && 
            window.mesesPagosLabels.length > 0 && window.mesesPagosData.length > 0) {
            new Chart(chartElement, {
                type: 'bar',
                data: {
                    labels: window.mesesPagosLabels,
                    datasets: [{
                        label: 'Total a pagar por mes',
                        data: window.mesesPagosData,
                        backgroundColor: 'rgba(54, 162, 235, 0.6)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    ...commonOptions,
                    scales: {
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Monto en S/.'
                            },
                            ticks: { font: { size: 14 } }
                        },
                        x: {
                            title: {
                                display: true,
                                text: 'Mes/Año'
                            },
                            ticks: { font: { size: 14 } }
                        }
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de pagos mensuales:', error);
    }

    // === 7. Tipos de empleado ===
    try {
        const chartElement = document.getElementById('chartTipoEmpleado');
        if (chartElement && window.tiposEmpleadoLabels && window.tiposEmpleadoData) {
            new Chart(chartElement, {
                type: 'bar',
                data: {
                    labels: window.tiposEmpleadoLabels,
                    datasets: [{
                        label: 'Promedio de Evaluación',
                        data: window.tiposEmpleadoData,
                        backgroundColor: 'rgba(255, 159, 64, 0.6)'
                    }]
                },
                options: {
                    ...commonOptions,
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 4,
                            ticks: { 
                                stepSize: 1,
                                font: { size: 14 }
                            }
                        },
                        x: {
                            ticks: { font: { size: 14 } }
                        }
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de tipos de empleado:', error);
    }

    // === 8. Criterios de evaluación ===
    try {
        const chartElement = document.getElementById('chartCriterios');
        if (chartElement && window.criteriosLabels && window.criteriosData) {
            new Chart(chartElement, {
                type: 'radar',
                data: {
                    labels: window.criteriosLabels,
                    datasets: [{
                        label: 'Promedio por Criterio',
                        data: window.criteriosData,
                        backgroundColor: 'rgba(54, 162, 235, 0.3)',
                        borderColor: 'rgba(54, 162, 235, 1)'
                    }]
                },
                options: {
                    plugins: commonOptions.plugins,
                    scales: {
                        r: {
                            min: 0,
                            max: 4,
                            ticks: {
                                stepSize: 1,
                                font: { size: 14 }
                            }
                        }
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error al crear gráfica de criterios:', error);
    }
});