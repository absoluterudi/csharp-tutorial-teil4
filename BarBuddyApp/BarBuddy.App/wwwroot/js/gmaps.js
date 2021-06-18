var map;
var currentPositionMarker;

function initGMaps(instance)
{
    var options = {
        zoom: 16,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControl: false,
        streetViewControl: false,
        zoomControl: false,
        fullscreenControl: false,
        clickableIcons: false
    };

    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 52.5293144, lng: 13.4030671 },
        options: options,
    });

    map.addListener("dragend", () =>
    {
        window.setTimeout(() =>
        {
            var centerLat = map.getCenter().lat();
            var centerLng = map.getCenter().lng();
            instance.invokeMethod('SetCenterScreenPosition', centerLat, centerLng);
        }, 1000);
    });

    if (navigator.geolocation)
    {
        console.log("geolocation === true");

        const locationButton = document.createElement("button");
        locationButton.innerHTML = "<i class='fas fa-location-arrow'></i>";
        locationButton.classList.add("custom-map-control-button");
        map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(locationButton);
        locationButton.addEventListener("click", () =>
        {
            if (navigator.geolocation)
            {
                navigator.geolocation.getCurrentPosition((position) =>
                {
                    setMyPosition(position);
                });
            }
        });

        navigator.geolocation.getCurrentPosition((position) =>
        {
            setMyPosition(position);
        });
    }
    else
    {
        console.log("geolocation === false");
    }

    var setMyPosition = function (position)
    {
        var pos = {
            lat: position.coords.latitude,
            lng: position.coords.longitude,
        };
        map.setCenter(pos);

        if (currentPositionMarker !== undefined)
        {
            currentPositionMarker.setMap(null);
        }

        currentPositionMarker = new google.maps.Marker({
            position: pos,
            map: map,
            icon: new google.maps.MarkerImage('/images/map-pin.svg', null, null, null, new google.maps.Size(45, 45))
        });

        instance.invokeMethod('SetMyPosition', pos.lat, pos.lng);
    }
}

function addLocationsToGMaps(instance)
{
    var markers = [];

    var clearMarkers = function ()
    {
        for (var i = 0; i < markers.length; i++)
        {
            markers[i].setMap(null);
            delete markers[i];
        }
        markers = [];
    };

    clearMarkers();

    $("#location-list li").each(function ()
    {
        var id = this.id;
        var lat = $(this).attr("data-lat");
        var lng = $(this).attr("data-lng");
        var places = $(this).attr("data-places");

        var marker;
        if (places === "0")
        {
            marker = new google.maps.Marker({
                id: id,
                position: new google.maps.LatLng(lat, lng),
                map: map,
                label: { text: places, color: "white" },
                icon: new google.maps.MarkerImage('/images/circle-red.svg', null, null, null, new google.maps.Size(30, 30))
            });
        }
        else
        {
            marker = new google.maps.Marker({
                id: id,
                position: new google.maps.LatLng(lat, lng),
                map: map,
                label: { text: places, color: "white" },
                icon: new google.maps.MarkerImage('/images/circle-green.svg', null, null, null, new google.maps.Size(30, 30))
            });
        }

        markers.push(marker);

        google.maps.event.addListener(marker, 'click', function ()
        {
            console.log(marker.id)
            instance.invokeMethod('SetSelectedLocation', marker.id);
        });
    });
}