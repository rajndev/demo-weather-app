$(function () {
    $("#searchterm").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/GetAutocompleteList",
                data: { "cityname": request.term },
                type: "POST",
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            CompanyName: item.name,
                            Industry: item.id,
                            json: item
                        }
                    }))
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        },
        focus: function (event, ui) {
            $('#searchterm').val(ui.item.CompanyName);
            return false;
        },
        select: function (event, ui) {
            $('#searchterm').val(ui.item.CompanyName);
            return false;
        },
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .append("<a>" + item.CompanyName + "," + item.Industry + "</a>")
            .appendTo(ul);
    };
});




                                                    //$(function () {
                                                    //    $("#searchterm").autocomplete({
                                                    //        source: function (request, response) {
                                                    //            $.ajax({
                                                    //                url: '/Home/GetAutocompleteList',
                                                    //                data: { "cityname": request.term },
                                                    //                type: "POST",
                                                    //                success: function (data) {
                                                    //                    response($.map(data, function (item) {
                                                    //                        return { label: item, value: item }
                                                    //                    }))
                                                    //                },
                                                    //                error: function (response) {
                                                    //                    alert(response.responseText);
                                                    //                },
                                                    //                failure: function (response) {
                                                    //                    alert(response.responseText);
                                                    //                }
                                                    //            });
                                                    //        },

                                                    //        minLength: 3
                                                    //    });
                                                    //});