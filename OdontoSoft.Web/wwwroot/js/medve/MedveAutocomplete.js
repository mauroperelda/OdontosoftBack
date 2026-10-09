if (!Medve) var Medve = {};
if (!Medve.Autocomplete) {
    Medve.Autocomplete = {
        register: function () {
            $.widget('custom.MedveAutocomplete', $.ui.autocomplete, {
                _create: function () {
                    this._super();
                    this.widget().menu("option", "items", "> :not(.ui-widget-header)");
                },
                _renderMenu: function (ul, items) {
                    var self = this, thead;
                    //if (this.options.showHeader) {
                    //    table = $('<div class="ui-widget-header" style="width:' + this.element[0].clientWidth + 'px;"></div>');
                    //    // Column headers
                    //    if (this.options.columns != undefined) {
                    //        $.each(this.options.columns, function (index, column) {
                    //            var minWidth = column.style && column.style.minWidth && typeof column.style.minWidth === "string" ? column.style.minWidth : "50px";
                    //            //var fontSize = column.style && column.style.fontSize && typeof column.style.fontSize === "string" ? column.style.fontSize : "12px";
                    //            table.append('<span style="float:left;min-width:' + minWidth + ';font-size:12px;">' + column.name + '</span>');
                    //        });
                    //    }
                    //    else {
                    //        if (items.length > 0) {
                    //            $.each(items[0], function (index, property) {
                    //                table.append('<span style="float:left;min-width:50px;font-size:12px;">' + property + '</span>');
                    //            });
                    //        }
                    //    }
                    //    table.append('<div style="clear: both;"></div>');
                    //    ul.append(table);
                    //}

                    ul.css("max-height", "600px");
                    ul.css("overflow-y", "auto");
                    if (items && items.length > 0) {
                        // List items
                        $.each(items, function (index, item) {
                            self._renderItem(ul, item);
                        });
                    }
                    if (this.options.striped) {
                        //ul.addClass("medve-autocomplete-striped");
                        var firstStrip = this.options.striped.first ? this.options.striped.first : "#dfdfdf";
                        var secondStrip = this.options.striped.first ? this.options.striped.first : "#fff";
                        $(ul).find("li:nth-child(odd)").css("background-color", firstStrip);
                        $(ul).find("li:nth-child(even)").css("background-color", secondStrip);
                    }
                },
                _resizeMenu: function () {
                    this.menu.element.outerWidth(this.element[0].clientWidth);
                },
                _renderItem: function (ul, item) {
                    var t = '',
                        result = '';
                    if (this.options.columns != undefined) {
                        $.each(this.options.columns, function (index, column) {
                            var minWidth = column.style && column.style.minWidth && typeof column.style.minWidth === "string" ? column.style.minWidth : "50px";
                            var fontSize = column.style && column.style.fontSize && typeof column.style.fontSize === "string" ? column.style.fontSize : "12px";
                            var fontWeight = column.style && column.style.fontWeight && (typeof column.style.fontWeight === "string" || typeof column.style.fontWeight === "number") ? column.style.fontWeight.toString() : "normal";
                            t += '<span style="min-width:' + minWidth + ';font-size:' + fontSize + ';font-weight:' + fontWeight + ';display:block;">' + item[column.valueField ? column.valueField : index] + '</span>';
                        });
                    }
                    else {
                        $.each(item, function (index, property) {
                            t += '<span style="min-width:50px;font-weight:normal;display:block;">' + item[property] + '</span>';
                        });
                    }

                    result = $('<li style="width:100%;"></li>')
                        .data('ui-autocomplete-item', item)
                        .append('<a style="width:100%;">' + t + '<div style="clear: both;"></a>')
                        .appendTo(ul);
                    return result;
                }
            });
        }
    }
}