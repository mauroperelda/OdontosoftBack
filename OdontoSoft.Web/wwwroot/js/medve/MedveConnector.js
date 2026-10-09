/*Singleton. Depends on jQuery
Usage: 
new Medve.Connector().doRequest(opts);
opts
string url (mandatory)
object data (mandatory)
string dataType (default json)
string type (default get)
function success(Data): is executed when response is OK. 
Recives de Data section of the message
function error(msg): is executed when response is OK. 
Recives the Message of the error.
function loader(working): informs the state of the ajax
Version 20120801
*/
if (!Medve) var Medve = {};
if (!Medve.Connector) {
    Medve.Connector = function MedveConnector() {
        var $ = jQuery, self = this;
        $.extend(self, Backbone.Events, {
            _def: {
                success: function () { },
                error: function (resp) {
                    //if (Medve.Dialog)
                    //	Medve.Dialog.showError(msg);
                    //else
                    alert(resp.Head.Msg);
                },
                type: "get",
                dataType: "json",
                async: true,
                loader: function () { }
            },
            lastRequest: null,
            _prepareData: function (data) {
                if (!data) {
                    data = { json: true, ts: (new Date()).getTime() }
                } else if ($.isPlainObject(data)) {
                    data.json = true;
                    data.ts = (new Date()).getTime();
                } else if ($.isArray(data)) {
                    data.push({ name: "json", value: true, name: "ts", value: (new Date()).getTime() });
                } else if (typeof (data) == 'string') {
                    if (data.length > 0) data += "&";
                    data += "json=true&ts=" + (new Date()).getTime();
                }
                return data;
            },
            doRequest: function (opts) {
                opts = $.extend({}, this._def, opts);
                if (self.lastRequest)
                    self.lastRequest.abort();
                opts.loader(true);
                //console.log("dataType", opts.dataType);
                self.lastRequest = $.ajax({
                    url: opts.url,
                    data: self._prepareData(opts.data),
                    dataType: opts.dataType,
                    traditional: true,
                    type: opts.type,
                    async: opts.async,
                    success: function (resp) {
                        if (Medve.Common.checkTimeOut(resp))
                        {
                            opts.loader(false);
                            if (opts.dataType != "json") {
                                opts.success(resp);
                            } else {
                                if (resp.Head.Ok) {
                                    opts.success(resp.Data, resp);
                                } else if (resp.Control.Action && resp.Control.Action != "") {
                                    self.trigger(resp.Control.Action, resp.Head.Msg, resp.Control);
                                } else {
                                    opts.error(resp);
                                }
                            }
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (errorThrown != "abort") {
                            opts.loader(false);
                            opts.error(errorThrown);
                        }
                    }
                });
                return self.lastRequest;
            }
        });


        self.on("redirect", function (Msg, Control) {
            window.location.href = Control.Parameter;
        });
    };
}