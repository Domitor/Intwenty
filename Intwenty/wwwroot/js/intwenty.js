


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

    if (modelitem.currentProperty.isStringType || modelitem.currentProperty.isNumericType)
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



function getVueCreateUpdate(vueelement, applicationid, apptablename, baseurl) {

    var app = new Vue({
        el: vueelement,
        data: {
            dataview: [],
            valuedomains: {},
            model: { [apptablename]: {} },
            validation: {},
            pageInfo: { "applicationId": applicationid, "dataViewMetaCode": "", "maxCount": 0, "batchSize": 10, "currentRowNum": 0, "filterValues": [] },
            current_edit_line: {},
            showFilter: false,
            dlgFilterColumnName: "",
            dlgFilterValue: ""
        },
        methods:
        {
            saveApplication() {
                if (this.runSave)
                    this.runSave();
            },
            onFileUpload: function () {

            },
            onImageChanged: function (event) {
                var context = this;
                var endpoint = baseurl + 'UploadImage';
                var formData = new FormData();
                formData.append('File', event.srcElement.files[0]);
                formData.append('FileName', event.srcElement.files[0].name);
                formData.append('FileSize', event.srcElement.files[0].size);

                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        var dbtable = event.srcElement.dataset.dbtable;
                        var dbfield = event.srcElement.dataset.dbfield;

                        var fileref = JSON.parse(xhr.response);
                        context.model[dbtable][dbfield] = "/USERDOC/" + fileref.fileName;
                        context.$forceUpdate();
                        if (context.saveAfterUpload)
                            context.saveAfterUpload();
                    }
                }
                xhr.open('POST', endpoint, true);
                xhr.send(formData);
            },
            canSave: function () {
                var context = this;
                var result = true;
                $("[data-required]").each(function () {
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
            },
            setValidationText: function (validationfield, text) {
                if (!this.validation)
                    return;
                if (!validationfield)
                    return;
                if (validationfield.length < 1)
                    return;

                this.validation[validationfield] = text;
                this.$forceUpdate();
            },
            clearValidationText: function (validationfield) {
                if (!this.validation)
                    return;
                if (!validationfield)
                    return;
                if (validationfield.length < 1)
                    return;

                this.validation[validationfield] = "";
                this.$forceUpdate();
            },
            /*
            * Handles userinput
            *
            */
            onUserInput: function (event) {
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
            },
            setSelectedDataViewValue: function (item, lookupid) {
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
            },
            getDataViewValue: function (viewname, keyfield, lookupid) {

                var context = this;

                context.pageInfo.filterValues = [];

                context.pageInfo.dataViewMetaCode = viewname;

                $("input[data-lookupid]").each(function () {
                    var id = $(this).data('lookupid');
                    if (id === lookupid) {
                        var dbfield = $(this).data('dbfield');
                        var viewfield = $(this).data('viewfield');
                        if (dbfield === keyfield) {
                            if (viewfield != '' && context.model[apptablename][dbfield]) {
                                context.pageInfo.filterValues.push({ "columnName": viewfield, "value": context.model[apptablename][dbfield] });
                            }
                        }
                    }
                });


                var endpointurl = baseurl + "GetDataViewValue";

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
                                    context.model[apptablename][dbfield] = dataviewitem[viewfield];
                                    context.$forceUpdate();
                                }

                            }
                        });


                    }
                });
            },
            openDataViewLookUp: function (viewname, line) {
                if (!viewname)
                    return;

                this.showFilter = false;
                this.dlgFilterColumnName = "";
                this.dlgFilterValue = "";
                this.pageInfo.maxCount = 0;
                this.pageInfo.currentRowNum = 0;
                this.pageInfo.dataViewMetaCode = viewname;
                this.getDataViewLookUpPage();
                this.current_edit_line = line;
                $("#" + viewname).modal();
            },
            nextDataViewLookUpPage: function () {
                var context = this;
                context.pageInfo.currentRowNum += context.pageInfo.batchSize;
                context.getDataViewLookUpPage();
            },
            prevDataViewLookUpPage: function () {
                var context = this;
                context.pageInfo.currentRowNum -= context.pageInfo.batchSize;
                context.getDataViewLookUpPage();
            },
            getDataViewLookUpPage: function () {
                var context = this;

                context.pageInfo.filterValues = [];
                if (context.dlgFilterColumnName != '' && context.dlgFilterValue != '') {
                    context.pageInfo.filterValues.push({ "columnName": context.dlgFilterColumnName, "value": context.dlgFilterValue });
                }

                var endpointurl = baseurl + "GetDataView";

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
            },
            isFirstDataViewPage: function () {
                return this.pageInfo.currentRowNum <= 0;
            },
            isLastDataViewPage: function () {
                return this.pageInfo.currentRowNum >= this.pageInfo.maxCount;
            },
            runFilter: function () {
                var context = this;
                context.pageInfo.currentRowNum = 0;
                context.getDataViewLookUpPage();

            },
            addSubTableLine: function (tablename) {
                var context = this;
                context.model[tablename].push({ Id: 0, ParentId: 0 });
                context.$forceUpdate();
            },
        },
        mounted: function () {
            if (this.runMounted)
                this.runMounted();


        }
    });


    return app;

}



