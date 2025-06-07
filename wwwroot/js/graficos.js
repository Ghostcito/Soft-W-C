document.addEventListener('DOMContentLoaded', function () {

    // === 1. Horas trabajadas por empleado ===
    if (window.horasTrabajadasLabels && window.horasTrabajadasData) {
        new Chart(document.getElementById('chartAsistencia'), {
            type: 'bar',
            data: {
                labels: window.horasTrabajadasLabels,
                datasets: [{
                    label: 'Horas trabajadas',
                    data: window.horasTrabajadasData,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)'
                }]
            },
            options: {
                plugins: {
                    legend: { labels: { font: { size: 14 } } }
                },
                scales: {
                    x: { ticks: { font: { size: 14 } } },
                    y: { ticks: { font: { size: 14 } } }
                }
            }
        });
    }

    // === 2. Empleados por sede ===
    if (window.sedesLabels && window.sedesData) {
        new Chart(document.getElementById('chartSede'), {
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
                plugins: {
                    legend: { labels: { font: { size: 14 } } }
                }
            }
        });
    }

    // === 3. Empleados por turno ===
    if (window.turnosLabels && window.turnosData) {
        new Chart(document.getElementById('chartTurno'), {
            type: 'bar',
            data: {
                labels: window.turnosLabels,
                datasets: [{
                    label: 'Empleados por turno',
                    data: window.turnosData,
                    backgroundColor: 'rgba(255, 99, 132, 0.6)'
                }]
            },
            options: {
                plugins: {
                    legend: { labels: { font: { size: 14 } } }
                },
                scales: {
                    x: { ticks: { font: { size: 14 } } },
                    y: { ticks: { font: { size: 14 } } }
                }
            }
        });
    }

    // === 4. Supervisores con más empleados ===
    if (window.supervisoresLabels && window.supervisoresData) {
        new Chart(document.getElementById('chartSupervisores'), {
            type: 'bar',
            data: {
                labels: window.supervisoresLabels,
                datasets: [{
                    label: 'Cantidad de empleados supervisados',
                    data: window.supervisoresData,
                    backgroundColor: 'rgba(153, 102, 255, 0.6)'
                }]
            },
            options: {
                plugins: {
                    legend: { labels: { font: { size: 14 } } }
                },
                scales: {
                    x: { ticks: { font: { size: 14 } } },
                    y: { ticks: { font: { size: 14 } } }
                }
            }
        });
    }

    // === 5. Horas trabajadas por semana (evolución temporal) ===
    if (window.semanasLabels && window.semanasData) {
        new Chart(document.getElementById('chartHorasSemana'), {
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
            options: {
                plugins: {
                    legend: { labels: { font: { size: 14 } } }
                },
                scales: {
                    x: { ticks: { font: { size: 14 } } },
                    y: { ticks: { font: { size: 14 } } }
                }
            }
        });
    }
    if (window.mesesPagosLabels && window.mesesPagosData && window.mesesPagosLabels.length > 0 && window.mesesPagosData.length > 0) {
            const ctx = document.getElementById('graficoPagosMensuales').getContext('2d');
            new Chart(ctx, {
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
                    plugins: {
                        legend: { labels: { font: { size: 14 } } }
                    },
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

});
