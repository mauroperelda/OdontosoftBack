if (!Medve) var Medve = {};
if (!Medve.Common) {
    Medve.Common = {
        toDate: function (dateStr, formatStr) {
            if (!dateStr)
                return null;

            var date = new Date();
            date.setTime(dateStr.replace('/Date(', '').replace(')/', ''));

            if (!formatStr)
                return date;

            var day, month, year;

            switch (formatStr) {
                case 'dd/mm/yyyy':
                    day = date.getDate().toString().length > 1 ? date.getDate() : "0" + date.getDate();
                    month = (date.getMonth() + 1).toString().length > 1 ? (date.getMonth() + 1) : "0" + (date.getMonth() + 1);
                    year = date.getFullYear();
                default:
                    day = date.getDate().toString().length > 1 ? date.getDate() : "0" + date.getDate();
                    month = (date.getMonth() + 1).toString().length > 1 ? (date.getMonth() + 1) : "0" + (date.getMonth() + 1);
                    year = date.getFullYear();
            }
            
            return day + '/' + month + '/' + year;
        },
        stringToFunction: function (str) {
            var arr = str.split(".");
            var fn = (window || this);
            for (var i = 0, len = arr.length; i < len; i++) {
                fn = fn[arr[i]];
            }
            
            if (typeof fn !== "function") {
                throw new Error("function not found");
            }

            return new fn();
        },
        checkTimeOut: function (data) {
            var thereIsStillTime = true;

            if (data) {
                if (data.responseText) {
                    if ((data.responseText.indexOf("<title>Log On</title>") > -1) || (data.responseText.indexOf("<title>Object moved</title>") > -1) || (data.responseText === '"_Logon_"')) thereIsStillTime = false;
                } else {
                    if (data == "_Logon_") thereIsStillTime = false;
                }

                if (!thereIsStillTime) {
                    window.location.href = MedveApp.Url.CommonTimeoutRedirect;
                }
            } 

            return thereIsStillTime;
        },
        getObservableItemByCode: function (items, code) {
            for (var i = 0; i < items.length; i++) {
                if(items[i].Code() == code)
                    return items[i];
            }
            return null;
        },
        initializeDatePickers: function () {
            $('.medve-datepicker').datepicker({
                language: "es",
                format: "dd/mm/yyyy",
                autoclose: true
            });
        },
        createGuid: function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        },
        isValidEmailAddress: function(emailAddress) {
            var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
            return pattern.test(emailAddress);
        },
        isValidPhoneNumber: function (phoneNumber) {
            var pattern = new RegExp(/^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$/i);
            return pattern.test(phoneNumber);
        },
        getDateFromDatePicker: function (c) {
            var f = null;
            if ($("#" + c).val()) {
                debugger;
                f = $("#" + c).data().datepicker.getFormattedDate('yyyy-mm-dd');
            }
            return f;
        }
    };
}