function getEditListView(vueelement, applicationid, baseurl, pagesize) {
    if (!pagesize)
        pagesize = 20;

    var app = new Vue({
        el: vueelement,
        data: {
            datalist: []
            , model: { "showFilter": false }
            , pageInfo: { "applicationId": applicationid, "maxCount": 0, "batchSize": pagesize, "currentRowNum": 0, "filterValues": [] }
            , currentSort: ''
            , currentSortDir: 'asc'
            , baseUrl: baseurl

        },
        methods: {
            downloadExcel: function () {

            },
            nextpage: function () {
                var context = this;
                context.pageInfo.currentRowNum += context.pageInfo.batchSize;
                context.getPage();
            },
            prevpage: function () {
                var context = this;
                context.pageInfo.currentRowNum -= context.pageInfo.batchSize;
                context.getPage();
            },
            getPage: function () {
                var context = this;
                var endpointurl = context.baseUrl + "GetEditListData";


                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(this.pageInfo),
                    success: function (response) {
                        //DATA
                        context.datalist = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.pageInfo = response.listFilter;
                        if (context.pageInfo.filterValues.length === 0)
                            context.addFilterValue();
                    }
                });
            },
            runFilter: function () {
                var context = this;
                if (context.pageInfo.filterValues.length > 0)
                    context.getPage();
            },
            addFilterValue: function () {
                var context = this;
                context.pageInfo.filterValues.push({ "columnName": "", "value": "" });
            },
            deleteFilterValue: function (item) {
                var context = this;

                for (var i = 0; i < context.pageInfo.filterValues.length; i++) {
                    if (context.pageInfo.filterValues[i].columnName === item.columnName) {
                        context.pageInfo.filterValues.splice(i, 1);
                        break;
                    }
                }
            },
            sortBycolumn: function (s) {
                //if s == current sort, reverse
                if (s === this.currentSort) {
                    this.currentSortDir = this.currentSortDir === 'asc' ? 'desc' : 'asc';
                }
                this.currentSort = s;
            },
            exportToExcel: function () {
                var args = { "applicationId": applicationid, "maxCount": 0, "batchSize": 2000, "currentRowNum": 0, "filterValues": [] }
                var context = this;
                var endpointurl = context.baseUrl + "GetEditListData";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(args),
                    success: function (response) {
                        var data = JSON.parse(response.data);
                        alasql.promise('SELECT * INTO XLSX("download.xlsx",{headers:true}) FROM ?', [data])
                            .then(function (data) {
                                console.log('Data saved');
                            }).catch(function (err) {
                                console.log('Error:', err);
                            });
                    }
                });
            },
            deleteApplication: function (item) {
                var context = this;
                var endpointurl = context.baseUrl + "Delete";

                var deleteapp = function () {
                    $.ajax({
                        url: endpointurl,
                        type: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(item),
                        success: function (response) {

                            if (response.isSuccess) {
                                window.location.reload(true);
                            }
                            else {
                                raiseErrorModal(response);
                            }
                        }
                    });
                };

                raiseYesNoModal("Delete ?", "This record will be deleted, are you sure ?", deleteapp);
            }
        },
        computed: {

            sortedResults: function () {
                return this.datalist.sort((a, b) => {
                    let modifier = 1;
                    if (this.currentSortDir === 'desc') modifier = -1;
                    if (a[this.currentSort] < b[this.currentSort]) return -1 * modifier;
                    if (a[this.currentSort] > b[this.currentSort]) return 1 * modifier;
                    return 0;
                });
            },
            isFirstPage: function () {
                return this.pageInfo.currentRowNum <= 0;
            },
            isLastPage: function () {
                return this.pageInfo.currentRowNum >= (this.pageInfo.maxCount - 1);
            }
        },
        mounted: function () {
            var context = this;
            context.getPage();
        }
    });

    return app;
};

function getListView(vueelement, applicationid, baseurl, pagesize) {
    if (!pagesize)
        pagesize = 20;

    var app = new Vue({
        el: vueelement,
        data: {
            datalist: []
            , model: {}
            , pageInfo: { "applicationId": applicationid, "maxCount": 0, "dataViewMetaCode": "", "batchSize": pagesize, "currentRowNum": 0, "filterValues": [] }
            , currentSort: ''
            , currentSortDir: 'asc'
            , baseUrl: baseurl

        },
        methods:
        {
            nextpage: function () {
                var context = this;
                context.pageInfo.currentRowNum += context.pageInfo.batchSize;
                context.getPage();
            },
            prevpage: function () {
                var context = this;
                context.pageInfo.currentRowNum -= context.pageInfo.batchSize;
                context.getPage();
            },
            getPage: function () {
                var context = this;
                var endpointurl = context.baseUrl + "GetListData";


                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(this.pageInfo),
                    success: function (response) {
                        //DATA
                        context.datalist = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.pageInfo = response.listFilter;
                    }
                });
            },
            handleFilterValue: function () {

            },
            sortBycolumn: function (s) {
                //if s == current sort, reverse
                if (s === this.currentSort) {
                    this.currentSortDir = this.currentSortDir === 'asc' ? 'desc' : 'asc';
                }
                this.currentSort = s;
            }
        },
        computed: {

            sortedResults: function () {
                return this.datalist.sort((a, b) => {
                    let modifier = 1;
                    if (this.currentSortDir === 'desc') modifier = -1;
                    if (a[this.currentSort] < b[this.currentSort]) return -1 * modifier;
                    if (a[this.currentSort] > b[this.currentSort]) return 1 * modifier;
                    return 0;
                });
            },
            isFirstPage: function () {
                return this.pageInfo.currentRowNum <= 0;
            },
            isLastPage: function () {
                return this.pageInfo.currentRowNum >= this.pageInfo.maxCount;
            }
        },
        mounted: function () {
            var context = this;
            context.getPage();
        }
    });

    return app;
};