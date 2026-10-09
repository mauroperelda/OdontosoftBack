if (!Medve) {
    var Medve = {};
}
; (function ($) {
    Medve.setAutonumerics = function () {
        Medve.setInputAutonumerics();
    }

    Medve.setInputAutonumerics = function () {
        $('.data-autonumeric-field, .data-autonumeric-field-disabled, .data-autonumeric-field-text, [data-autonumeric-field]').each(function () {
            var $this = $(this);
            var tagName = $this.prop("tagName");
            var $newInput = $("<" + tagName + ">");
			var settings = $this.data("autonumeric-settings");
            if (typeof settings == "string") {
                settings = $.parseJSON(settings);
            }
            $newInput.attr("id", $this.attr("id") + "_autonumeric");
            $newInput.attr("class", $this.attr("class"));
            $newInput.attr("style", $this.attr("style"));
            $newInput.attr("type", $this.attr("type"));
            $newInput.prop("disabled", $this.prop("disabled"));
            $this.hide();
            $this.before($newInput);
            if (typeof settings == "object") {
                $newInput.autoNumeric("init", settings);
            }
            else {
                $newInput.autoNumeric("init");
            }
            if ($this.val().replace(".", "").replace(",", ".") == "") {
                if (settings && settings.isNullable && settings.isNullable == false) {
					$newInput.autoNumeric("set", 0);
				}
            }
            else {
                $newInput.autoNumeric("set", parseFloat($this.val().replace(".", "")));
            }

            $newInput.on("blur focusout", function (e) {
                if ($this.val($newInput.autoNumeric("get").replace('.', ','))) {
                    $this.trigger("change");
                }
            });

            $newInput.on("keypress keyup", function () {
                $this.val($newInput.autoNumeric("get").replace('.', ','));
            });
        });
    }
})(jQuery);