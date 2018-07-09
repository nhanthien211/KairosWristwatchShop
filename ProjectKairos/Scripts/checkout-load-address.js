var baseService = "/Service";
var cityUrl = baseService + "/GetAllCities";
var districtUrl = baseService + "/GetDistrictByCity";
var wardUrl = baseService + "/GetWardByDistrict";

jQuery(document).ready(function () {
    _getCity();
    $("#shipDistrict").html('<option value="" selected>-- Select a district --</option>');
    $("#shipWard").html('<option value="" selected>-- Select a ward --</option>');

    $("#shipCity").on('change', function () {
        var cityID = $(this).val();
        if (cityID != undefined && cityID != '') {
            _getDistrict(cityID);
        } else {
            $("#shipDistrict").html('<option value="" selected>-- Select a district --</option>');
            $("#shipWard").html('<option value="" selected>-- Select a ward --</option>');
        }
    });

    $("#shipDistrict").on('change', function () {
        var districtID = $(this).val();
        if (districtID != undefined && districtID != '') {
            _getWard(districtID);
        } else {
            $("#shipWard").html('<option value="" selected>-- Select a ward --</option>');
        }
    });
});

function _getCity() {
    $.get(cityUrl, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="" selected>-- Select a city --</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.CityID + '>' + item.Type + ' ' + item.CityName + '</option>';
            });
            $("#shipCity").html(html);
            $("#shipDistrict").html('<option value="" selected>-- Select a district --</option>');
            $("#shipWard").html('<option value="" selected>-- Select a ward --</option>');
        }
    });
};

function _getDistrict(cityID) {
    $.get(districtUrl + "/" + cityID, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="" selected>-- Select a district --</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.DistrictID + '>' + item.Type + ' ' + item.DistrictName + '</option>';
            });
            $("#shipDistrict").html(html);
            $("#shipWard").html('<option value="" selected>-- Select a ward --</option>');
        }
    });
}

function _getWard(districtID) {
    $.get(wardUrl + "/" + districtID, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="">-- Select a ward --</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.WardID + '>' + item.Type + ' ' + item.WardName + '</option>';
            });
            $("#shipWard").html(html);
        }
    });
}