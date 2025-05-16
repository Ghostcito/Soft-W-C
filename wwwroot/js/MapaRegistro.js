
// Inicializar el mapa
const map = L.map('map').setView([-16.398833,-71.536970], 12); // Centro inicial en Bogotá
        
// Añadir capa base
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

let marker;
        
        // Función para actualizar marcador y coordenadas
function updateMarker(latlng) {
    if (marker) {
        map.removeLayer(marker);
    }
            
    marker = L.marker(latlng, {
        draggable: true
    }).addTo(map)
        .bindPopup(`Ubicación seleccionada<br>Lat: ${latlng.lat.toFixed(6)}<br>Lng: ${latlng.lng.toFixed(6)}`)
        .openPopup();
            
    // Actualizar campos del formulario
    document.getElementById('latitud-input').value = latlng.lat.toFixed(6);
    document.getElementById('longitud-input').value = latlng.lng.toFixed(6);
    // Permitir arrastrar el marcador
    marker.on('dragend', function(e) {
        const newLatLng = e.target.getLatLng();
        updateMarker(newLatLng);
    });
}

// Evento click en el mapa
map.on('click', function(e) {
    updateMarker(e.latlng);
});

// Configurar geocoder
const geocoder = L.Control.geocoder({
    defaultMarkGeocode: false,
    position: 'topright'
}).on('markgeocode', function(e) {
    const result = e.geocode;
    const address = result.properties.address || {};

    map.setView(result.center, 16);
    updateMarker(result.center);
            
    document.getElementById('Direccion_local').value = e.geocode.name;
    document.getElementById('Ciudad').value = address.city || '';
    document.getElementById('Provincia').value = address.state || '';
            
}).addTo(map);

// Buscador manual
document.getElementById('search-button').addEventListener('click', function() {
    const address = document.getElementById('address-search').value;
    if (address) {
        fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}`)
            .then(response => response.json())
            .then(data => {
                if (data.length > 0) {
                    const firstResult = data[0];
                    const latlng = L.latLng(firstResult.lat, firstResult.lon);
                    map.setView(latlng, 16);
                    updateMarker(latlng);
                } else {
                    alert('Dirección no encontrada');
                }
            });
    }
});

