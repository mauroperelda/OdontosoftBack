var isSave = false;

jQuery(function($) {
    $('.btn-success').on('click', function () {
        isSave = true;
    });

    $(window).on('beforeunload', function (e) {
        if (isSave)
            isSave = false;
        else
            return '¿Desea cerrar esta página?';
    });
});
