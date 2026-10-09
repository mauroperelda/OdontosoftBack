if (!Medve) var Medve = {};
if (!Medve.Bootbox) {
    Medve.Bootbox = {
        alertSuccess: function (title, message, okText, okCallback) {
            if (typeof title !== "string" && typeof title !== "number") { title = "Success"; }
            if (typeof message !== "string" && typeof title !== "number") { message = "Your action has been successfully completed"; }
            if (typeof okText !== "string" && typeof title !== "number") { okText = "OK"; }
            bootbox.dialog({
                title: title,
                message: message,
                size: "small",
                className: "modal-success",
                buttons: {
                    success: {
                        label: okText,
                        className: "btn btn-outline",
                        callback: function () {
                            if (okCallback && typeof okCallback === "function") {
                                okCallback();
                            }
                        }
                    },
                }
            });
        },
        alertDanger: function (title, message, okText, okCallback) {
            if (typeof title !== "string" && typeof title !== "number") { title = "Error"; }
            if (typeof message !== "string" && typeof message !== "number") { message = "An error has ocurred"; }
            if (typeof okText !== "string" && typeof okText !== "number") { okText = "OK"; }
            bootbox.dialog({
                title: title,
                message: message,
                size: "small",
                className: "modal-danger",
                buttons: {
                    success: {
                        label: okText,
                        className: "btn btn-outline",
                        callback: function() {
                            if (okCallback && typeof okCallback === "function") {
                                okCallback();
                            }
                        }
                    },
                }
            });
        },
        alertInfo: function (title, message, okText, okCallback) {
            if (typeof title !== "string" && typeof title !== "number") { title = "Info"; }
            if (typeof message !== "string" && typeof message !== "number") { message = "This is an information modal about nothing"; }
            if (typeof okText !== "string" && typeof okText !== "number") { okText = "OK"; }
            bootbox.dialog({
                title: title,
                message: message,
                size: "small",
                className: "modal-info",
                buttons: {
                    success: {
                        label: okText,
                        className: "btn btn-outline",
                        callback: function() {
                            if (okCallback && typeof okCallback === "function") {
                                okCallback();
                            }
                        }
                    },
                }
            });
        },
        confirm: function (title, message, okText, noText, okCallback, noCallback) {
            if (typeof title !== "string" && typeof title !== "number") { title = "Are you sure?"; }
            if (typeof message !== "string" && typeof message !== "number") { message = "Are you sure you wanna continue?"; }
            if (typeof okText !== "string" && typeof okText !== "number") { okText = "OK"; }
            if (typeof noText !== "string" && typeof noText !== "number") { noText = "No"; }

            bootbox.dialog({
                title: title,
                message: message,
                size: "small",
                className: "modal-warning",
                buttons: {
                    ok: {
                        label: okText,
                        className: "btn btn-outline btn-default",
                        callback: function () {
                            if (okCallback && typeof okCallback === "function") {
                                okCallback();
                            }
                        }
                    },
                    no: {
                        label: noText,
                        className: "btn btn-outline",
                        callback: function () {
                            if (noCallback && typeof noCallback === "function") {
                                noCallback();
                            }
                        }
                    }
                }
            });
        },
        customDialog: function (title, message, size, className, buttons) {
            if (typeof title !== "string" && typeof title !== "number") { title = "Title"; }
            if (typeof message !== "string" && typeof message !== "number") { message = "Message "; }
            if (typeof size !== "string" && size != "small" && size != "large") { size = "large"; }
            if (typeof className !== "string") { className = ""; }
            if (typeof buttons !== "object") { buttons = { main: { label: "OK", className: "btn btn-outline", callback: function () { } } }; }

            bootbox.dialog({
                title: title,
                message: message,
                size: size,
                className: className,
                buttons: buttons
            });
        }
    }
}