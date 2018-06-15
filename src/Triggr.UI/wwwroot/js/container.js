//probeJson -raw
//probe -activation

$(".probe").click(function () {
    try {

        var url = $(this).attr('href');
        axios.get(url).then(function (response) {
            var data = response.data;
            if (data !== undefined && data !== "")
                swal("Probe is activated", "", "success")
            else
                swal("Something went wrong", "", "error");
        }).catch(function () {
            swal("Something went wrong", "", "error");
        })
    }
    catch (ex) {
        console.log(ex)
    }
    return false;
});


$(".probeJson").click(function () {
    try {

        var url = $(this).attr('href');
        axios.get(url).then(function (response) {
            var data = response.data;
            if (data !== undefined && data !== -1) {
                var slider = document.createElement("pre");
                slider.innerText = data;
                slider.style = "text-align:left !important;"

                swal({
                    content: slider,
                });
            }
            else
                swal("Something went wrong", "", "error");
        }).catch(function () {
            swal("Something went wrong", "", "error");
        })
    }
    catch (ex) {
        console.log(ex)
    }
    return false;
});