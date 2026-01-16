mapboxgl.accessToken = 'pk.eyJ1IjoiZ2Fyb3NoY3Bha2V0b20iLCJhIjoiY2x1NDE5amc2MTd5dzJtbng1d2VvdjQ3dyJ9.SKOegY6VOMGk0z_RWDCj6w';

const getRandomColor = () => {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

const createRoute_geoJson = (trip) => {
    const routePoints = [];

    trip.find('.route').each((index, routePoint) => {
        const latitude = parseFloat($(routePoint).find('.latitude').text());
        const longitude = parseFloat($(routePoint).find('.longitude').text());
        routePoints.push([longitude, latitude]);
    });

    return {
        'type': 'Feature',
        'properties': {},
        'geometry': {
            'type': 'LineString',
            'coordinates': routePoints
        }
    };
};

const createTripObject = (tripElement) => {
    const trip = $(tripElement);

    const startPoint = trip.find('.start-point');
    const endPoint = trip.find('.end-point');

    return {
        tripId: trip.find(".trip-id").text(),
        startPoint: [parseFloat(startPoint.find('.longitude').text()), parseFloat(startPoint.find('.latitude').text())],
        endPoint: [parseFloat(endPoint.find('.longitude').text()), parseFloat(endPoint.find('.latitude').text())],
        startTime: trip.find('.start-time').text(),
        endTime: trip.find('.end-time').text(),
        startPointDescription: trip.find('.start-point-description').text(),
        endPointDescription: trip.find('.end-point-description').text(),
        route: createRoute_geoJson(trip)
    };
};

const trips = $(".trip").map((index, tripElement) => createTripObject(tripElement)).get();

const map = new mapboxgl.Map({
    container: 'map',
    center: trips[0].startPoint,
    zoom: 13,
    style: 'mapbox://styles/mapbox/streets-v11',
})

const addTripToMap = (trip) => {
    new mapboxgl.Marker($('<div class="marker"></div>')[0])
        .setLngLat(trip.startPoint)
        .setPopup(new mapboxgl.Popup({offset: 25}).setHTML(`<h4>Trip ${trip.tripId} start</h4><p>${trip.startTime}</p>`))
        .addTo(map);
    new mapboxgl.Marker($('<div class="marker"></div>')[0])
        .setLngLat(trip.endPoint)
        .setPopup(new mapboxgl.Popup({offset: 25}).setHTML(`<h4>Trip ${trip.tripId} end</h4><p>${trip.endTime}</p>`))
        .addTo(map);
    map.addLayer({
        id: `route-${trip.tripId}`,
        type: 'line',
        source: {
            'type': 'geojson',
            'data': trip.route
        },
        layout: {
            'line-join': 'round',
            'line-cap': 'round'
        },
        paint: {
            'line-color': getRandomColor(),
            'line-width': 8
        }
    });
}

map.on('load', function () {
    trips.forEach((trip) => addTripToMap(trip))
})

