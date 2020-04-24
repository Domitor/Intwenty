﻿'use strict';

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function raiseValidationErrorModal(message) {
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(message);
    $('#msg_dlg_modal').modal();
}

function raiseErrorModal(operationresult) {
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(operationresult.userError);
    $('#msg_dlg_modal').modal();
}

function raiseYesNoModal(headertxt, bodytext, yes_callback) {
    $('#yesno_dlg_modal_hdr').text(headertxt);
    $('#yesno_dlg_modal_text').text(bodytext);
    $('#yesno_dlg_modal_yesbtn').click(yes_callback);
    $('#yesno_dlg_modal').modal();
}

function hasRequiredValues(datalist, requiredlist) {

    for (var i = 0; i < datalist.length; i++) {
        for (var z = 0; z < requiredlist.length; z++) {
            var fld = requiredlist[z];
            if (!datalist[i][fld]) return false;
            if (!datalist[i][fld] === "") return false;
        }
    }

    return true;
}

function getVueCreateUpdate(vueelement, applicationid, apptablename, baseurl) {

    var app = new Vue({
        el: vueelement,
        data: {
            dataview: [],
            valuedomains: {},
            model: _defineProperty({}, apptablename, {}),
            validation: {},
            viewretrieveinfo: { "applicationId": applicationid, "dataViewMetaCode": "", "maxCount": 0, "batchSize": 10, "currentRowNum": 0, "filterField": "", "filterValue": "" },
            current_edit_line: {}
        },
        methods: {
            saveApplication: function saveApplication() {
                this.runSave();
            },
            onFileUpload: function onFileUpload() {},
            onImageChanged: function onImageChanged(event) {
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
                        var s = "";
                    }
                };
                xhr.open('POST', endpoint, true);
                xhr.send(formData);
            },
            canSave: function canSave() {
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
                        } else if (context.model[dbtable][dbfield].length == 0) {
                            result = false;
                            $(this).addClass('requiredNotValid');
                            context.setValidationText(validationfield, title + " is required");
                        } else {
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
            setValidationText: function setValidationText(validationfield, text) {
                if (!this.validation) return;
                if (!validationfield) return;
                if (validationfield.length < 1) return;

                this.validation[validationfield] = text;
                this.$forceUpdate();
            },
            clearValidationText: function clearValidationText(validationfield) {
                if (!this.validation) return;
                if (!validationfield) return;
                if (validationfield.length < 1) return;

                this.validation[validationfield] = "";
                this.$forceUpdate();
            },
            /*
            * Handles userinput
            *
            */
            onUserInput: function onUserInput(event) {
                if (!event) return;

                var elementId = event.srcElement.id;
                if (!elementId) return;

                //Remove requiredNotValid if the input is valid
                $("[data-required]").each(function () {
                    var required = $(this).data('required');
                    var id = $(this).attr('id');
                    if (required === "True" && id === elementId) {
                        var val = event.srcElement.value;
                        if (val) {
                            if (val.length > 0) $("#" + elementId).removeClass('requiredNotValid');
                        }
                    }
                });
            },
            setSelectedDataViewValue: function setSelectedDataViewValue(item, lookupid) {
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
            getDataViewValue: function getDataViewValue(viewname, keyfield, lookupid) {

                var context = this;
                context.viewretrieveinfo.dataViewMetaCode = viewname;

                $("input[data-lookupid]").each(function () {
                    var id = $(this).data('lookupid');
                    if (id === lookupid) {
                        var dbfield = $(this).data('dbfield');
                        var viewfield = $(this).data('viewfield');
                        if (dbfield === keyfield) {
                            context.viewretrieveinfo.filterField = viewfield;
                            context.viewretrieveinfo.filterValue = context.model[apptablename][dbfield];
                        }
                    }
                });

                if (!context.viewretrieveinfo.filterValue) return;
                if (context.viewretrieveinfo.filterValue.length == 0) return;

                var endpointurl = baseurl + "GetDataViewValue";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(context.viewretrieveinfo),
                    success: function success(response) {
                        var dataviewitem = JSON.parse(response.data);
                        $("input[data-lookupid]").each(function () {
                            var id = $(this).data('lookupid');
                            if (id === lookupid) {
                                var dbfield = $(this).data('dbfield');
                                var viewfield = $(this).data('viewfield');
                                context.model[apptablename][dbfield] = dataviewitem[viewfield];
                                context.$forceUpdate();
                            }
                        });
                    }
                });
            },
            openDataViewLookUp: function openDataViewLookUp(viewname, line) {
                if (!viewname) return;

                this.viewretrieveinfo.maxCount = 0;
                this.viewretrieveinfo.currentRowNum = 0;
                this.viewretrieveinfo.filterField = "";
                this.viewretrieveinfo.filterValue = "";
                this.viewretrieveinfo.dataViewMetaCode = viewname;
                this.getDataViewLookUpPage();
                this.current_edit_line = line;
                $("#" + viewname).modal();
            },
            nextDataViewLookUpPage: function nextDataViewLookUpPage() {
                var context = this;
                context.viewretrieveinfo.currentRowNum += context.viewretrieveinfo.batchSize;
                context.getDataViewLookUpPage();
            },
            prevDataViewLookUpPage: function prevDataViewLookUpPage() {
                var context = this;
                context.viewretrieveinfo.currentRowNum -= context.viewretrieveinfo.batchSize;
                context.getDataViewLookUpPage();
            },
            getDataViewLookUpPage: function getDataViewLookUpPage() {
                var context = this;

                var endpointurl = baseurl + "GetDataView";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(context.viewretrieveinfo),
                    success: function success(response) {
                        //DATA
                        context.dataview = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.viewretrieveinfo = response.retriveListArgs;
                    }
                });
            },
            isFirstDataViewPage: function isFirstDataViewPage() {
                return this.listRetrieveInfo.currentRowNum <= 0;
            },
            handleDataViewFilter: function handleDataViewFilter() {
                var context = this;
                context.viewretrieveinfo.currentRowNum = 0;
                context.getDataViewLookUpPage();
            },
            addSubTableLine: function addSubTableLine(tablename) {
                var context = this;
                context.model[tablename].push({ Id: 0, ParentId: 0 });
                context.$forceUpdate();
            }
        },
        mounted: function mounted() {
            this.runMounted();
        }
    });

    return app;
}

