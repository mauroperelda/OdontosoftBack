    $(document).ready(function () {


        $(".select2Reference").select2({
            minimumInputLength: 3,
            placeholder: "",
            allowClear: true,
            language: "es",
            ajax: {
                url: MedveApp.Url.GetReferenciasByCodigo,
                dataType: 'json',
                type: "GET",
                quietMillis: 50,
                data: function (params) {
                    return {
                        codigoReferencia: $(this).data("codigo"),
                        searchText: params.term
                    };
                },
                processResults: function (data) {
                    debugger;
                    var r = $.map(data.Data.Data, function (item) {
                        return {
                            id: item.Valor,
                            text: item.Valor,
                            Valor: item.Valor,
                            ValorAlternativo: item.ValorAlternativo,
                        }
                    });
                    r.splice(0, 0,
                        {
                            id: 0,
                            text: "",
                            Valor: "",
                            ValorAlternativo: "",
                        });

                    return { results: r };
                }
            },
            templateResult: formatRepo1,
            templateSelection: formatRepoSelection

        });



    function formatRepo1 (repo) {
          if (repo.loading) {
            return repo.text;
          }

    var $container = $(
    "<div class='select2-result-repository clearfix'>" +
        "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'></div>" +
            "<div class='select2-result-repository__description'></div>" +
            "</div>" +
        "</div>"
    );

    $container.find(".select2-result-repository__title").text(repo.Valor);
    $container.find(".select2-result-repository__description").text(repo.ValorAlternativo);

    return $container;
        }

    function formatRepoSelection (repo) {
          return repo.Valor || repo.text;
        }


    $('.select2Reference').each(function(){
            var studentSelect = $(this);
    if (studentSelect.data("default-value") != "") {

                var option = new Option(studentSelect.data("default-value"), studentSelect.data("default-value"), true, true);
    studentSelect.append(option).trigger('change');
    studentSelect.trigger({
        type: 'select2:select',
    params: {
        data: studentSelect.data("default-value")
                    }
                });

                //$.ajax({
        //    url: MedveApp.Url.GetReferenciasByCodigoAndValue,
        //    dataType: 'json',
        //    type: "GET",
        //    data: 
        //    {
        //        codigoReferencia: studentSelect.data("codigo"),
        //        searchText: studentSelect.data("default-value")
        //    },
        //}).then(function (data) {            
        //    var option = new Option(data.Data.Data[0].Valor, data.Data.Data[0].Valor, true, true);
        //    studentSelect.append(option).trigger('change');
        //    studentSelect.trigger({
        //        type: 'select2:select',
        //        params: {
        //            data: data.Data.Data[0].Valor
        //        }
        //    });
        //});


    }

        });
        


    });

