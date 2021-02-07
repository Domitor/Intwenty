function raiseInformationModal(headertext, bodytext, close_callback) {
    $('#msg_dlg_modal_hdr').text(headertext);
    $('#msg_dlg_modal_text').text(bodytext);
    if (close_callback) {
        $('#msg_dlg_modal_closebtn').off('click', close_callback);
        $('#msg_dlg_modal_closebtn').off().on('click', close_callback);
    }
    $('#msg_dlg_modal').modal();

};


function raiseValidationErrorModal(message) {
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(message);
    $('#msg_dlg_modal').modal();

};

function raiseErrorModal(operationresult) {
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(operationresult.userError);
    $('#msg_dlg_modal').modal();

};

function raiseYesNoModal(headertxt, bodytext, yes_callback) {
    $('#yesno_dlg_modal_hdr').text(headertxt);
    $('#yesno_dlg_modal_text').text(bodytext);
    $('#yesno_dlg_modal_yesbtn').off('click', yes_callback);
    $('#yesno_dlg_modal_yesbtn').off().on('click', yes_callback);
    $('#yesno_dlg_modal').modal();

};

function hasRequiredValues(datalist, requiredlist) {

    for (var i = 0; i < datalist.length; i++) {
        for (var z = 0; z < requiredlist.length; z++) {
            var fld = requiredlist[z];
            if (!datalist[i][fld])
                return false;
            if (!datalist[i][fld] === "")
                return false;

        }
    }

    return true;

};


Array.prototype.where = function (filter) {

    var collection = this;

    switch (typeof filter) {

        case 'function':
            return $.grep(collection, filter);

        case 'object':
            for (var property in filter) {
                if (!filter.hasOwnProperty(property))
                    continue; // ignore inherited properties

                collection = $.grep(collection, function (item) {
                    return item[property] === filter[property];
                });
            }
            return collection.slice(0); // copy the array 
        // (in case of empty object filter)

        default:
            throw new TypeError('func must be either a' +
                'function or an object of properties and values to filter by');
    }
};


Array.prototype.firstOrDefault = function (func) {
    return this.where(func)[0] || null;
};


Vue.component("intwentyselect", {
    template: '<select><slot></slot></select>',
    mounted: function () {
        var vm = this;
        var element = $(this.$el);

        element.select2({ theme: "bootstrap", closeOnSelect: true }).on("select2:select", function ()
        {
            var selectionstring = "";
            var selectiontextstring = "";
            var selections = element.select2('data');

            //CANT BE ZERO IN THIS EVENT
            if (selections.length == 0)
                return;

            for (var i = 0; i < selections.length; i++) {
                if (selectionstring == "") {
                    selectionstring += selections[i].id;
                    selectiontextstring += selections[i].text;
                }
                else {
                    selectionstring += "," + selections[i].id;
                    selectiontextstring += "," + selections[i].text;
                }
            }

            vm.$emit('update:idfield', selectionstring);
            vm.$emit('update:textfield', selectiontextstring);

        }).on("select2:unselect", function () {
            var selectionstring = "";
            var selectiontextstring = "";
            var selections = element.select2('data');
            for (var i = 0; i < selections.length; i++) {
                if (selectionstring == "") {
                    selectionstring += selections[i].id;
                    selectiontextstring += selections[i].text;
                }
                else {
                    selectionstring += "," + selections[i].id;
                    selectiontextstring += "," + selections[i].text;
                }
            }

            vm.$emit('update:idfield', selectionstring);
            vm.$emit('update:textfield', selectiontextstring);
        });


    },
    updated: function () {

        $(this.$el).val(null).trigger("change");

        if (!this.$attrs.idfield)
            return;

        var arr = this.$attrs.idfield.split(",");
        $(this.$el).val(arr);
        $(this.$el).val(arr).trigger("change");
        $(this.$el).trigger("select2:select");

    },
    watch:
    {
        value: function (value) {
            var x = "";
        }

    },
    destroyed: function () {
        $(this.$el).off().select2("destroy");
    }
});



Vue.prototype.selectableProperties = function (item) {
    var context = this;

    if (!item.metaType)
        return [];

    if (!context.model.propertyCollection)
        return [];

    var result = [];
    for (var i = 0; i < context.model.propertyCollection.length; i++) {
        var isincluded = false;
        if (context.model.propertyCollection[i].validFor) {
            for (var z = 0; z < context.model.propertyCollection[i].validFor.length; z++) {

                if (item.metaType === context.model.propertyCollection[i].validFor[z])
                    isincluded = true;
            }
        }
        if (isincluded)
            result.push(context.model.propertyCollection[i]);
    }

    return result;
};


