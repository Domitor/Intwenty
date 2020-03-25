
function createVue(vueelement, applicationid, appmetacode, baseurl)
{

    var app = new Vue({
        el: vueelement,
        data: {
            dataview: [],
            valuedomains: {},
            model: { [appmetacode]:{}},
            viewretrieveinfo: { "applicationId": applicationid, "dataViewMetaCode": "", "maxCount": 0, "batchSize": 10, "currentRowNum": 0, "filterField": "", "filterValue": "" }
        },
        methods:
        {
            saveApplication() {
                this.runSave();
            },
            /*
            * Client validation.
            * Check if this application can be changed.
            */
            canSave: function () {
                var context = this;
                var result = true;
                $("[data-required]").each(function () {
                    var required = $(this).data('required');
                    if (required === "True") {
                        var dbfield = $(this).data('dbfield');
                        if (!context.model[appmetacode][dbfield]) {
                            result = false;
                            $(this).addClass('requiredNotValid');
                        }
                        else if (context.model[appmetacode][dbfield].length == 0) {
                            result = false;
                            $(this).addClass('requiredNotValid');
                        }
                        else {
                            $(this).removeClass('requiredNotValid');
                        }
                    }
                });


                return result;
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
                        if (!dbfield || !viewfield)
                            continue;
                        if (dbfield === "" || viewfield === "")
                            continue;

                        context.model[appmetacode][dbfield] = item[viewfield];
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
                            context.viewretrieveinfo.filterValue = context.model[appmetacode][dbfield];
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
                                context.model[appmetacode][dbfield] = dataviewitem[viewfield];
                                context.$forceUpdate();
                            }
                        });


                    }
                });
            },
            openDataViewLookUp: function (viewname) {
                if (!viewname)
                    return;

                this.viewretrieveinfo.maxCount = 0;
                this.viewretrieveinfo.currentRowNum = 0;
                this.viewretrieveinfo.filterField = "";
                this.viewretrieveinfo.filterValue = "";
                this.viewretrieveinfo.dataViewMetaCode = viewname;
                this.getDataViewLookUpPage();
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