function getVueListView(vueelement, applicationid, baseurl) {

    var app = new Vue({
        el: vueelement,
        data: {
            datalist: [],
            model: { "filtervalue": "", "filterfield": "" },
            listRetrieveInfo: { "applicationId": applicationid, "maxCount": 0, "dataViewMetaCode": "", "listViewMetaCode": "", "batchSize": 20, "currentRowNum": 0, "filterField": "", "filterValue": "" },
            currentSort: '',
            currentSortDir: 'asc',
            baseUrl: baseurl

        },
        methods: {
            downloadExcel: function downloadExcel() {
                var context = this;
                alert('test');
            },
            nextpage: function nextpage() {
                var context = this;
                context.listRetrieveInfo.currentRowNum += context.listRetrieveInfo.batchSize;
                context.getPage();
            },
            prevpage: function prevpage() {
                var context = this;
                context.listRetrieveInfo.currentRowNum -= context.listRetrieveInfo.batchSize;
                context.getPage();
            },
            getPage: function getPage() {
                var context = this;
                var endpointurl = context.baseUrl + "GetListView";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(this.listRetrieveInfo),
                    success: function success(response) {
                        //DATA
                        context.datalist = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.listRetrieveInfo = response.retriveListArgs;
                    }
                });
            },
            handleFilterValue: function handleFilterValue() {
                var context = this;

                if (!context.model.filterfield || context.model.filterfield == "") return;

                if (context.model.filtervalue == "") return;

                if (context.model.filtervalue != context.listRetrieveInfo.filterValue) {
                    context.listRetrieveInfo.currentRowNum = 0;
                    context.listRetrieveInfo.filterField = context.model.filterfield;
                    context.listRetrieveInfo.filterValue = context.model.filtervalue;
                    context.getPage();
                }
            },
            sortBycolumn: function sortBycolumn(s) {
                //if s == current sort, reverse
                if (s === this.currentSort) {
                    this.currentSortDir = this.currentSortDir === 'asc' ? 'desc' : 'asc';
                }
                this.currentSort = s;
            },
            exportToExcel: function exportToExcel() {
                var args = { "applicationId": applicationid, "maxCount": 0, "dataViewMetaCode": "", "listViewMetaCode": "", "batchSize": 2000, "currentRowNum": 0, "filterField": "", "filterValue": "" };
                var context = this;
                var endpointurl = context.baseUrl + "GetListView";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(args),
                    success: function success(response) {
                        var data = JSON.parse(response.data);
                        alasql.promise('SELECT * INTO XLSX("download.xlsx",{headers:true}) FROM ?', [data]).then(function (data) {
                            console.log('Data saved');
                        })['catch'](function (err) {
                            console.log('Error:', err);
                        });
                    }
                });
            }
        },
        computed: {

            sortedResults: function sortedResults() {
                var _this = this;

                return this.datalist.sort(function (a, b) {
                    var modifier = 1;
                    if (_this.currentSortDir === 'desc') modifier = -1;
                    if (a[_this.currentSort] < b[_this.currentSort]) return -1 * modifier;
                    if (a[_this.currentSort] > b[_this.currentSort]) return 1 * modifier;
                    return 0;
                });
            },
            isFirstPage: function isFirstPage() {
                return this.listRetrieveInfo.currentRowNum <= 0;
            }
        },
        mounted: function mounted() {
            var context = this;
            context.getPage();
        }
    });

    return app;
}
