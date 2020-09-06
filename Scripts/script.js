(function ($) {
    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#blah').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]); // convert to base64 string
        }
    }
    $("#file").click(function () {
        $('#blah').attr('src', '/Profiles/dummy.jpg');
    });
    $("#file").change(function () {
        readURL(this);
    });
    //$(".countryInput").autocomplete({
    //    source: function (request, response) {
    //        $.ajax({
    //            url: "https://restcountries.eu/rest/v2/name/",
    //            dataType: "jsonp",
    //            data: {
    //                name: request.term
    //            },
    //            success: function (data) {
    //                response(data);
    //            }
    //        });
    //    },
    //    minLength: 2,
    //    select: function (event, ui) {
    //        log("Selected: " + ui.item.value + " aka " + ui.item.id);
    //    }
    //});
    $("body").on("keydown", ".countryInput", function () {
        var url = "https://restcountries.eu/rest/v2/name/" + $(".countryInput").val();
        $.ajax({
            url: url,
            dataType: "json",
            success: function (data) {
                $(".autocompletevalue").html = "";
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        item = data[i];
                        $(".autocompletevalue").append("<option class='loadContryCode' data-phonecode='" + item.callingCodes[0]+"' value = '" + item.name + "'>" + item.name + "</option>");
                    }
                    $(".autocompletevalue").removeClass("hidden");
                } else {
                    $(".autocompletevalue").addClass("hidden");
                }
            }
        });
    });

    $("body").on("change", ".autocompletevalue", function () {
        var name = $(".autocompletevalue :selected").val();
        var phonecode = $(".autocompletevalue :selected").attr("data-phonecode");
        $(".countryInput").val(name);
        $(".phonecode").text("+" + phonecode);
        $("#PhoneCode").val(phonecode);
        $(".autocompletevalue").addClass("hidden");
    });

}(jQuery))