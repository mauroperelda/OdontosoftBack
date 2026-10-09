$(document).ready(function () {


    $('*[data-codigo="ACEITETIPO"]').change(function () {
        var s = $(this).val();
        if (s == "SINTETICO" || s == "SEMI-SINTETICO") {
            $('*[data-codigo="KILOMETROSACEITE"]').val("10000");
        }
        else {
            if (s == "MINERAL") {
                $('*[data-codigo="KILOMETROSACEITE"]').val("8000");
            } else {
                $('*[data-codigo="KILOMETROSACEITE"]').val("");}
        }
        
    });
    

    $("#btnCalcularKilometros").click(function () {

        var eActual = $("#KilometrajeActual");
        var actual = (isNaN(eActual.val()) || $.trim(eActual.val()) == "") ? 0 : parseInt($.trim(eActual.val()));

        var aceite = 0;
        var eAceite = $('*[data-codigo="KILOMETROSACEITE"]');
        if (eAceite.val()) {
            aceite = (isNaN(eAceite.val()) || $.trim(eAceite.val()) == "") ? 0 : parseInt($.trim(eAceite.val()));
        }

        var proximoCambioKm = actual + aceite;
        $("#ProximaRevisionKilometraje").val(proximoCambioKm);

        var vehiculoId = $("#VehiculoId").val();

        var fechaEstimada = "";

        debugger;
        var conn = new Medve.Connector();

        conn.doRequest({
            url: MedveApp.Url.FechaEstimadaKilometros,
            type: "get",
            dataType: "json",
            data: {
                vehiculoId: vehiculoId,
                proximoCambioKm: proximoCambioKm
            },
            success: function (result) {
                
                
                $("#ProximaRevisionFecha").val(result.Data);
                

            },
            error: function (e) {
                ShowErrorMessage(e);
            }
            ,
            loader: function (e) {
                //if (e) {
                //    $("#user-loader").show();
                //}
                //else {
                //    $("#user-loader").hide();
                //}
            }
        });





        //(isNaN(eThis.val())) ? 0 : eThis.val()

        //if (actual == "NaN" || aceite == "NaN") {
        //    $("#ProximaRevisionKilometraje").val("");
        //}
        //else {
        //    $("#ProximaRevisionKilometraje").val(actual + aceite);
        //}
    });

    
    

    

});