Vue.prototype.addProperty = function (modelitem) {

    if (!modelitem)
        return;

    if (!modelitem.currentProperty)
        return;

    if (!modelitem.propertyList)
        return;

    if (modelitem.currentProperty.isBoolType) {
        modelitem.currentProperty.codeValue = "TRUE";
        modelitem.currentProperty.displayValue = "True";
    }

    if (modelitem.currentProperty.isStringType || modelitem.currentProperty.isNumericType || modelitem.currentProperty.isListType)
        modelitem.currentProperty.displayValue = modelitem.currentProperty.codeValue;


    var t = modelitem.propertyList.firstOrDefault({ codeName: modelitem.currentProperty.codeName });
    if (t != null)
        return;

    if (!modelitem.currentProperty.codeValue)
        return;


    modelitem.propertyList.push({ codeName: modelitem.currentProperty.codeName, codeValue: modelitem.currentProperty.codeValue, displayValue: modelitem.currentProperty.displayValue });

    modelitem.currentProperty.codeValue = "";

    this.$forceUpdate();
};

Vue.prototype.deleteProperty = function (property, modelitem) {

    if (!property)
        return;

    if (!modelitem)
        return;

    if (!modelitem.propertyList)
        return;

    for (var i = 0; i < modelitem.propertyList.length; i++) {
        if (modelitem.propertyList[i].codeName === property.codeName) {
            modelitem.propertyList.splice(i, 1);
            break;
        }
    }
};

Vue.prototype.initializePropertyUI = function (modelitem) {
    if (!modelitem)
        return;

    modelitem.currentProperty = {};
    if (!modelitem.propertyList)
        modelitem.propertyList = [];

    if (!modelitem.hasOwnProperty("showSettings"))
        modelitem.showSettings = false;

    modelitem.showSettings = !modelitem.showSettings;

    this.$forceUpdate();

};


Vue.prototype.onFileUpload = function ()
{
   
};

Vue.prototype.uploadImage = function (event)
{
    var context = this;
    var endpoint = context.baseurl + 'UploadImage';
    var formData = new FormData();
    formData.append('File', event.srcElement.files[0]);
    formData.append('FileName', event.srcElement.files[0].name);
    formData.append('FileSize', event.srcElement.files[0].size);

    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function ()
    {
        if (xhr.readyState === 4) {
            var dbtable = event.srcElement.dataset.dbtable;
            var dbfield = event.srcElement.dataset.dbfield;

            var fileref = JSON.parse(xhr.response);
            context.model[dbtable][dbfield] = "/USERDOC/" + fileref.fileName;
            context.$forceUpdate();
        }
    }
    xhr.open('POST', endpoint, true);
    xhr.send(formData);
};

Vue.prototype.canSave = function () {
    var context = this;
    var result = true;
    $("[data-required]").each(function ()
    {
        var required = $(this).data('required');
        if (required === "True") {
            var validationfield = $(this).data('validationfield');
            var id = $(this).attr('id');
            var title = $(this).data('title');
            var metatype = $(this).data('metatype');
            var dbfield = $(this).data('dbfield');
            var dbtable = $(this).data('dbtable');

            if (!context.model[dbtable][dbfield]) {
                result = false;
                $(this).addClass('requiredNotValid');
                context.setValidationText(validationfield, title + " is required");
            }
            else if (context.model[dbtable][dbfield].length == 0) {
                result = false;
                $(this).addClass('requiredNotValid');
                context.setValidationText(validationfield, title + " is required");
            }
            else {
                if (metatype == "EMAILBOX") {
                    var check = isValidEmail(context.model[dbtable][dbfield]);
                    if (!check.result) {
                        result = false;
                        $(this).addClass('requiredNotValid');
                        context.setValidationText(validationfield, check.msg);
                    }
                }
                if (metatype == "PASSWORDBOX") {
                    var check = isValidPassword(context.model[dbtable][dbfield]);
                    if (!check.result) {
                        result = false;
                        $(this).addClass('requiredNotValid');
                        context.setValidationText(validationfield, check.msg);
                    }
                }

                if (result) {
                    context.clearValidationText(validationfield);
                    $(this).removeClass('requiredNotValid');
                }
            }
        }
    });


    return result;
};

Vue.prototype.setValidationText = function (validationfield, text)
{
    if (!this.validation)
        return;
    if (!validationfield)
        return;
    if (validationfield.length < 1)
        return;

    this.validation[validationfield] = text;
    this.$forceUpdate();
};

Vue.prototype.clearValidationText = function (validationfield) {
    if (!this.validation)
        return;
    if (!validationfield)
        return;
    if (validationfield.length < 1)
        return;

    this.validation[validationfield] = "";
    this.$forceUpdate();
};

Vue.prototype.onUserInput = function (event)
{
    if (!event)
        return;

    var elementId = event.srcElement.id;
    if (!elementId)
        return;

    //Remove requiredNotValid if the input is valid
    $("[data-required]").each(function () {
        var required = $(this).data('required');
        var id = $(this).attr('id');
        if (required === "True" && id === elementId) {
            var val = event.srcElement.value;
            if (val) {
                if (val.length > 0)
                    $("#" + elementId).removeClass('requiredNotValid');
            }
        }
    });
};

