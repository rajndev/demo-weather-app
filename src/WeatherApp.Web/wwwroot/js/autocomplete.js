$(function () {
    $("#searchterm").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Home/GetAutocompleteList',
                data: { "cityname": request.term },
                type: "POST",
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            citycode: item.citycode,
                            name: item.name,
                            state: item.state,
                            country: item.country
                        }
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (event, ui) {

            $('#CityIdHidden').val(ui.item.citycode);
            $('#CityNameHidden').val(ui.item.name);
            $('#StateHidden').val(ui.item.state);
            $('#CountryHidden').val(ui.item.country);

            if (ui.item.state != "") {
                $(this).val(ui.item.name + ", " + ui.item.state + ", " + ui.item.country);
            }
            else {
                $(this).val(ui.item.name +  ", " + ui.item.country);
            }
            return false;
        },

        minLength: 3
    }).data("ui-autocomplete")._renderItem = function (ul, item) {

        if (item.state == "") {
            var listItem = "<a>" + item.name + ", " + item.country + "</a>";
        }
        else {
            var listItem = "<a>" + item.name + ", " + item.state + ", " + item.country + "</a>";
        }

        return $("<li></li>")
            .append(listItem)
            .appendTo(ul);
    };
});