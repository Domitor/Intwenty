
function createVue(vueelement, applicationid, baseurl)
{
   

    var app = new Vue({
        el: vueelement,
        data: {
            datalist: []
            , model: { "filtervalue": "", "filterfield": "" }
            , listRetrieveInfo: { "applicationId": applicationid, "maxCount": 0, "dataViewMetaCode": "", "listViewMetaCode": "", "batchSize": 20, "currentRowNum": 0, "filterField": "", "filterValue": "" }
            , currentSort: ''
            , currentSortDir: 'asc'
            , baseUrl: baseurl

        },
        methods: {
            downloadExcel: function () {
                var context = this;
                alert('test');
            },
            nextpage: function () {
                var context = this;
                context.listRetrieveInfo.currentRowNum += context.listRetrieveInfo.batchSize;
                context.getPage();
            },
            prevpage: function () {
                var context = this;
                context.listRetrieveInfo.currentRowNum -= context.listRetrieveInfo.batchSize;
                context.getPage();
            },
            getPage: function () {
                var context = this;
                var endpointurl = context.baseUrl + "GetListView";


                $.ajax({
                    url: endpointurl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(this.listRetrieveInfo),
                    success: function (response) {
                        //DATA
                        context.datalist = JSON.parse(response.data);

                        //UPDATE CURRENT PAGE INFO
                        context.listRetrieveInfo = response.retriveListArgs;
                    }
                });
            },
            handleFilterValue: function () {
                var context = this;

                if (!context.model.filterfield || context.model.filterfield == "")
                    return;

                if (context.model.filtervalue == "")
                    return;

                if (context.model.filtervalue != context.listRetrieveInfo.filterValue) {
                    context.listRetrieveInfo.currentRowNum = 0;
                    context.listRetrieveInfo.filterField = context.model.filterfield;
                    context.listRetrieveInfo.filterValue = context.model.filtervalue;
                    context.getPage();
                }


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
                return this.listRetrieveInfo.currentRowNum <= 0;
            }
        },
        mounted: function () {
            var context = this;
            context.getPage();
        }
    });

    return app;
}