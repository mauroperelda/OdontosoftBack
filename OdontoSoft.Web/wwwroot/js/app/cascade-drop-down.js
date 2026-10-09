function cascadeEstablecimientos(dropdownId, cuit, emptyItemText) {
    basicCascade(dropdownId, "cuitPrestador", cuit, MedveApp.Url.GetEstablecimientosByCuit, true, emptyItemText);    
}

function cascadeMarcaModelo(dropdownId, marcaId, emptyItem) {
    basicCascade(dropdownId, "marcaId", marcaId, MedveApp.Url.GetModelosByMarca, emptyItem, "");
}

function basicCascade(dropdownId, parameterName, parameterValue, urlAction, emptyItem, emptyItemText) {
    debugger;
    dropdown = $("#" + dropdownId);
    dropdown.empty();

    if (emptyItem) {
        addDropDownItem(dropdown, "", emptyItemText);
    }

    if (parameterValue == undefined || parameterValue == 0) return;

    var myData = {};
    myData[parameterName] = parameterValue;

    var conn = new Medve.Connector();
    conn.doRequest({
        url: urlAction,
        type: "get",
        dataType: "json",
        data: myData,
        success: function (result) {
            $(result.Data).each(function () {
                addDropDownItem(dropdown, this.Id, this.GetString);
            });
        },
        error: function (e) {
            var errorMessage = "Ha ocurrido un error al contactarse con el servidor";
            if (e.Head && e.Head.Msg) {
                errorMessage = e.Head.Msg;
            }
            if (typeof e == "string" && e != "") {
                errorMessage = e;
            }
            Medve.Bootbox.alertDanger("Error", errorMessage, "Cerrar");
            //var errorMessage = "Ha ocurrido un error de conexion";
            //alert(errorMessage);
        }
    });

}


function addDropDownItem(dropdown, value, text) {
    $("<option />", {
        val: value,
        text: text
    }).appendTo(dropdown);
}