Vue.prototype.setSelectedDataViewValue = function (item, lookupid)
{
    var context = this;

    $("input[data-lookupid]").each(function () {
        var id = $(this).data('lookupid');
        if (id === lookupid) {
            var dbfield = $(this).data('dbfield');
            var viewfield = $(this).data('viewfield');
            context.current_edit_line[dbfield] = item[viewfield];
        }
    });

    context.$forceUpdate();
};

Vue.prototype.getDataViewValue = function (viewname, keyfield, lookupid)
{

    var context = this;

    context.pageInfo.filterValues = [];

    context.pageInfo.dataViewMetaCode = viewname;

    $("input[data-lookupid]").each(function () {
        var id = $(this).data('lookupid');
        if (id === lookupid) {
            var dbfield = $(this).data('dbfield');
            var viewfield = $(this).data('viewfield');
            if (dbfield === keyfield) {
                if (viewfield != '' && context.model[context.appMainTable][dbfield]) {
                    context.pageInfo.filterValues.push({ "name": viewfield, "value": context.model[context.appMainTable][dbfield] });
                }
            }
        }
    });


    var endpointurl = context.baseurl + "GetDataViewValue";

    $.ajax({
        url: endpointurl,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(context.pageInfo),
        success: function (response) {
            var dataviewitem = JSON.parse(response.data);
            $("input[data-lookupid]").each(function () {
                var id = $(this).data('lookupid');
                if (id === lookupid) {
                    var dbfield = $(this).data('dbfield');
                    var viewfield = $(this).data('viewfield');
                    if (dbfield != keyfield) {
                        context.model[context.appMainTable][dbfield] = dataviewitem[viewfield];
                        context.$forceUpdate();
                    }

                }
            });


        }
    });
};

Vue.prototype.openDataViewLookUp = function (viewname, line) {
    if (!viewname)
        return;

    this.showFilter = false;
    this.dlgFilterColumnName = "";
    this.dlgFilterValue = "";
    this.pageInfo.maxCount = 0;
    this.pageInfo.pageNumber = 0;
    this.pageInfo.dataViewMetaCode = viewname;
    this.getDataViewLookUpPage();
    this.current_edit_line = line;
    $("#" + viewname).modal();
};

Vue.prototype.nextDataViewLookUpPage = function () {
    var context = this;
    context.pageInfo.pageNumber++;
    context.getDataViewLookUpPage();
};

Vue.prototype.prevDataViewLookUpPage = function () {
    var context = this;
    context.pageInfo.pageNumber--;
    if (context.pageInfo.pageNumber < 0)
        context.pageInfo.pageNumber = 0;
    context.getDataViewLookUpPage();
};

Vue.prototype.getDataViewLookUpPage = function () {
    var context = this;

    context.pageInfo.filterValues = [];
    if (context.dlgFilterColumnName != '' && context.dlgFilterValue != '') {
        context.pageInfo.filterValues.push({ "name": context.dlgFilterColumnName, "value": context.dlgFilterValue });
    }

    var endpointurl = context.baseurl + "GetDataView";

    $.ajax({
        url: endpointurl,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(context.pageInfo),
        success: function (response) {
            //DATA
            context.dataview = JSON.parse(response.data);

            //UPDATE CURRENT PAGE INFO
            context.pageInfo = response.listFilter;
        }
    });
};

Vue.prototype.isFirstDataViewPage = function () {
    return (this.pageInfo.pageNumber <= 0);
};

Vue.prototype.isLastDataViewPage = function () {
    return (((this.pageInfo.pageNumber + 1) * this.pageInfo.pageSize) >= this.pageInfo.maxCount);
};

Vue.prototype.runDataViewFilter = function () {
    var context = this;
    context.pageInfo.pageNumber = 0;
    context.getDataViewLookUpPage();
};

Vue.prototype.addSubTableLine = function (tablename)
{
    var context = this;
    context.model[tablename].push({ Id: 0, ParentId: 0 });
    context.$forceUpdate();
};

Vue.prototype.downloadExcel = function () {

};

Vue.prototype.nextpage = function ()
{
    var context = this;
    context.pageInfo.pageNumber++;
    context.getPage();
};

Vue.prototype.prevpage = function () {
    var context = this;
    context.pageInfo.pageNumber--;
    if (context.pageInfo.pageNumber < 0)
        context.pageInfo.pageNumber = 0;
    context.getPage();
};


Vue.prototype.runFilter = function () {
    var context = this;
    if (context.pageInfo.filterValues.length > 0)
        context.getPage();
};

Vue.prototype.addFilterValue = function () {
    var context = this;
    context.pageInfo.filterValues.push({ "name": "", "value": "" });
};

Vue.prototype.deleteFilterValue = function (item) {
    var context = this;
    for (var i = 0; i < context.pageInfo.filterValues.length; i++)
    {
        if (context.pageInfo.filterValues[i].name === item.name)
        {
            context.pageInfo.filterValues.splice(i, 1);
            context.getPage();
            break;

        }
    }
};

