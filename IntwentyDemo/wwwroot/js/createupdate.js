
function createVue(vueelement, applicationid, apptablename, baseurl)
{

    var app = new Vue({
        el: vueelement,
        data: {
            dataview: [],
            valuedomains: {},
            model: { [apptablename]: {} },
            validation: {},
            viewretrieveinfo: { "applicationId": applicationid, "dataViewMetaCode": "", "maxCount": 0, "batchSize": 10, "currentRowNum": 0, "filterField": "", "filterValue": "" },
            current_edit_line: {}
        },
        methods:
        {
            saveApplication() {
                this.runSave();
            },
            onFileUpload: function ()
            {
               
            },
            onImageChanged: function (event)
            {
                var context = this;
                var endpoint = baseurl + 'UploadImage';
                var formData = new FormData();
                formData.append('File', event.srcElement.files[0]);
                formData.append('FileName', event.srcElement.files[0].name);
                formData.append('FileSize', event.srcElement.files[0].size);

                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4)
                    {
                        var dbtable = event.srcElement.dataset.dbtable;
                        var dbfield = event.srcElement.dataset.dbfield;

                        var fileref = JSON.parse(xhr.response);
                        context.model[dbtable][dbfield] = "/USERDOC/" + fileref.fileName;
                        context.$forceUpdate();
                        var s = "";
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
            setValidationText: function (validationfield, text)
            {
                if (!this.validation)
                    return;
                if (!validationfield)
                    return;
                if (validationfield.length < 1)
                    return;

                    this.validation[validationfield] = text;
                    this.$forceUpdate();
            },
            clearValidationText: function (validationfield)
            {
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

                if (!context.viewretrieveinfo.filterValue)
                    return;
                if (context.viewretrieveinfo.filterValue.length == 0)
                    return;

                var endpointurl = baseurl + "GetDataViewValue";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(context.viewretrieveinfo),
                    success: function (response) {
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
            openDataViewLookUp: function (viewname, line) {
                if (!viewname)
                    return;

                this.viewretrieveinfo.maxCount = 0;
                this.viewretrieveinfo.currentRowNum = 0;
                this.viewretrieveinfo.filterField = "";
                this.viewretrieveinfo.filterValue = "";
                this.viewretrieveinfo.dataViewMetaCode = viewname;
                this.getDataViewLookUpPage();
                this.current_edit_line = line;
                $("#" + viewname).modal();
            },
            nextDataViewLookUpPage: function () {
                var context = this;
                context.viewretrieveinfo.currentRowNum += context.viewretrieveinfo.batchSize;
                context.getDataViewLookUpPage();
            },
            prevDataViewLookUpPage: function () {
                var context = this;
                context.viewretrieveinfo.currentRowNum -= context.viewretrieveinfo.batchSize;
                context.getDataViewLookUpPage();
            },
            getDataViewLookUpPage: function () {
                var context = this;

                var endpointurl = baseurl + "GetDataView";

                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(context.viewretrieveinfo),
                    success: function (response) {
                        //DATA
                        context.dataview = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.viewretrieveinfo = response.retriveListArgs;
                    }
                });
            },
            isFirstDataViewPage: function () {
                return this.listRetrieveInfo.currentRowNum <= 0;
            },
            handleDataViewFilter: function () {
                var context = this;
                context.viewretrieveinfo.currentRowNum = 0;
                context.getDataViewLookUpPage();

            },
            addSubTableLine: function (tablename) {
                var context = this;
                context.model[tablename].push({ Id: 0, ParentId: 0 });
                context.$forceUpdate();
            },
        },
        mounted: function () {
            this.runMounted();

        }
    });


    return app;

}